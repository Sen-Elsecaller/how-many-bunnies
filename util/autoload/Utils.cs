using Godot;

/// <summary>
/// Autoload con referencias a nodos importantes del juego.
/// Configurar en Project Settings > Autoload con nombre "Utils".
/// Acceso: Utils.Instance.BunnyHouse
/// </summary>
public partial class Utils : Node
{
	public static Utils Instance { get; private set; }

	public Node2D BunnyHouse;
	public Node2D BunniesContainer;
	public UIManager UIManager;
	public RandomNumberGenerator Rng;
	public Vector2[] BunnyBounds;

	// Cursores
	private Texture2D _cursorDefault;
	private Texture2D _cursorPointing;
	private Texture2D _cursorGrabbing;

	public override void _Ready()
	{
		Instance = this;
		Rng = new RandomNumberGenerator();
		LoadCursors();
	}

	private void LoadCursors()
	{
		_cursorDefault = GD.Load<Texture2D>("res://assets/cursor/hand_open.png");
		_cursorPointing = GD.Load<Texture2D>("res://assets/cursor/hand_point.png");
		_cursorGrabbing = GD.Load<Texture2D>("res://assets/cursor/hand_closed.png");

		// Aplicar cursor default al iniciar
		SetDefaultCursor();
	}

	public void SetDefaultCursor()
	{
		Input.SetCustomMouseCursor(_cursorDefault, Input.CursorShape.Arrow);
	}

	public void SetPointingCursor()
	{
		Input.SetCustomMouseCursor(_cursorPointing, Input.CursorShape.Arrow);
	}

	public void SetGrabbingCursor()
	{
		Input.SetCustomMouseCursor(_cursorGrabbing, Input.CursorShape.Arrow);
	}
}
