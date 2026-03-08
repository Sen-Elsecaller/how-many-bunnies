using Godot;

/// <summary>
/// Autoload que controla el ciclo día/noche y reproducción.
/// Configurar en Project Settings > Autoload con nombre "GameManager".
/// Acceso: GameManager.Instance.StartDay()
/// </summary>
public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public enum GamePhase { Day, Sunset, Night }

	[Signal]
	public delegate void PhaseChangedEventHandler(int phase);

	[Signal]
	public delegate void SunsetStartedEventHandler();

	[Signal]
	public delegate void NightStartedEventHandler();

	[Signal]
	public delegate void DayStartedEventHandler();

	[Signal]
	public delegate void DayTimeUpdatedEventHandler(float remaining);

	[Export]
	public float DayDuration { get; set; } = 20f;

	[Export]
	public float SunsetDuration { get; set; } = 5f;

	[Export]
	public float EagleSpawnTime { get; set; } = 3f; // Segundos restantes cuando aparece el águila

	[Export]
	public PackedScene EagleScene { get; set; }

	public GamePhase CurrentPhase { get; private set; } = GamePhase.Day;
	public float TimeRemaining { get; private set; }

	private bool _timerActive = false;
	private bool _eagleSpawned = false;

	public override void _Ready()
	{
		Instance = this;
		TimeRemaining = DayDuration;
	}

	public override void _Process(double delta)
	{
		if (!_timerActive || CurrentPhase == GamePhase.Night)
		{
			return;
		}

		TimeRemaining -= (float)delta;
		EmitSignal(SignalName.DayTimeUpdated, TimeRemaining);

		// Transición a Sunset cuando quedan 5 segundos
		if (CurrentPhase == GamePhase.Day && TimeRemaining <= SunsetDuration)
		{
			SetPhase(GamePhase.Sunset);
		}

		// Spawn eagle durante sunset
		if (CurrentPhase == GamePhase.Sunset && !_eagleSpawned)
		{
			if (TimeRemaining <= EagleSpawnTime)
			{
				GD.Print($"[Eagle] Condition met! Time: {TimeRemaining:F1}");
				SpawnEagle();
			}
		}

		// Transición a Night cuando llega a 0
		if (TimeRemaining <= 0)
		{
			TimeRemaining = 0;
			SetPhase(GamePhase.Night);
		}
	}

	/// <summary>
	/// Inicia el ciclo del día. Llamar al inicio del juego o después del menú de upgrades.
	/// </summary>
	public void StartDay()
	{
		TimeRemaining = DayDuration;
		_timerActive = true;
		_eagleSpawned = false;
		SetPhase(GamePhase.Day);
	}

	private void SpawnEagle()
	{
		_eagleSpawned = true;
		GD.Print($"[Eagle] SpawnEagle called, bunnies: {BunniesManager.Instance?.BunnyCount}");

		var target = BunniesManager.Instance?.GetRandomBunny();
		GD.Print($"[Eagle] Target: {target?.Name ?? "NULL"}");
		if (target == null) return;

		EagleScene ??= GD.Load<PackedScene>("res://scenes/actors/eagle.tscn");
		if (EagleScene == null) { GD.Print("[Eagle] Scene NULL"); return; }

		var eagle = EagleScene.Instantiate<Eagle>();
		GetTree().CurrentScene.AddChild(eagle);
		eagle.Launch(target);
		GD.Print("[Eagle] Launched!");
	}

	/// <summary>
	/// Llamar desde el botón del menú de upgrades para empezar nuevo día.
	/// </summary>
	public void OnUpgradeMenuClosed()
	{
		BunniesManager.Instance.ProcessReproduction();
		StartDay();
	}

	private void SetPhase(GamePhase newPhase)
	{
		if (CurrentPhase == newPhase)
		{
			return;
		}

		CurrentPhase = newPhase;
		EmitSignal(SignalName.PhaseChanged, (int)newPhase);

		switch (newPhase)
		{
			case GamePhase.Sunset:
				GD.Print("[GameManager] Sunset started - bunnies going to sleep");
				EmitSignal(SignalName.SunsetStarted);
				break;
			case GamePhase.Night:
				GD.Print("[GameManager] Night started - day ended");
				_timerActive = false;
				EmitSignal(SignalName.NightStarted);
				break;
			case GamePhase.Day:
				GD.Print($"[GameManager] Day started - duration: {DayDuration}s");
				EmitSignal(SignalName.DayStarted);
				break;
		}
	}

	/// <summary>
	/// Agregar tiempo extra al día (para upgrades).
	/// </summary>
	public void AddDayTime(float extraSeconds)
	{
		DayDuration += extraSeconds;
	}

	/// <summary>
	/// Cambia a la escena del juego desde el menú principal.
	/// </summary>
	public void StartGame()
	{
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
	}

	/// <summary>
	/// Vuelve al menú principal.
	/// </summary>
	public void ReturnToMenu()
	{
		_timerActive = false;
		CurrentPhase = GamePhase.Day;
		TimeRemaining = DayDuration;
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
