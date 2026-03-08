using Godot;

/// <summary>
/// Entidad principal del conejo.
/// Maneja crecimiento, alimentación, detección de zanahorias y personalidad.
/// </summary>
[GlobalClass]
public partial class Bunny : Node2D
{
	[Signal]
	public delegate void BecameAdultEventHandler(Bunny bunny);

	[Signal]
	public delegate void AteEventHandler(Bunny bunny, int currentNutrition);

	[Export]
	public int NutritionToAdult { get; set; } = 4;

	[Export]
	public float BabyScale { get; set; } = 0.5f;

	[Export]
	public float AdultScale { get; set; } = 1.0f;

	[Export(PropertyHint.Range, "0,1")]
	public float AlertChance { get; set; } = 0.3f;

	[Export]
	public float AlertReactionDelay { get; set; } = 0.3f;

	[Export]
	public float BaseMoveSpeed { get; set; } = 20f;

	private AnimatedSprite2D _animations;
	private BunnyStateMachine _stateMachine;
	private Area2D _carrotDetection;

	private int _currentNutrition = 0;
	private bool _isAdult = false;
	private bool _isAlert = false;

	public bool IsAdult => _isAdult;
	public bool IsAlert => _isAlert;
	public int CurrentNutrition => _currentNutrition;
	public BunnyStateMachine StateMachine => _stateMachine;

	public override void _Ready()
	{
		_animations = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_stateMachine = GetNode<BunnyStateMachine>("BunnyStateMachine");
		_carrotDetection = GetNodeOrNull<Area2D>("CarrotDetection");

		_stateMachine.Init(this, _animations);

		// Determinar si es espabilado
		_isAlert = GD.Randf() < AlertChance;

		// Escala inicial de bebé
		UpdateScale();

		// Conectar detección de zanahorias a la FSM
		if (_carrotDetection != null)
		{
			_carrotDetection.AreaEntered += _stateMachine.OnCarrotDetected;
		}
	}

	public override void _ExitTree()
	{
		if (_carrotDetection != null && _stateMachine != null)
		{
			var callable = Callable.From<Area2D>(_stateMachine.OnCarrotDetected);
			if (_carrotDetection.IsConnected(Area2D.SignalName.AreaEntered, callable))
			{
				_carrotDetection.AreaEntered -= _stateMachine.OnCarrotDetected;
			}
		}
	}

	/// <summary>
	/// Escanea el área de detección por zanahorias disponibles.
	/// Retorna la primera zanahoria encontrada, o null si no hay o si es adulto.
	/// </summary>
	public CarrotEdible ScanForNearbyCarrots()
	{
		if (_isAdult) return null;
		if (_carrotDetection == null) return null;

		foreach (var area in _carrotDetection.GetOverlappingAreas())
		{
			var parent = area.GetParent();
			if (parent is CarrotEdible carrot && GodotObject.IsInstanceValid(carrot))
			{
				return carrot;
			}
		}

		return null;
	}

	/// <summary>
	/// Alimenta al conejo. Retorna true si comió, false si ya es adulto.
	/// </summary>
	public bool Eat(int nutrition = 1)
	{
		// Adultos no comen
		if (_isAdult)
		{
			return false;
		}

		_currentNutrition += nutrition;
		EmitSignal(SignalName.Ate, this, _currentNutrition);

		// Actualizar tamaño visual
		UpdateScale();

		// Verificar si llegó a adulto
		if (_currentNutrition >= NutritionToAdult)
		{
			BecomeAdult();
		}

		return true;
	}

	private void UpdateScale()
	{
		// Interpolar escala entre baby y adult según nutrición
		float progress = Mathf.Clamp((float)_currentNutrition / NutritionToAdult, 0f, 1f);
		float currentScale = Mathf.Lerp(BabyScale, AdultScale, progress);

		Scale = new Vector2(currentScale, currentScale);
	}

	private void BecomeAdult()
	{
		_isAdult = true;
		Scale = new Vector2(AdultScale, AdultScale);
		EmitSignal(SignalName.BecameAdult, this);
	}

	/// <summary>
	/// Reinicia el conejo a estado bebé (para pooling o nuevas crías).
	/// </summary>
	public void ResetToBaby()
	{
		_currentNutrition = 0;
		_isAdult = false;
		_isAlert = GD.Randf() < AlertChance;
		_stateMachine.ClearTargetCarrot();
		UpdateScale();
	}

	/// <summary>
	/// Mueve al conejo y actualiza dirección del sprite.
	/// Respeta los límites definidos en Utils.BunnyBounds.
	/// </summary>
	public void Move(Vector2 direction, double delta, float speedMultiplier = 1f)
	{
		// Multiplicar por upgrade de velocidad global (cacheado en BunniesManager)
		float upgradeSpeed = BunniesManager.Instance?.SpeedMultiplier ?? 1f;
		Vector2 newPosition = GlobalPosition + direction * BaseMoveSpeed * speedMultiplier * upgradeSpeed * (float)delta;

		// Limitar a los bounds si están definidos
		var bounds = Utils.Instance.BunnyBounds;
		if (bounds != null && bounds.Length > 0)
		{
			if (!Geometry2D.IsPointInPolygon(newPosition, bounds))
			{
				// Fuera del área, no mover
				return;
			}
		}

		GlobalPosition = newPosition;

		if (_animations != null && direction.X != 0)
		{
			_animations.FlipH = direction.X < 0;
		}
	}
}
