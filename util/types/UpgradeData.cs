using Godot;
using HowManyBunnies;

[GlobalClass]
public partial class UpgradeData : Resource
{
    [Export] public string Id { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; set; }
    [Export] public int Cost { get; set; }
    [Export] public Texture2D Icon { get; set; }
    [Export] public UpgradeEffectType EffectType { get; set; } = UpgradeEffectType.None;
    [Export] public float EffectValue { get; set; }
}
