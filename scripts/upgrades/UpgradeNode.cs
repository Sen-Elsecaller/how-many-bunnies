using Godot;
using HowManyBunnies;

public partial class UpgradeNode : Control
{
	[Export] public UpgradeData Data { get; set; }

	// Colores para estados (ajustables en editor)
	[Export] public Color LockedColor { get; set; } = new Color(0.3f, 0.3f, 0.3f, 1f);
	[Export] public Color AvailableColor { get; set; } = new Color(1f, 1f, 1f, 1f);
	[Export] public Color PurchasedColor { get; set; } = new Color(0.5f, 1f, 0.5f, 1f);

	private UpgradeTree _tree;
	private UpgradeNode _parentUpgrade;
	private Button _button;
	private TextureRect _texture;
	private Panel _bgPanel;
	private Tween _tween;
	private Line2D _connectionLine;

	public override void _Ready()
	{
		_tree = GetTree().GetFirstNodeInGroup("upgrade_tree") as UpgradeTree;
		_parentUpgrade = GetParent() as UpgradeNode;
		_button = GetNode<Button>("MarginContainer/Button");
		_texture = GetNode<TextureRect>("%UpgradeTexture");
		_bgPanel = GetNodeOrNull<Panel>("MarginContainer/Button/BGPanel");

		if (Data != null && Data.Icon != null)
		{
			_texture.Texture = Data.Icon;
		}

		// Pivot para animaciones (centro-abajo)
		_button.PivotOffset = _button.Size * new Vector2(0.5f, 1f);

		// Conectar signals
		_button.Pressed += OnPressed;
		_button.MouseEntered += OnMouseEntered;
		_button.MouseExited += OnMouseExited;
		_button.ButtonDown += OnButtonDown;
		_button.ButtonUp += OnButtonUp;

		// Escuchar cambios de puntos
		_tree.PointsChanged += OnPointsChanged;

		// Crear línea de conexión hacia el padre
		CreateConnectionLine();

		UpdateVisualState();
	}

	private void CreateConnectionLine()
	{
		if (_parentUpgrade == null) return;

		_connectionLine = new Line2D();
		_connectionLine.Width = 2f;
		_connectionLine.DefaultColor = LockedColor;
		_connectionLine.ZIndex = -1;

		// Deferred: padre aún está configurando hijos durante _Ready()
		_parentUpgrade.CallDeferred(Node.MethodName.AddChild, _connectionLine);

		// Actualizar puntos después de agregar
		CallDeferred(MethodName.UpdateConnectionLine);
	}

	private void UpdateConnectionLine()
	{
		if (_connectionLine == null || _parentUpgrade == null) return;

		// Puntos: centro del padre → centro de este nodo
		var parentCenter = _parentUpgrade.Size / 2;
		var myCenter = GlobalPosition - _parentUpgrade.GlobalPosition + Size / 2;

		_connectionLine.ClearPoints();
		_connectionLine.AddPoint(parentCenter);
		_connectionLine.AddPoint(myCenter);

		// Color según estado
		if (IsPurchased() && _parentUpgrade.IsPurchased())
		{
			_connectionLine.DefaultColor = PurchasedColor;
		}
		else if (_parentUpgrade.IsPurchased())
		{
			_connectionLine.DefaultColor = AvailableColor;
		}
		else
		{
			_connectionLine.DefaultColor = LockedColor;
		}
	}

	public override void _ExitTree()
	{
		_button.Pressed -= OnPressed;
		_button.MouseEntered -= OnMouseEntered;
		_button.MouseExited -= OnMouseExited;
		_button.ButtonDown -= OnButtonDown;
		_button.ButtonUp -= OnButtonUp;

		if (_tree != null)
		{
			_tree.PointsChanged -= OnPointsChanged;
		}
	}

	// --- Estado visual ---

	public bool IsPurchased()
	{
		return Data != null && _tree != null && _tree.IsPurchased(Data);
	}

	public bool IsPrerequisiteMet()
	{
		// Sin padre = es raíz, siempre desbloqueado
		if (_parentUpgrade == null) return true;
		// El padre debe estar comprado
		return _parentUpgrade.IsPurchased();
	}

	public bool CanPurchase()
	{
		if (Data == null || _tree == null) return false;
		return _tree.CanPurchase(Data, IsPrerequisiteMet());
	}

	private void UpdateVisualState()
	{
		if (Data == null || _tree == null) return;

		if (IsPurchased())
		{
			SetVisualColor(PurchasedColor);
			_button.Disabled = true;
		}
		else if (CanPurchase())
		{
			SetVisualColor(AvailableColor);
			_button.Disabled = false;
		}
		else
		{
			SetVisualColor(LockedColor);
			_button.Disabled = true;
		}

		UpdateConnectionLine();
	}

	private void SetVisualColor(Color color)
	{
		if (_bgPanel != null)
		{
			_bgPanel.Modulate = color;
		}
	}


	// --- Eventos ---

	private void OnPressed()
	{
		if (!CanPurchase()) return;

		_tree.TryPurchase(Data, IsPrerequisiteMet());
	}

	private void OnPointsChanged(int available, int total)
	{
		UpdateVisualState();
	}

	// --- Animaciones (estilo AnimatedButton) ---

	private void OnMouseEntered()
	{
		_tween?.Kill();
		_button.Scale = new Vector2(1.04f, 1.04f);
		TooltipManager.Instance?.ShowUpgrade(Data, IsPurchased());
	}

	private void OnMouseExited()
	{
		_tween?.Kill();
		_button.Scale = Vector2.One;
		TooltipManager.Instance?.Hide();
	}

	private void OnButtonDown()
	{
		_tween?.Kill();
		_button.Scale = new Vector2(0.95f, 0.875f);
	}

	private void OnButtonUp()
	{
		_tween = CreateTween().SetEase(Tween.EaseType.Out);
		_tween.SetTrans(Tween.TransitionType.Elastic);
		_tween.TweenProperty(_button, "scale", Vector2.One, 0.25f);
	}
}
