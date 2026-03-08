using Godot;

/// <summary>
/// Estado de observar una zanahoria volando.
/// Espabilados: esperan 0.3s y van.
/// Normales: esperan a que toque el suelo.
/// </summary>
public partial class BunnyWatchingState : BunnyState
{
	private float _waitTimer = 0f;
	private bool _carrotWasGrounded = false;

	public override void Enter()
	{
		_waitTimer = 0f;
		_carrotWasGrounded = false;

		// Animación de mirar (si existe, sino idle)
		if (Animations != null)
		{
			if (Animations.SpriteFrames.HasAnimation("watch"))
			{
				Animations.Play("watch");
			}
			else if (Animations.SpriteFrames.HasAnimation("idle"))
			{
				Animations.Play("idle");
			}
		}

		// Si la zanahoria ya está en el suelo, ir directo
		if (StateMachine.TargetCarrot != null && StateMachine.TargetCarrot.IsGrounded)
		{
			_carrotWasGrounded = true;
		}
	}

	public override void Update(double delta)
	{
		// Verificar que la zanahoria sigue existiendo
		var carrot = StateMachine.TargetCarrot;
		if (carrot == null || !IsInstanceValid(carrot))
		{
			StateMachine.ClearTargetCarrot();
			TransitionTo("idle");
			return;
		}

		// Mirar hacia la zanahoria
		LookAtCarrot(carrot);

		_waitTimer += (float)delta;

		// Lógica de espabilado vs normal
		if (Parent.IsAlert)
		{
			// Espabilado: espera AlertReactionDelay y va
			if (_waitTimer >= Parent.AlertReactionDelay)
			{
				TransitionTo("goingtocarrot");
			}
		}
		else
		{
			// Normal: espera a que toque el suelo
			if (carrot.IsGrounded && !_carrotWasGrounded)
			{
				// Acaba de tocar el suelo, ir
				TransitionTo("goingtocarrot");
			}
			else if (_carrotWasGrounded)
			{
				// Ya estaba en el suelo cuando empezamos a mirar
				TransitionTo("goingtocarrot");
			}
		}
	}

	private void LookAtCarrot(CarrotEdible carrot)
	{
		if (Animations == null)
		{
			return;
		}

		// Flip según posición de la zanahoria
		float direction = carrot.GlobalPosition.X - Parent.GlobalPosition.X;
		if (direction != 0)
		{
			Animations.FlipH = direction < 0;
		}
	}
}
