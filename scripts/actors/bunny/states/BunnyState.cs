using Godot;

/// <summary>
/// Clase base para todos los estados del conejo.
/// Los estados concretos heredan de esta clase e implementan su logica.
/// </summary>
public partial class BunnyState : Node
{
	[Signal]
	public delegate void StateTransitionEventHandler(BunnyState sourceState, string newStateName);

	// Referencias inyectadas por la FSM
	public Bunny Parent { get; set; }
	public AnimatedSprite2D Animations { get; set; }
	public BunnyStateMachine StateMachine { get; set; }

	/// <summary>
	/// Llamado al entrar al estado.
	/// </summary>
	public virtual void Enter()
	{
	}

	/// <summary>
	/// Llamado al salir del estado.
	/// </summary>
	public virtual void Exit()
	{
	}

	/// <summary>
	/// Llamado cada frame de fisica.
	/// </summary>
	public virtual void Update(double delta)
	{
	}

	/// <summary>
	/// Helper para transicionar a otro estado.
	/// </summary>
	protected void TransitionTo(string stateName)
	{
		EmitSignal(SignalName.StateTransition, this, stateName);
	}
}
