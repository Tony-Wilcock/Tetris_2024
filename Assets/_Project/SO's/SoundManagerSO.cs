using UnityEngine;

[CreateAssetMenu(fileName = "SoundManagerSO", menuName = "Scriptable Objects/SoundManagerSO")]
public class SoundManagerSO : ScriptableObject
{
    public bool musicEnabled = true, sfxEnabled = true;

    [SerializeField] [Range(0, 1)] public float musicVolume = 1.0f, sfxVolume = 1.0f;

    [field: SerializeField] public AudioClip ClearRowSound { get; private set; }
    [field: SerializeField] public AudioClip MoveSound { get; private set; }
    [field: SerializeField] public AudioClip MoveErrorSound { get; private set; }
    [field: SerializeField] public AudioClip RotateSound { get; private set; }
    [field: SerializeField] public AudioClip DropSound { get; private set; }
    [field: SerializeField] public AudioClip LandSound { get; private set; }
    [field: SerializeField] public AudioClip HoldSound { get; private set; }
    [field: SerializeField] public AudioClip GameOverSound { get; private set; }
    [field: SerializeField] public AudioClip[] VocalSounds { get; private set; }
    [field: SerializeField] public AudioClip GameOverVocalSound { get; private set; }
    [field: SerializeField] public AudioClip LevelUpVocalSound { get; private set; }

    [SerializeField] public AudioSource musicSource;
    //public AudioSource sfxSource;

    [SerializeField] public AudioClip[] musicClips;
}
