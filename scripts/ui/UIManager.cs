using Godot;

/// <summary>
/// Maneja visibilidad de UIs. Se registra en Utils.
/// Cada UI (GameUI, UpgradesMenu) tiene su propio script con lógica específica.
/// </summary>
public partial class UIManager : CanvasLayer
{
	private Control _gameUI;
	private Control _pauseMenu;
	private Control _upgradesMenu;

	public override void _Ready()
	{
		Utils.Instance.UIManager = this;

		_gameUI = GetNodeOrNull<Control>("GameUI");
		_pauseMenu = GetNodeOrNull<Control>("PauseMenu");
		_upgradesMenu = GetNodeOrNull<Control>("UpgradesMenu");

		GameManager.Instance.NightStarted += OnNightStarted;
		GameManager.Instance.DayStarted += OnDayStarted;

		// Estado inicial: solo GameUI visible
		ShowGameUI();
	}

	public override void _ExitTree()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.NightStarted -= OnNightStarted;
			GameManager.Instance.DayStarted -= OnDayStarted;
		}
	}

	private void OnNightStarted() => ShowUpgrades();
	private void OnDayStarted() => ShowGameUI();

	public void ShowGameUI()
	{
		SetUIVisible(_gameUI, true);
		SetUIVisible(_pauseMenu, false);
		SetUIVisible(_upgradesMenu, false);
	}

	public void ShowPauseMenu()
	{
		SetUIVisible(_pauseMenu, true);
	}

	public void HidePauseMenu()
	{
		SetUIVisible(_pauseMenu, false);
	}

	public void ShowUpgrades()
	{
		SetUIVisible(_gameUI, false);
		SetUIVisible(_upgradesMenu, true);
	}

	private void SetUIVisible(Control ui, bool visible)
	{
		if (ui != null)
		{
			ui.Visible = visible;
		}
	}

	// TODO: Agregar tweens para transiciones
}
