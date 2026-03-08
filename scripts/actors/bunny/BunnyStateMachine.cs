using Godot;
using Godot.Collections;

/// <summary>
/// Maquina de estados para el conejo.
/// Maneja transiciones entre estados y delega Update() al estado actual.
/// </summary>
[GlobalClass]
public partial class BunnyStateMachine : Node
{
	private Dictionary<string, BunnyState> _states = new();
	private BunnyState _currentState;
	private CarrotEdible _targetCarrot;
	private Bunny _bunny;

	[Export]
	public BunnyState InitialState { get; set; }

	public CarrotEdible TargetCarrot => _targetCarrot;

	/// <summary>
	/// Estado actual (solo lectura).
	/// </summary>
	public BunnyState CurrentState => _currentState;

	/// <summary>
	/// Nombre del estado actual para debug (lowercase).
	/// </summary>
	public string CurrentStateName => _currentState?.Name.ToString().ToLower() ?? "none";

	public bool IsInSleepState => CurrentStateName == "sleeping" || CurrentStateName == "goingtosleep";

	// --- Target Carrot ---

	public void OnCarrotDetected(Area2D area)
	{
		if (IsInSleepState) return;
		if (_bunny.IsAdult) return;
		if (_targetCarrot != null) return;

		var parent = area.GetParent();
		if (parent is CarrotEdible carrot)
		{
			SetTargetCarrot(carrot);
		}
	}

	public void SetTargetCarrot(CarrotEdible carrot)
	{
		if (_targetCarrot != null) return;

		_targetCarrot = carrot;
		carrot.Consumed += OnTargetCarrotConsumed;
		ForceChangeState("watching");
	}

	public void ClearTargetCarrot()
	{
		if (_targetCarrot != null && GodotObject.IsInstanceValid(_targetCarrot))
		{
			_targetCarrot.Consumed -= OnTargetCarrotConsumed;
		}
		_targetCarrot = null;
	}

	private void OnTargetCarrotConsumed(CarrotEdible carrot)
	{
		ClearTargetCarrot();
		ForceChangeState("idle");
	}

	/// <summary>
	/// Inicializa la FSM. Llamar desde Bunny._Ready().
	/// </summary>
	public void Init(Bunny bunny, AnimatedSprite2D animations)
	{
		_bunny = bunny;

		// Registrar todos los estados hijos
		foreach (Node child in GetChildren())
		{
			if (child is BunnyState state)
			{
				string stateName = child.Name.ToString().ToLower();
				_states[stateName] = state;

				// Inyectar referencias
				state.Parent = bunny;
				state.Animations = animations;
				state.StateMachine = this;

				// Conectar signal de transicion
				state.StateTransition += OnStateTransition;
			}
		}

		// Entrar al estado inicial
		if (InitialState != null)
		{
			_currentState = InitialState;
			_currentState.Enter();
		}

		// Escuchar ciclo día/noche
		GameManager.Instance.SunsetStarted += OnSunsetStarted;
		GameManager.Instance.DayStarted += OnDayStarted;
	}

	private void OnSunsetStarted()
	{
		ForceChangeState("goingtosleep");
	}

	private void OnDayStarted()
	{
		if (IsInSleepState)
		{
			ForceChangeState("idle");
		}
	}

	public override void _ExitTree()
	{
		// Desconectar signals de estados
		foreach (var state in _states.Values)
		{
			state.StateTransition -= OnStateTransition;
		}

		// Desconectar signals de GameManager
		if (GameManager.Instance != null)
		{
			GameManager.Instance.SunsetStarted -= OnSunsetStarted;
			GameManager.Instance.DayStarted -= OnDayStarted;
		}

		ClearTargetCarrot();
	}

	public override void _PhysicsProcess(double delta)
	{
		_currentState?.Update(delta);
	}

	/// <summary>
	/// Callback de la signal StateTransition.
	/// Solo procesa si el estado origen es el actual.
	/// </summary>
	private void OnStateTransition(BunnyState sourceState, string newStateName)
	{
		// Ignorar si no viene del estado actual
		if (sourceState != _currentState)
		{
			return;
		}

		TransitionTo(newStateName);
	}

	/// <summary>
	/// Transicion normal entre estados.
	/// </summary>
	private void TransitionTo(string newStateName)
	{
		string key = newStateName.ToLower();

		if (!_states.TryGetValue(key, out BunnyState newState))
		{
			GD.PrintErr($"BunnyStateMachine: Estado '{newStateName}' no existe");
			return;
		}

		if (_currentState == newState)
		{
			return;
		}

		_currentState?.Exit();
		_currentState = newState;
		_currentState.Enter();
	}

	/// <summary>
	/// Forzar cambio de estado (ej: cuando recibe zanahoria, noche, etc.).
	/// </summary>
	public void ForceChangeState(string newStateName)
	{
		string key = newStateName.ToLower();

		if (!_states.TryGetValue(key, out BunnyState newState))
		{
			GD.PrintErr($"BunnyStateMachine: Estado '{newStateName}' no existe");
			return;
		}

		if (_currentState == newState)
		{
			return;
		}

		_currentState?.Exit();
		_currentState = newState;
		_currentState.Enter();
	}
}
