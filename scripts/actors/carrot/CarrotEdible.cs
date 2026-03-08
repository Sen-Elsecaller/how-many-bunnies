using Godot;

/// <summary>
/// Zanahoria cosechada que el jugador lanza.
/// Tiene física básica (velocidad + gravedad).
/// Los conejos la detectan y van hacia ella.
/// </summary>
[GlobalClass]
public partial class CarrotEdible : Node2D
{
	public enum CarrotState { Held, Flying, Grounded }

	[Signal]
	public delegate void GroundedEventHandler(CarrotEdible carrot);

	[Signal]
	public delegate void ConsumedEventHandler(CarrotEdible carrot);

	[Export]
	public int NutritionValue { get; set; } = 2;

	[Export]
	public float ThrowForceMultiplier { get; set; } = 0.5f;

	[Export]
	public float Friction { get; set; } = 5f;

	[Export]
	public float MinSpeedToStop { get; set; } = 10f;

	private CarrotState _state = CarrotState.Held;
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _lastMousePosition;
	private Vector2 _mouseVelocity;

	public CarrotState State => _state;
	public bool IsGrounded => _state == CarrotState.Grounded;
	public bool IsFlying => _state == CarrotState.Flying;

	public override void _Ready()
	{
		_lastMousePosition = GetGlobalMousePosition();
	}

	public override void _Process(double delta)
	{
		switch (_state)
		{
			case CarrotState.Held:
				ProcessHeld(delta);
				break;
			case CarrotState.Flying:
				ProcessFlying(delta);
				break;
			case CarrotState.Grounded:
				// Quieta en el suelo, esperando ser comida
				break;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (_state != CarrotState.Held)
		{
			return;
		}

		// Soltar con release del click
		if (@event is InputEventMouseButton mouseEvent &&
			mouseEvent.ButtonIndex == MouseButton.Left &&
			!mouseEvent.Pressed)
		{
			Throw();
		}
	}

	/// <summary>
	/// Inicia el seguimiento del mouse (llamado al ser cosechada).
	/// </summary>
	public void StartFollowingMouse()
	{
		_state = CarrotState.Held;
		_lastMousePosition = GetGlobalMousePosition();
		Utils.Instance?.SetGrabbingCursor();
	}

	private void ProcessHeld(double delta)
	{
		// Seguir mouse
		Vector2 currentMousePos = GetGlobalMousePosition();
		GlobalPosition = currentMousePos;

		// Calcular velocidad del mouse para el lanzamiento
		_mouseVelocity = (currentMousePos - _lastMousePosition) / (float)delta;
		_lastMousePosition = currentMousePos;
	}

	private void Throw()
	{
		_state = CarrotState.Flying;
		Utils.Instance?.SetDefaultCursor();

		// Velocidad inicial basada en movimiento del mouse
		_velocity = _mouseVelocity * ThrowForceMultiplier;
	}

	private void ProcessFlying(double delta)
	{
		// Aplicar fricción (desacelerar)
		_velocity = _velocity.MoveToward(Vector2.Zero, Friction * (float)delta * _velocity.Length());

		// Mover
		GlobalPosition += _velocity * (float)delta;

		// Verificar si se detuvo
		if (_velocity.Length() < MinSpeedToStop)
		{
			Land();
		}
	}

	private void Land()
	{
		_state = CarrotState.Grounded;
		_velocity = Vector2.Zero;

		EmitSignal(SignalName.Grounded, this);
	}

	/// <summary>
	/// Llamado por un conejo cuando la come.
	/// </summary>
	public void Consume()
	{
		EmitSignal(SignalName.Consumed, this);
		QueueFree();
	}
}
