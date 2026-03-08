using Godot;

/// <summary>
/// Estado de ir hacia la zanahoria objetivo.
/// Al llegar, transiciona a Eating.
/// </summary>
public partial class BunnyGoingToCarrotState : BunnyState
{
	[Export]
	public float SpeedMultiplier { get; set; } = 1.6f;

	[Export]
	public float ArrivalDistance { get; set; } = 10f;

	public override void Enter()
	{
		if (Animations != null && Animations.SpriteFrames.HasAnimation("walk"))
		{
			Animations.Play("walk");
		}
	}

	public override void Update(double delta)
	{
		var carrot = StateMachine.TargetCarrot;
		if (carrot == null || !IsInstanceValid(carrot))
		{
			StateMachine.ClearTargetCarrot();
			TransitionTo("idle");
			return;
		}

		Vector2 direction = (carrot.GlobalPosition - Parent.GlobalPosition).Normalized();
		Parent.Move(direction, delta, SpeedMultiplier);

		float distance = Parent.GlobalPosition.DistanceTo(carrot.GlobalPosition);
		if (distance <= ArrivalDistance)
		{
			TransitionTo("eating");
		}
	}
}
