using Godot;

/// <summary>
/// Estado de caminar del conejo.
/// Elige una direccion aleatoria, camina por un tiempo, luego vuelve a Idle.
/// </summary>
public partial class BunnyWanderingState : BunnyState
{
	[Export]
	public float MinWanderTime { get; set; } = 1.0f;

	[Export]
	public float MaxWanderTime { get; set; } = 3.0f;

	private Vector2 _direction;
	private float _wanderTimer;
	private float _targetTime;

	public override void Enter()
	{
		// Direccion aleatoria normalizada
		float angle = (float)GD.RandRange(0, Mathf.Tau);
		_direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

		// Tiempo aleatorio de caminata
		_wanderTimer = 0f;
		_targetTime = (float)GD.RandRange(MinWanderTime, MaxWanderTime);

		if (Animations != null && Animations.SpriteFrames.HasAnimation("walk"))
		{
			Animations.Play("walk");
		}
	}

	public override void Update(double delta)
	{
		_wanderTimer += (float)delta;

		Parent.Move(_direction, delta);

		if (_wanderTimer >= _targetTime)
		{
			TransitionTo("idle");
		}
	}
}
