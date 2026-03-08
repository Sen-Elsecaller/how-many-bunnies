using Godot;
using HowManyBunnies;

[GlobalClass]
public partial class AnimatedButton : Button
{
	[Export] public SoundEffectType ClickSound { get; set; } = SoundEffectType.ButtonClick;

	private Tween _tween;

	public override void _Ready()
	{
		FocusMode = FocusModeEnum.None;
		Resized += UpdatePivot;
		UpdatePivot();

		ButtonDown += OnButtonDown;
		ButtonUp += OnButtonUp;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	public override void _ExitTree()
	{
		Resized -= UpdatePivot;
		ButtonDown -= OnButtonDown;
		ButtonUp -= OnButtonUp;
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
	}

	private void UpdatePivot()
	{
		PivotOffset = Size * new Vector2(0.5f, 1f);
	}

	private void OnMouseEntered()
	{
		_tween?.Kill();
		Scale = new Vector2(1.04f, 1.04f);
		Utils.Instance?.SetPointingCursor();
	}

	private void OnMouseExited()
	{
		_tween?.Kill();
		Scale = Vector2.One;
		Utils.Instance?.SetDefaultCursor();
	}

	private void OnButtonDown()
	{
		_tween?.Kill();
		Scale = new Vector2(0.95f, 0.875f);
		AudioManager.Instance?.CreateAudio(ClickSound);
	}

	private void OnButtonUp()
	{
		_tween = CreateTween().SetEase(Tween.EaseType.Out);
		_tween.SetTrans(Tween.TransitionType.Elastic);
		_tween.TweenProperty(this, "scale", Vector2.One, 0.25f);
	}
}
