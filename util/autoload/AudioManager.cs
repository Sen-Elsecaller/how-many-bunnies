using Godot;
using Godot.Collections;
using HowManyBunnies;
using System;
using System.Collections.Generic;

/// <summary>
/// Autoload para reproducir efectos de sonido y música de fondo.
/// SFX: AudioManager.Instance.CreateAudio(SoundEffectType.ButtonClick);
/// BGM: AudioManager.Instance.StartMusicLoop();
///
/// Auto-carga SoundEffect .tres desde SfxFolder y AudioStream desde OstFolder.
/// </summary>
public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	[ExportGroup("SFX")]
	[Export] public string SfxFolder { get; set; } = "res://resources/sfx/";

	[ExportGroup("BGM")]
	[Export] public string OstFolder { get; set; } = "res://assets/ost/";
	[Export] public string MenuTrackName { get; set; } = "Forgotten Biomes.wav";
	[Export] public float DelayMin { get; set; } = 10f;
	[Export] public float DelayMax { get; set; } = 30f;
	[Export] public float FadeDuration { get; set; } = 2f;
	[Export] public float BgmVolumeDb { get; set; } = -10f;

	private System.Collections.Generic.Dictionary<SoundEffectType, SoundEffect> _soundDict = new();

	// BGM
	private AudioStreamPlayer _bgmPlayer;
	private AudioStream _menuTrack;
	private List<AudioStream> _gameTracks = new();
	private int _lastTrackIndex = -1;
	private bool _musicLoopActive = false;
	private Tween _bgmTween;

	public override void _Ready()
	{
		Instance = this;
		LoadSoundEffectsFromFolder();
		LoadTracksFromFolder();
		SetupBgmPlayer();
	}

	private void LoadSoundEffectsFromFolder()
	{
		_soundDict.Clear();

		var dir = DirAccess.Open(SfxFolder);
		if (dir == null)
		{
			GD.PushWarning($"[AudioManager] Carpeta no encontrada: {SfxFolder}");
			return;
		}

		dir.ListDirBegin();
		var fileName = dir.GetNext();

		while (!string.IsNullOrEmpty(fileName))
		{
			if (!dir.CurrentIsDir() && fileName.EndsWith(".tres"))
			{
				var path = SfxFolder + fileName;
				var resource = GD.Load<Resource>(path);

				if (resource is SoundEffect sfx && sfx.Type != SoundEffectType.None)
				{
					if (_soundDict.ContainsKey(sfx.Type))
					{
						GD.PushWarning($"[AudioManager] Tipo duplicado: {sfx.Type}");
					}
					else
					{
						_soundDict[sfx.Type] = sfx;
					}
				}
			}
			fileName = dir.GetNext();
		}

		dir.ListDirEnd();
		GD.Print($"[AudioManager] {_soundDict.Count} efectos cargados desde {SfxFolder}");
	}

	/// <summary>Reproduce sonido global (no posicional)</summary>
	public void CreateAudio(SoundEffectType type)
	{
		if (!TryGetSoundEffect(type, out var soundEffect)) return;
		if (!soundEffect.CanPlay()) return;

		soundEffect.IncrementCount();

		var player = new AudioStreamPlayer();
		AddChild(player);
		ConfigurePlayer(player, soundEffect);
		player.Finished += () => OnAudioFinished(player, soundEffect);
		player.Play();
	}

	/// <summary>Reproduce sonido en posición 2D</summary>
	public void Create2DAudioAt(Vector2 position, SoundEffectType type)
	{
		if (!TryGetSoundEffect(type, out var soundEffect)) return;
		if (!soundEffect.CanPlay()) return;

		soundEffect.IncrementCount();

		var player = new AudioStreamPlayer2D();
		AddChild(player);
		player.Position = position;
		ConfigurePlayer(player, soundEffect);
		player.Finished += () => OnAudioFinished(player, soundEffect);
		player.Play();
	}

	private bool TryGetSoundEffect(SoundEffectType type, out SoundEffect soundEffect)
	{
		if (_soundDict.TryGetValue(type, out soundEffect))
		{
			return true;
		}
		GD.PushError($"[AudioManager] Tipo no encontrado: {type}");
		return false;
	}

	private void ConfigurePlayer(AudioStreamPlayer player, SoundEffect soundEffect)
	{
		player.Stream = soundEffect.Stream;
		player.VolumeDb = soundEffect.VolumeDb;
		player.PitchScale = soundEffect.GetRandomizedPitch();
	}

	private void ConfigurePlayer(AudioStreamPlayer2D player, SoundEffect soundEffect)
	{
		player.Stream = soundEffect.Stream;
		player.VolumeDb = soundEffect.VolumeDb;
		player.PitchScale = soundEffect.GetRandomizedPitch();
	}

	private void OnAudioFinished(Node player, SoundEffect soundEffect)
	{
		soundEffect.DecrementCount();
		player.QueueFree();
	}

	#region BGM

	private void SetupBgmPlayer()
	{
		_bgmPlayer = new AudioStreamPlayer();
		_bgmPlayer.VolumeDb = BgmVolumeDb;
		_bgmPlayer.Finished += OnTrackFinished;
		AddChild(_bgmPlayer);
	}

	private void LoadTracksFromFolder()
	{
		_gameTracks.Clear();
		_menuTrack = null;

		var dir = DirAccess.Open(OstFolder);
		if (dir == null)
		{
			GD.PushWarning($"[AudioManager] OST folder not found: {OstFolder}");
			return;
		}

		dir.ListDirBegin();
		var fileName = dir.GetNext();

		while (!string.IsNullOrEmpty(fileName))
		{
			if (!dir.CurrentIsDir() && fileName.EndsWith(".wav"))
			{
				var path = OstFolder + fileName;
				var stream = GD.Load<AudioStream>(path);
				if (stream != null)
				{
					if (fileName == MenuTrackName)
						_menuTrack = stream;
					else
						_gameTracks.Add(stream);
				}
			}
			fileName = dir.GetNext();
		}

		dir.ListDirEnd();
		GD.Print($"[AudioManager] Menu track: {(_menuTrack != null ? "OK" : "NOT FOUND")}, Game tracks: {_gameTracks.Count}");
	}

	/// <summary>Inicia el ciclo de música (primera canción inmediata)</summary>
	public void StartMusicLoop()
	{
		if (_musicLoopActive && _bgmPlayer.Playing) return; // Ya está activo

		_musicLoopActive = true;
		PlayRandomTrack();
	}

	/// <summary>Detiene el ciclo de música</summary>
	public void StopMusicLoop()
	{
		_musicLoopActive = false;
		_bgmTween?.Kill();
		_bgmPlayer.Stop();
	}

	/// <summary>Fade out y ejecuta callback al terminar</summary>
	public void FadeOutMusic(Action onComplete = null)
	{
		if (!_bgmPlayer.Playing)
		{
			onComplete?.Invoke();
			return;
		}

		_bgmTween?.Kill();
		_bgmTween = CreateTween();
		_bgmTween.TweenProperty(_bgmPlayer, "volume_db", -40f, FadeDuration);
		_bgmTween.TweenCallback(Callable.From(() =>
		{
			_bgmPlayer.Stop();
			_bgmPlayer.VolumeDb = BgmVolumeDb;
			onComplete?.Invoke();
		}));
	}

	/// <summary>Reproduce la música del menú (loop)</summary>
	public void PlayMenuMusic()
	{
		if (_menuTrack == null) return;
		if (_bgmPlayer.Playing && _bgmPlayer.Stream == _menuTrack) return;

		_musicLoopActive = false; // No usar el ciclo random
		_bgmPlayer.Stream = _menuTrack;
		_bgmPlayer.VolumeDb = BgmVolumeDb;
		_bgmPlayer.Play();
	}

	/// <summary>Reproduce track aleatorio de game tracks (evita repetir)</summary>
	private void PlayRandomTrack()
	{
		if (_gameTracks.Count == 0) return;

		int index;
		if (_gameTracks.Count == 1)
		{
			index = 0;
		}
		else
		{
			do
			{
				index = GD.RandRange(0, _gameTracks.Count - 1);
			} while (index == _lastTrackIndex);
		}

		_lastTrackIndex = index;
		_bgmPlayer.Stream = _gameTracks[index];
		_bgmPlayer.VolumeDb = BgmVolumeDb;
		_bgmPlayer.Play();
	}

	private void OnTrackFinished()
	{
		// Si es música de menú, repetir
		if (!_musicLoopActive && _bgmPlayer.Stream == _menuTrack)
		{
			_bgmPlayer.Play();
			return;
		}

		if (!_musicLoopActive) return;

		// Esperar delay aleatorio antes de siguiente track
		float delay = (float)GD.RandRange(DelayMin, DelayMax);
		GetTree().CreateTimer(delay).Timeout += PlayRandomTrack;
	}

	#endregion
}
