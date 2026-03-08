using Godot;

public partial class MainMenu : CanvasLayer
{
	public override void _Ready()
	{
		GetNode<Button>("%PlayButton").Pressed += OnPlayPressed;
		AudioManager.Instance?.PlayMenuMusic();
		SetupCredits();
	}

	private void SetupCredits()
	{
		var credits = GetNodeOrNull<RichTextLabel>("%CreditsText");
		if (credits == null) return;

		credits.BbcodeEnabled = true;
		credits.Text = "[center]" +
			"[font_size=18][b]Free Game Assets[/b][/font_size]\nBackgrounds\n\n" +
			"[font_size=18][b]Kenney[/b][/font_size]\nCursor\n\n" +
			"[font_size=18][b]seethingswarm[/b][/font_size]\nBunnies\n\n" +
			"[font_size=18][b]ObsydianX[/b][/font_size]\nSFX\n\n" +
			"[font_size=18][b]sanctumpixel[/b][/font_size]\nCarrots\n\n" +
			"[font_size=18][b]Pizza Doggy[/b][/font_size]\nMusic" +
			"[/center]";
	}

	private void OnPlayPressed()
	{
		// Fade out música del menú, luego iniciar juego con música random
		AudioManager.Instance?.FadeOutMusic(() =>
		{
			GameManager.Instance.StartGame();
			AudioManager.Instance?.StartMusicLoop();
		});
	}
}
