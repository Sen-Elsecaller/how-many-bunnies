using Godot;

/// <summary>
/// Autoload que controla el color del cielo según fase del día.
/// Configurar en Project Settings > Autoload con nombre "ShaderManager".
///
/// Requiere en escena principal:
/// - CanvasModulate (se registra automáticamente si está en grupo "SkyLight")
///
/// Transiciones de 5 segundos al anochecer y amanecer.
/// </summary>
public partial class ShaderManager : Node
{
	public static ShaderManager Instance { get; private set; }

	private CanvasModulate _skyLight;
	private Tween _tween;

	[ExportGroup("Sky Colors")]
	[Export] public Color DayColor { get; set; } = new Color("ffffff");
	[Export] public Color SunsetColor { get; set; } = new Color("ff9966");
	[Export] public Color NightColor { get; set; } = new Color("1a1a2e");

	[ExportGroup("Transitions")]
	[Export] public float TransitionDuration { get; set; } = 5f;

	public override void _Ready()
	{
		Instance = this;

		GameManager.Instance.DayStarted += OnDayStarted;
		GameManager.Instance.SunsetStarted += OnSunsetStarted;
		GameManager.Instance.NightStarted += OnNightStarted;
	}

	public override void _ExitTree()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.DayStarted -= OnDayStarted;
			GameManager.Instance.SunsetStarted -= OnSunsetStarted;
			GameManager.Instance.NightStarted -= OnNightStarted;
		}
	}

	/// <summary>
	/// Registra el CanvasModulate. Llamar desde escena principal.
	/// </summary>
	public void RegisterSkyLight(CanvasModulate skyLight)
	{
		_skyLight = skyLight;
		_skyLight.Color = DayColor;
		GD.Print("[ShaderManager] SkyLight registered");
	}

	private void OnDayStarted()
	{
		TransitionTo(DayColor);
	}

	private void OnSunsetStarted()
	{
		TransitionTo(SunsetColor);
	}

	private void OnNightStarted()
	{
		TransitionTo(NightColor);
	}

	private void TransitionTo(Color targetColor)
	{
		if (_skyLight == null) return;

		_tween?.Kill();
		_tween = CreateTween();
		_tween.TweenProperty(_skyLight, "color", targetColor, TransitionDuration);
	}

	/// <summary>
	/// Aplica color inmediatamente sin transición.
	/// </summary>
	public void SetColorImmediate(GameManager.GamePhase phase)
	{
		if (_skyLight == null) return;

		_tween?.Kill();
		_skyLight.Color = phase switch
		{
			GameManager.GamePhase.Day => DayColor,
			GameManager.GamePhase.Sunset => SunsetColor,
			GameManager.GamePhase.Night => NightColor,
			_ => DayColor
		};
	}
}
