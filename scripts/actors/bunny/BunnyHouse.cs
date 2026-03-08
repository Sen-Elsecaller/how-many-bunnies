using Godot;

/// <summary>
/// Casa de los conejos. Se registra en Utils para que los conejos sepan dónde ir a dormir.
/// El Sprite2D debe tener z_index alto para dibujar por encima de los conejos.
/// </summary>
public partial class BunnyHouse : Node2D
{
	public override void _Ready()
	{
		Utils.Instance.BunnyHouse = this;
	}
}
