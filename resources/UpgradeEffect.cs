namespace HowManyBunnies;

public enum UpgradeEffectType
{
    /// <summary>Para upgrades que solo desbloquean otros (sin efecto directo)</summary>
    None,

    /// <summary>Probabilidad (0-1) de duplicar crías en reproducción</summary>
    TwinChance,

    /// <summary>Crías adicionales por pareja por noche (flat)</summary>
    BabiesPerCouple,

    /// <summary>Multiplicador de velocidad de conejos (1.0 = base)</summary>
    BunnySpeed,
}
