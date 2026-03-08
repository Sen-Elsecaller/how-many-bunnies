using Godot;

/// <summary>
/// Estado de comer la zanahoria.
/// Para por EatingDuration, come, destruye zanahoria, vuelve a Idle.
/// </summary>
public partial class BunnyEatingState : BunnyState
{
	[Export]
	public float EatingDuration { get; set; } = 1.0f;

	private float _eatTimer = 0f;
	private bool _hasEaten = false;

	public override void Enter()
	{
		_eatTimer = 0f;
		_hasEaten = false;

		// Animación de comer (si existe, sino idle)
		if (Animations != null)
		{
			if (Animations.SpriteFrames.HasAnimation("eat"))
			{
				Animations.Play("eat");
			}
			else if (Animations.SpriteFrames.HasAnimation("idle"))
			{
				Animations.Play("idle");
			}
		}
	}

	public override void Update(double delta)
	{
		_eatTimer += (float)delta;

		// Verificar que la zanahoria sigue existiendo
		var carrot = StateMachine.TargetCarrot;
		if (carrot == null || !IsInstanceValid(carrot))
		{
			// Otro conejo se la comió
			StateMachine.ClearTargetCarrot();
			TransitionTo("idle");
			return;
		}

		// Esperar duración de comer
		if (_eatTimer >= EatingDuration && !_hasEaten)
		{
			EatCarrot(carrot);
		}
	}

	private void EatCarrot(CarrotEdible carrot)
	{
		_hasEaten = true;

		// Alimentar al conejo
		Parent.Eat(carrot.NutritionValue);

		// Limpiar referencia antes de destruir
		StateMachine.ClearTargetCarrot();

		// Destruir zanahoria
		carrot.Consume();

		// Volver a idle
		TransitionTo("idle");
	}

	public override void Exit()
	{
		_hasEaten = false;
	}
}
