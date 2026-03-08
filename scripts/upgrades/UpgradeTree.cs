using Godot;
using Godot.Collections;
using HowManyBunnies;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeTree : Control
{
	[Signal] public delegate void UpgradePurchasedEventHandler(UpgradeData upgrade);
	[Signal] public delegate void PointsChangedEventHandler(int available, int total);

	// Bonificaciones acumuladas (otros sistemas consultan estos valores)
	public float TwinChance { get; private set; } = 0f;
	public int BonusBabiesPerCouple { get; private set; } = 0;
	public float BunnySpeedMultiplier { get; private set; } = 1f;

	private HashSet<string> _purchasedIds = new();

	public override void _EnterTree()
	{
		AddToGroup("upgrade_tree");
	}

	public override void _Ready()
	{
		BunniesManager.Instance.BunnyCountChanged += OnBunnyCountChanged;
	}

	public override void _ExitTree()
	{
		if (BunniesManager.Instance != null)
		{
			BunniesManager.Instance.BunnyCountChanged -= OnBunnyCountChanged;
		}
	}

	private void OnBunnyCountChanged(int count)
	{
		EmitSignal(SignalName.PointsChanged, GetAvailablePoints(), GetTotalPoints());
	}

	// --- Puntos ---

	public int GetTotalPoints()
	{
		BunniesManager.Instance.RefreshCount();
		return BunniesManager.Instance.BunnyCount;
	}

	public int GetSpentPoints()
	{
		int spent = 0;
		foreach (var node in GetAllUpgradeNodes())
		{
			if (_purchasedIds.Contains(node.Data.Id))
			{
				spent += node.Data.Cost;
			}
		}
		return spent;
	}

	private List<UpgradeNode> GetAllUpgradeNodes()
	{
		var nodes = new List<UpgradeNode>();
		CollectUpgradeNodes(this, nodes);
		return nodes;
	}

	private void CollectUpgradeNodes(Node parent, List<UpgradeNode> nodes)
	{
		foreach (var child in parent.GetChildren())
		{
			if (child is UpgradeNode upgradeNode)
			{
				nodes.Add(upgradeNode);
			}
			CollectUpgradeNodes(child, nodes);
		}
	}

	public int GetAvailablePoints()
	{
		return GetTotalPoints() - GetSpentPoints();
	}

	// --- Compra ---

	public bool IsPurchased(UpgradeData data)
	{
		return _purchasedIds.Contains(data.Id);
	}

	public bool CanPurchase(UpgradeData data, bool prerequisiteMet = true)
	{
		if (IsPurchased(data)) return false;
		if (!prerequisiteMet) return false;
		if (GetAvailablePoints() < data.Cost) return false;
		return true;
	}

	public bool TryPurchase(UpgradeData data, bool prerequisiteMet = true)
	{
		if (!CanPurchase(data, prerequisiteMet)) return false;

		_purchasedIds.Add(data.Id);
		ApplyEffect(data.EffectType, data.EffectValue);

		EmitSignal(SignalName.UpgradePurchased, data);
		EmitSignal(SignalName.PointsChanged, GetAvailablePoints(), GetTotalPoints());

		return true;
	}

	// --- Efectos ---
	// Valores en editor: 10 = 10%, 1 = +1 bebé, etc.

	private void ApplyEffect(UpgradeEffectType type, float value)
	{
		switch (type)
		{
			case UpgradeEffectType.TwinChance:
				// value=10 → +10% probabilidad
				TwinChance += value / 100f;
				break;

			case UpgradeEffectType.BabiesPerCouple:
				// value=1 → +1 bebé por pareja
				BonusBabiesPerCouple += (int)value;
				break;

			case UpgradeEffectType.BunnySpeed:
				// value=10 → +10% velocidad (1.0 + 0.1 = 1.1x)
				BunnySpeedMultiplier += value / 100f;
				break;

			case UpgradeEffectType.None:
			default:
				break;
		}
	}

	// --- Reset (para respec futuro) ---

	public void ResetAllUpgrades()
	{
		_purchasedIds.Clear();
		TwinChance = 0f;
		BonusBabiesPerCouple = 0;
		BunnySpeedMultiplier = 1f;

		EmitSignal(SignalName.PointsChanged, GetAvailablePoints(), GetTotalPoints());
	}

	// --- Serialización (para guardado futuro) ---

	public Array<string> GetPurchasedIds()
	{
		return new Array<string>(_purchasedIds);
	}

	public void LoadPurchasedIds(Array<string> ids)
	{
		ResetAllUpgrades();

		foreach (var node in GetAllUpgradeNodes())
		{
			if (ids.Contains(node.Data.Id))
			{
				_purchasedIds.Add(node.Data.Id);
				ApplyEffect(node.Data.EffectType, node.Data.EffectValue);
			}
		}

		EmitSignal(SignalName.PointsChanged, GetAvailablePoints(), GetTotalPoints());
	}
}
