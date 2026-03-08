using Godot;
using HowManyBunnies;

/// <summary>
/// Aguila que desciende y roba un conejo durante el sunset.
/// Estructura esperada:
///   Eagle (Node2D)
///   ├── Sprite2D (placeholder negro)
///   └── HitArea (Area2D) - para futura colisión con zanahorias
/// </summary>
public partial class Eagle : Node2D
{
	[Export] public float SwoopDuration { get; set; } = 1.2f;
	[Export] public float EscapeDuration { get; set; } = 0.8f;
	[Export] public Vector2 EntryOffset { get; set; } = new Vector2(400, -300);

	private Bunny _target;
	private Tween _tween;
	private Vector2 _targetPos;

	/// <summary>
	/// Lanza el águila hacia el conejo objetivo.
	/// </summary>
	public void Launch(Bunny target)
	{
		if (target == null)
		{
			QueueFree();
			return;
		}

		_target = target;
		_targetPos = target.GlobalPosition;

		// Posicionar fuera de pantalla (arriba-derecha del target)
		GlobalPosition = _targetPos + EntryOffset;

		// Tween de entrada (swoop hacia el conejo)
		_tween = CreateTween();
		_tween.TweenProperty(this, "global_position", _targetPos, SwoopDuration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);
		_tween.TweenCallback(Callable.From(GrabTarget));
	}

	private void GrabTarget()
	{
		if (_target == null || !IsInstanceValid(_target))
		{
			FlyAway();
			return;
		}

		// Sonido
		AudioManager.Instance?.CreateAudio(SoundEffectType.EagleTakingBunny);

		// Reparentar conejo al águila (lo "agarra")
		var bunnyGlobalPos = _target.GlobalPosition;
		_target.GetParent()?.RemoveChild(_target);
		AddChild(_target);
		_target.Position = Vector2.Zero;

		// Desactivar comportamiento del conejo
		_target.SetProcess(false);
		_target.SetPhysicsProcess(false);

		FlyAway();
	}

	private void FlyAway()
	{
		// Volar hacia arriba-izquierda
		Vector2 exitPos = GlobalPosition + new Vector2(-500, -400);

		_tween = CreateTween();
		_tween.TweenProperty(this, "global_position", exitPos, EscapeDuration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);
		_tween.TweenCallback(Callable.From(Cleanup));
	}

	private void Cleanup()
	{
		if (_target != null && IsInstanceValid(_target))
		{
			BunniesManager.Instance?.RemoveBunny(_target);
		}
		QueueFree();
	}
}
