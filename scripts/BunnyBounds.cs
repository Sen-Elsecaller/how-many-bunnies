using Godot;

/// <summary>
/// Define el área donde los conejos pueden moverse.
/// Dibujar el polígono directamente en el editor.
/// </summary>
public partial class BunnyBounds : Polygon2D
{
	public override void _Ready()
	{
		// Convertir puntos locales a globales
		var localPoints = Polygon;
		var globalPoints = new Vector2[localPoints.Length];

		for (int i = 0; i < localPoints.Length; i++)
		{
			globalPoints[i] = ToGlobal(localPoints[i]);
		}

		Utils.Instance.BunnyBounds = globalPoints;

		GD.Print($"[BunnyBounds] Registered polygon with {globalPoints.Length} points");
	}
}
