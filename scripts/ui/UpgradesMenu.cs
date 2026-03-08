using Godot;

/// <summary>
/// Menú nocturno de upgrades. Botón para empezar nuevo día.
/// </summary>
public partial class UpgradesMenu : Control
{
	private Button _newDayButton;
	private Label _bunnyCounter;
	private UpgradeTree _upgradeTree;

	public override void _Ready()
	{
		_newDayButton = GetNodeOrNull<Button>("%NewDayButton");
		_bunnyCounter = GetNodeOrNull<Label>("%BunnyCounter");
		_upgradeTree = GetTree().GetFirstNodeInGroup("upgrade_tree") as UpgradeTree;

		if (_newDayButton != null)
		{
			_newDayButton.Pressed += OnNewDayPressed;
		}

		if (_upgradeTree != null)
		{
			_upgradeTree.PointsChanged += OnPointsChanged;
		}

		VisibilityChanged += OnVisibilityChanged;
	}

	public override void _ExitTree()
	{
		if (_newDayButton != null)
		{
			_newDayButton.Pressed -= OnNewDayPressed;
		}

		if (_upgradeTree != null)
		{
			_upgradeTree.PointsChanged -= OnPointsChanged;
		}

		VisibilityChanged -= OnVisibilityChanged;
	}

	private void OnNewDayPressed()
	{
		GameManager.Instance.OnUpgradeMenuClosed();
	}

	private void OnPointsChanged(int available, int total)
	{
		UpdateBunnyCounter(available);
	}

	private void OnVisibilityChanged()
	{
		if (Visible && _upgradeTree != null)
		{
			UpdateBunnyCounter(_upgradeTree.GetAvailablePoints());
		}
	}

	private void UpdateBunnyCounter(int count)
	{
		if (_bunnyCounter != null)
		{
			_bunnyCounter.Text = count.ToString();
		}
	}
}
