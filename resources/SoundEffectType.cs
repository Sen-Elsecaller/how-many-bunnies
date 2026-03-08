namespace HowManyBunnies;

/// <summary>
/// Tipos de efectos de sonido disponibles.
/// Agregar nuevos tipos aquí y crear el SoundEffect resource correspondiente.
/// </summary>
public enum SoundEffectType
{
	None,

	// UI
	ButtonClick,
	ButtonBack,
	ButtonHover,
	UpgradePurchase,
	UpgradePurchase2,
	UpgradePurchase3,

	// Gameplay
	CarrotPick,
	CarrotThrow,
	CarrotLand,
	BunnyEat,
	BunnySqueak,
	EagleTakingBunny,

	// Ambiente
	DayStart,
	NightStart,
}
