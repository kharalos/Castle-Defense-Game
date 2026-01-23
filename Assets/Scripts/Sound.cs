using UnityEngine;

[System.Serializable]
public class Sound 
{
    public ClipType clipType;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public float time;

    public bool loop;


    [HideInInspector]
    public AudioSource source;
}

public enum ClipType
{
    ArrowFired = 0,
    ArrowPierces = 1,
    ArrowHit = 2,
    EnemyDamaged = 3,
    GiantDeath = 4,
    MinionDeath = 5,
    CastleHit = 6,
    BuySound = 7,
    HealthRestored = 8,
    JumpDrop = 9,
    CoinAcquired = 10,
    Explosion = 11,
    ThemeMusic = 12,
    HeroSlashes = 13,
    HeroSwings = 14,
    HeroHastened = 15,
}
