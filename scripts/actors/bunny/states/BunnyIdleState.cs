using Godot;

/// <summary>
/// Estado de reposo del conejo.
/// Espera un tiempo aleatorio y luego transiciona a Wandering.
/// </summary>
public partial class BunnyIdleState : BunnyState
{
	[Export]
	public float MinIdleTime { get; set; } = 0.5f;

	[Export]
	public float MaxIdleTime { get; set; } = 2.0f;

	private float _idleTimer;
	private float _targetTime;

	public override void Enter()
	{
		// Escanear por zanahorias cercanas
		var carrot = Parent.ScanForNearbyCarrots();
		if (carrot != null)
		{
			StateMachine.SetTargetCarrot(carrot);
			return; // SetTargetCarrot ya transiciona a watching
		}

		// Animacion idle (si existe)
		if (Animations != null && Animations.SpriteFrames.HasAnimation("idle"))
		{
			Animations.Play("idle");
		}

		// Tiempo aleatorio antes de empezar a caminar
		_idleTimer = 0f;
		_targetTime = (float)GD.RandRange(MinIdleTime, MaxIdleTime);
	}

	public override void Update(double delta)
	{
		_idleTimer += (float)delta;

		if (_idleTimer >= _targetTime)
		{
			TransitionTo("wandering");
		}
	}
}
