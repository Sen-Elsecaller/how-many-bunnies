using Godot;

/// <summary>
/// HUD del gameplay. Muestra timer y botón para empezar el primer día.
/// </summary>
public partial class GameUI : Control
{
	private Label _timeLabel;
	private Button _startButton;

	public override void _Ready()
	{
		//GD.Print("[GameUI] _Ready called");

		_timeLabel = GetNode<Label>("%TimeLabel");
		_startButton = GetNodeOrNull<Button>("%StartButton");

		//GD.Print($"[GameUI] TimeLabel: {_timeLabel != null}, StartButton: {_startButton != null}");

		GameManager.Instance.DayTimeUpdated += OnDayTimeUpdated;

		if (_startButton != null)
		{
			_startButton.Pressed += OnStartPressed;
			GD.Print("[GameUI] StartButton connected");
		}
	}

	public override void _ExitTree()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.DayTimeUpdated -= OnDayTimeUpdated;
		}

		if (_startButton != null)
		{
			_startButton.Pressed -= OnStartPressed;
		}
	}

	private void OnDayTimeUpdated(float remaining)
	{
		int seconds = Mathf.CeilToInt(remaining);
		_timeLabel.Text = seconds.ToString();
	}

	private void OnStartPressed()
	{
		//GD.Print("[GameUI] StartButton pressed!");
		_startButton.Visible = false;
		GameManager.Instance.StartDay();
	}
}
