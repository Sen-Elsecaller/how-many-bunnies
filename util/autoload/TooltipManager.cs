using Godot;

public partial class TooltipManager : Node
{
	public static TooltipManager Instance { get; private set; }

	private TooltipPanel _panel;

	public override void _Ready()
	{
		Instance = this;
	}

	/// <summary>Llamado por TooltipPanel en su _Ready()</summary>
	public void RegisterPanel(TooltipPanel panel)
	{
		_panel = panel;
	}

	public void ShowUpgrade(UpgradeData data, bool isPurchased)
	{
		_panel?.ShowUpgrade(data, isPurchased);
	}

	public void Hide()
	{
		_panel?.HideAnimated();
	}
}
