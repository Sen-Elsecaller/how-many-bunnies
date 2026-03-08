using Godot;

/// <summary>
/// Autoload que centraliza todo lo relacionado con población de conejos.
/// Facade que oculta complejidad de conteo, spawning y modificadores.
/// Configurar en Project Settings > Autoload con nombre "BunniesManager".
/// </summary>
public partial class BunniesManager : Node
{
	public static BunniesManager Instance { get; private set; }

	[Signal] public delegate void BunnyCountChangedEventHandler(int count);
	[Signal] public delegate void BunniesReproducedEventHandler(int newBunnies, int totalBunnies);

	[Export] public PackedScene BunnyScene { get; set; }
	[Export] public int AdultsPerBaby { get; set; } = 2;

	public int BunnyCount { get; private set; } = 0;

	// Modifiers cacheados (actualizados via signal)
	public float SpeedMultiplier { get; private set; } = 1f;
	public float TwinChance { get; private set; } = 0f;
	public int BonusBabiesPerCouple { get; private set; } = 0;

	private UpgradeTree _upgradeTree;

	public override void _Ready()
	{
		Instance = this;

		if (BunnyScene == null)
		{
			BunnyScene = GD.Load<PackedScene>("uid://cthght2wgfni");
		}

		// Conectar a UpgradeTree cuando esté disponible
		CallDeferred(nameof(ConnectToUpgradeTree));
	}

	private void ConnectToUpgradeTree()
	{
		_upgradeTree = GetTree().GetFirstNodeInGroup("upgrade_tree") as UpgradeTree;
		if (_upgradeTree != null)
		{
			_upgradeTree.UpgradePurchased += OnUpgradePurchased;
			RefreshModifiers();
		}
	}

	public override void _ExitTree()
	{
		if (_upgradeTree != null)
		{
			_upgradeTree.UpgradePurchased -= OnUpgradePurchased;
		}
	}

	private void OnUpgradePurchased(UpgradeData upgrade)
	{
		RefreshModifiers();
	}

	private void RefreshModifiers()
	{
		if (_upgradeTree == null) return;
		SpeedMultiplier = _upgradeTree.BunnySpeedMultiplier;
		TwinChance = _upgradeTree.TwinChance;
		BonusBabiesPerCouple = _upgradeTree.BonusBabiesPerCouple;
	}

	// --- Población ---

	public void RefreshCount()
	{
		int newCount = GetTree().GetNodesInGroup("Bunnies").Count;
		if (newCount != BunnyCount)
		{
			BunnyCount = newCount;
			EmitSignal(SignalName.BunnyCountChanged, BunnyCount);
		}
	}

	public int CountAdults()
	{
		int count = 0;
		foreach (var node in GetTree().GetNodesInGroup("Bunnies"))
		{
			if (node is Bunny bunny && bunny.IsAdult)
			{
				count++;
			}
		}
		return count;
	}

	public Bunny GetRandomBunny()
	{
		var bunnies = GetTree().GetNodesInGroup("Bunnies");
		if (bunnies.Count == 0) return null;

		int index = GD.RandRange(0, bunnies.Count - 1);
		return bunnies[index] as Bunny;
	}

	// --- Spawning ---

	public Bunny SpawnBunny(Vector2 position)
	{
		if (BunnyScene == null)
		{
			GD.PrintErr("[BunniesManager] BunnyScene not assigned!");
			return null;
		}

		Node parent = Utils.Instance.BunniesContainer ?? GetTree().CurrentScene;
		var bunny = BunnyScene.Instantiate<Bunny>();
		bunny.GlobalPosition = position;
		bunny.AddToGroup("Bunnies");
		parent.AddChild(bunny);

		RefreshCount();
		return bunny;
	}

	public void RemoveBunny(Bunny bunny)
	{
		bunny.QueueFree();
		// RefreshCount se llamará en el siguiente frame después de QueueFree
		CallDeferred(nameof(RefreshCount));
	}

	// --- Reproducción ---

	public void ProcessReproduction()
	{
		int adultCount = CountAdults();
		int baseBabies = adultCount / AdultsPerBaby;

		// Aplicar bonus de upgrades (propiedades cacheadas)
		int couples = adultCount / 2;
		int bonusBabies = couples * BonusBabiesPerCouple;

		int totalNewBabies = baseBabies + bonusBabies;

		// Aplicar TwinChance a cada bebé
		float twinChance = TwinChance;
		int extraTwins = 0;
		for (int i = 0; i < totalNewBabies; i++)
		{
			if (GD.Randf() < twinChance)
			{
				extraTwins++;
			}
		}
		totalNewBabies += extraTwins;

		if (totalNewBabies <= 0)
		{
			GD.Print($"[BunniesManager] Reproduction: {adultCount} adults, no new bunnies");
			return;
		}

		// Spawn en la casa
		Vector2 spawnPos = Utils.Instance.BunnyHouse?.GlobalPosition ?? Vector2.Zero;

		for (int i = 0; i < totalNewBabies; i++)
		{
			SpawnBunny(spawnPos);
		}

		GD.Print($"[BunniesManager] Reproduction: {adultCount} adults → {totalNewBabies} babies (base: {baseBabies}, bonus: {bonusBabies}, twins: {extraTwins})");
		EmitSignal(SignalName.BunniesReproduced, totalNewBabies, BunnyCount);
	}
}
