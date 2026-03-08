using Godot;

/// <summary>
/// Zanahoria que crece en el campo.
/// Tiene 3 etapas de crecimiento. Al clickear en etapa 3, genera CarrotEdible.
/// </summary>
[GlobalClass]
public partial class CarrotCrop : Node2D
{
	[Signal]
	public delegate void HarvestedEventHandler(CarrotCrop crop, Vector2 position);

	public enum GrowthStage { Stage1, Stage2, Stage3 }

	[Export]
	public float GrowthTime { get; set; } = 3.0f;

	[Export]
	public Texture2D Stage1Texture { get; set; }

	[Export]
	public Texture2D Stage2Texture { get; set; }

	[Export]
	public Texture2D Stage3Texture { get; set; }

	[Export]
	public PackedScene CarrotEdibleScene { get; set; }

	private Sprite2D _sprite;
	private Area2D _clickArea;
	private GrowthStage _currentStage = GrowthStage.Stage1;
	private float _growthTimer = 0f;

	public GrowthStage CurrentStage => _currentStage;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_clickArea = GetNode<Area2D>("ClickArea");

		// Conectar signal de click
		_clickArea.InputEvent += OnInputEvent;

		// Iniciar en stage 1
		UpdateVisual();
	}

	public override void _ExitTree()
	{
		_clickArea.InputEvent -= OnInputEvent;
	}

	public override void _Process(double delta)
	{
		// Solo crecer si no está en stage 3
		if (_currentStage == GrowthStage.Stage3)
		{
			return;
		}

		_growthTimer += (float)delta;

		if (_growthTimer >= GrowthTime)
		{
			_growthTimer = 0f;
			AdvanceStage();
		}
	}

	private void AdvanceStage()
	{
		if (_currentStage == GrowthStage.Stage1)
		{
			_currentStage = GrowthStage.Stage2;
		}
		else if (_currentStage == GrowthStage.Stage2)
		{
			_currentStage = GrowthStage.Stage3;
		}

		UpdateVisual();
	}

	private void UpdateVisual()
	{
		if (_sprite == null)
		{
			return;
		}

		_sprite.Texture = _currentStage switch
		{
			GrowthStage.Stage1 => Stage1Texture,
			GrowthStage.Stage2 => Stage2Texture,
			GrowthStage.Stage3 => Stage3Texture,
			_ => Stage1Texture
		};
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		// Solo procesar click si está lista para cosechar
		if (_currentStage != GrowthStage.Stage3)
		{
			return;
		}

		if (@event is InputEventMouseButton mouseEvent &&
			mouseEvent.ButtonIndex == MouseButton.Left &&
			mouseEvent.Pressed)
		{
			Harvest();
		}
	}

	private void Harvest()
	{
		// Crear zanahoria edible
		if (CarrotEdibleScene != null)
		{
			var edible = CarrotEdibleScene.Instantiate<CarrotEdible>();
			GetTree().CurrentScene.AddChild(edible);
			edible.GlobalPosition = GlobalPosition;
			edible.StartFollowingMouse();
		}

		// Emitir signal para que el field sepa que este hueco está libre
		EmitSignal(SignalName.Harvested, this, GlobalPosition);

		// Destruir el crop
		QueueFree();
	}

	/// <summary>
	/// Reinicia el crop para reutilizar (alternativa a destruir).
	/// </summary>
	public void Reset()
	{
		_currentStage = GrowthStage.Stage1;
		_growthTimer = 0f;
		UpdateVisual();
	}
}
