using Godot;
using HowManyBunnies;

/// <summary>
/// Resource que define un efecto de sonido con sus propiedades.
/// Crear uno por cada tipo de sonido en el editor.
/// </summary>
[GlobalClass]
public partial class SoundEffect : Resource
{
	[Export] public SoundEffectType Type { get; set; } = SoundEffectType.None;
	[Export] public AudioStream Stream { get; set; }

	[ExportGroup("Volume & Pitch")]
	[Export(PropertyHint.Range, "-40,20,0.1")] public float VolumeDb { get; set; } = 0f;
	[Export(PropertyHint.Range, "0.1,4,0.01")] public float PitchScale { get; set; } = 1f;
	[Export(PropertyHint.Range, "0,1,0.01")] public float PitchRandomness { get; set; } = 0f;

	[ExportGroup("Limits")]
	[Export(PropertyHint.Range, "0,20,1")] public int MaxSimultaneous { get; set; } = 5;

	// Contador interno (no exportado)
	private int _activeCount = 0;

	/// <summary>Retorna true si se puede reproducir otro sonido de este tipo</summary>
	public bool CanPlay()
	{
		return MaxSimultaneous == 0 || _activeCount < MaxSimultaneous;
	}

	/// <summary>Incrementa contador de sonidos activos</summary>
	public void IncrementCount()
	{
		_activeCount++;
	}

	/// <summary>Decrementa contador cuando termina un sonido</summary>
	public void DecrementCount()
	{
		_activeCount = Mathf.Max(0, _activeCount - 1);
	}

	/// <summary>Calcula pitch con randomness aplicado</summary>
	public float GetRandomizedPitch()
	{
		if (PitchRandomness <= 0) return PitchScale;
		return PitchScale + (float)GD.RandRange(-PitchRandomness, PitchRandomness);
	}
}
