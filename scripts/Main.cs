using Godot;

/// <summary>
/// Script de la escena principal.
/// Registra nodos globales en los autoloads.
/// </summary>
public partial class Main : Node2D
{
	public override void _Ready()
	{
		// Registrar SkyLight en ShaderManager
		var skyLight = GetNodeOrNull<CanvasModulate>("SkyLight");
		if (skyLight != null)
		{
			ShaderManager.Instance.RegisterSkyLight(skyLight);
		}

		// Iniciar el día
		GameManager.Instance.StartDay();
	}
}
