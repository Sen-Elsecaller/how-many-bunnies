using Godot;

/// <summary>
/// Estado de ir a dormir. El conejo va hacia la casa.
/// Se activa cuando GameManager emite SunsetStarted.
/// </summary>
public partial class BunnyGoingToSleepState : BunnyState
{
	[Export]
	public float SpeedMultiplier { get; set; } = 1.2f;

	[Export]
	public float ArrivalDistance { get; set; } = 5f;

	private Vector2 _housePosition;

	public override void Enter()
	{
		// Obtener posición de la casa desde Utils
		if (Utils.Instance.BunnyHouse != null)
		{
			_housePosition = Utils.Instance.BunnyHouse.GlobalPosition;
		}
		else
		{
			// Sin casa, quedarse donde está
			TransitionTo("sleeping");
			return;
		}

		if (Animations != null && Animations.SpriteFrames.HasAnimation("walk"))
		{
			Animations.Play("walk");
		}
	}

	public override void Update(double delta)
	{
		Vector2 direction = (_housePosition - Parent.GlobalPosition).Normalized();
		Parent.Move(direction, delta, SpeedMultiplier);

		float distance = Parent.GlobalPosition.DistanceTo(_housePosition);
		if (distance <= ArrivalDistance)
		{
			// Posicionar exactamente en la casa
			Parent.GlobalPosition = _housePosition;
			TransitionTo("sleeping");
		}
	}
}
