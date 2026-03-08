using Godot;
using HowManyBunnies;

/// <summary>
/// Panel visual del tooltip. Debe estar en un CanvasLayer con layer alto (ej: 100).
/// Se registra automáticamente en TooltipManager.
///
/// Estructura esperada:
/// CanvasLayer (layer=100)
/// └── TooltipPanel (este script)
///     └── MarginContainer
///         └── RichTextLabel (%Label, bbcode_enabled=true, fit_content=true)
/// </summary>
[GlobalClass]
public partial class TooltipPanel : PanelContainer
{
	[ExportGroup("Position")]
	[Export] public Vector2 Offset { get; set; } = new(16, 16);

	[ExportGroup("Animation")]
	[Export] public float FadeDuration { get; set; } = 0.15f;

	[ExportGroup("Colors")]
	[Export] public Color TitleColor { get; set; } = Colors.Black;
	[Export] public Color DescColor { get; set; } = new Color(0.4f, 0.4f, 0.4f);
	[Export] public Color EffectColor { get; set; } = new Color(0f, 0.5f, 0.7f);
	[Export] public Color CostColor { get; set; } = new Color(0.7f, 0.5f, 0f);
	[Export] public Color PurchasedColor { get; set; } = new Color(0.2f, 0.6f, 0.2f);

	private RichTextLabel _label;
	private Tween _tween;
	private bool _isShowing;

	public override void _Ready()
	{
		_label = GetNode<RichTextLabel>("%Label");

		// Iniciar oculto
		Hide();
		Modulate = Colors.Transparent;

		// Registrar en manager
		TooltipManager.Instance?.RegisterPanel(this);
	}

	public override void _Input(InputEvent @event)
	{
		if (_isShowing && @event is InputEventMouseMotion)
		{
			UpdatePosition();
		}
	}

	public void ShowUpgrade(UpgradeData data, bool isPurchased)
	{
		if (data == null) return;

		var bbcode = FormatUpgrade(data, isPurchased);
		_label.Text = bbcode;

		// Forzar recálculo de tamaño
		_label.ResetSize();
		ResetSize();

		_isShowing = true;
		UpdatePosition();
		Show();
		TweenOpacity(Colors.White);
	}

	public async void HideAnimated()
	{
		_isShowing = false;
		var tween = TweenOpacity(Colors.Transparent);
		await ToSignal(tween, Tween.SignalName.Finished);

		if (!_isShowing)
		{
			Hide();
		}
	}

	private string FormatUpgrade(UpgradeData data, bool isPurchased)
	{
		var titleHex = TitleColor.ToHtml(false);
		var descHex = DescColor.ToHtml(false);
		var effectHex = EffectColor.ToHtml(false);
		var costHex = CostColor.ToHtml(false);
		var purchasedHex = PurchasedColor.ToHtml(false);

		// Efecto según tipo
		var effect = data.EffectType switch
		{
			UpgradeEffectType.TwinChance => $"[color=#{effectHex}]+{data.EffectValue}% gemelos[/color]",
			UpgradeEffectType.BunnySpeed => $"[color=#{effectHex}]+{data.EffectValue}% velocidad[/color]",
			UpgradeEffectType.BabiesPerCouple => $"[color=#{effectHex}]+{data.EffectValue} crías/pareja[/color]",
			_ => ""
		};

		// Estado
		var status = isPurchased
			? $"[color=#{purchasedHex}][Comprado][/color]"
			: $"Costo: [color=#{costHex}]{data.Cost}[/color]";

		var effectLine = string.IsNullOrEmpty(effect) ? "" : $"\n{effect}";

		return $"[color=#{titleHex}][b]{data.DisplayName}[/b][/color]\n[color=#{descHex}]{data.Description}[/color]{effectLine}\n{status}";
	}

	private Tween TweenOpacity(Color to)
	{
		_tween?.Kill();
		_tween = CreateTween();
		_tween.TweenProperty(this, "modulate", to, FadeDuration);
		return _tween;
	}

	private void UpdatePosition()
	{
		GlobalPosition = GetGlobalMousePosition() + Offset;
		ClampToViewport();
	}

	private void ClampToViewport()
	{
		var viewportSize = GetViewportRect().Size;
		var pos = GlobalPosition;

		if (pos.X + Size.X > viewportSize.X)
			pos.X = viewportSize.X - Size.X;

		if (pos.Y + Size.Y > viewportSize.Y)
			pos.Y = viewportSize.Y - Size.Y;

		pos.X = Mathf.Max(0, pos.X);
		pos.Y = Mathf.Max(0, pos.Y);

		GlobalPosition = pos;
	}
}
