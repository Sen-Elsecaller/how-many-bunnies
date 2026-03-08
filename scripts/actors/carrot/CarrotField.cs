using Godot;
using System.Collections.Generic;

/// <summary>
/// Campo de zanahorias. Maneja los huecos (Marker2D) y el spawn de crops.
/// </summary>
[GlobalClass]
public partial class CarrotField : Node2D
{
	[Export]
	public PackedScene CarrotCropScene { get; set; }

	[Export]
	public float RespawnDelay { get; set; } = 2.0f;

	// Tracking de huecos: posición -> crop actual (null si vacío)
	private Dictionary<Vector2, CarrotCrop> _slots = new();

	// Queue de huecos esperando respawn
	private List<SlotRespawnTimer> _respawnQueue = new();

	private struct SlotRespawnTimer
	{
		public Vector2 Position;
		public float Timer;
	}

	public override void _Ready()
	{
		// Obtener todas las posiciones de los Marker2D hijos
		foreach (Node child in GetChildren())
		{
			if (child is Marker2D marker)
			{
				Vector2 slotPosition = marker.GlobalPosition;
				_slots[slotPosition] = null;

				// Spawn inicial
				SpawnCropAt(slotPosition);
			}
		}
	}

	public override void _Process(double delta)
	{
		// Procesar respawn queue
		for (int i = _respawnQueue.Count - 1; i >= 0; i--)
		{
			var item = _respawnQueue[i];
			item.Timer -= (float)delta;
			_respawnQueue[i] = item;

			if (item.Timer <= 0)
			{
				SpawnCropAt(item.Position);
				_respawnQueue.RemoveAt(i);
			}
		}
	}

	private void SpawnCropAt(Vector2 position)
	{
		if (CarrotCropScene == null)
		{
			GD.PrintErr("CarrotField: CarrotCropScene no asignada");
			return;
		}

		var crop = CarrotCropScene.Instantiate<CarrotCrop>();
		AddChild(crop);
		crop.GlobalPosition = position;

		// Conectar signal de cosecha
		crop.Harvested += OnCropHarvested;

		// Registrar en el slot
		_slots[position] = crop;
	}

	private void OnCropHarvested(CarrotCrop crop, Vector2 position)
	{
		// Desconectar signal
		crop.Harvested -= OnCropHarvested;

		// Marcar slot como vacío
		if (_slots.ContainsKey(position))
		{
			_slots[position] = null;
		}

		// Agregar a queue de respawn
		_respawnQueue.Add(new SlotRespawnTimer
		{
			Position = position,
			Timer = RespawnDelay
		});
	}

	/// <summary>
	/// Obtiene cuántos slots están ocupados con crops listos para cosechar.
	/// </summary>
	public int GetReadyCropCount()
	{
		int count = 0;
		foreach (var crop in _slots.Values)
		{
			if (crop != null && crop.CurrentStage == CarrotCrop.GrowthStage.Stage3)
			{
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// Obtiene el total de slots.
	/// </summary>
	public int GetTotalSlots() => _slots.Count;
}
