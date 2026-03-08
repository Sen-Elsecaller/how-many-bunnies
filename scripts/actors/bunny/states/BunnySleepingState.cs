using Godot;

/// <summary>
/// Estado de dormir. El conejo está en la casa, inactivo.
/// Espera signal DayStarted para salir.
/// </summary>
public partial class BunnySleepingState : BunnyState
{
	public override void Enter()
	{
		// Animación idle o sleep si existe
		if (Animations != null)
		{
			if (Animations.SpriteFrames.HasAnimation("sleep"))
			{
				Animations.Play("sleep");
			}
			else if (Animations.SpriteFrames.HasAnimation("idle"))
			{
				Animations.Play("idle");
			}
		}

		// Ocultar sprite (la casa lo tapa visualmente de todos modos)
		if (Animations != null)
		{
			Animations.Visible = false;
		}
	}

	public override void Exit()
	{
		// Mostrar sprite al salir
		if (Animations != null)
		{
			Animations.Visible = true;
		}
	}

	// No hace nada en Update - espera signal externa para despertar
}
