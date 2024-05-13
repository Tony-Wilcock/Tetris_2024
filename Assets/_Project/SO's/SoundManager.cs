using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] SoundManagerSO soundManagerSO;
    private void Start()
    {
        if (!soundManagerSO.musicSource) soundManagerSO.musicSource = FindFirstObjectByType<AudioSource>();

        // soundManagerSO.musicEnabled = true;
        // soundManagerSO.sfxEnabled = true;

        PlayBackgroundMusic(GetRandomClip(soundManagerSO.musicClips));
    }

    public void AlterVolume(float volume, bool paused = false)
    {
        soundManagerSO.musicSource.volume = paused ? soundManagerSO.musicVolume * volume : soundManagerSO.musicVolume;
    }

    public void PlaySfxClip(AudioClip sfxClip, float volumeMultiplier = 1f, Vector3 pos = new Vector3())
    {
        if (soundManagerSO.sfxEnabled && sfxClip) AudioSource.PlayClipAtPoint(sfxClip, pos, Mathf.Clamp(soundManagerSO.sfxVolume * volumeMultiplier, 0.0f, 1f));
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        return randomClip;
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (!soundManagerSO.musicEnabled || !musicClip || !soundManagerSO.musicSource) return;

        soundManagerSO.musicSource.Stop();
        soundManagerSO.musicSource.clip = musicClip;
        soundManagerSO.musicSource.volume = soundManagerSO.musicVolume;
        soundManagerSO.musicSource.loop = true;
        soundManagerSO.musicSource.Play();
    }

    public void UpdateMusic()
    {
        if (soundManagerSO.musicSource.isPlaying != soundManagerSO.musicEnabled)
        {
            if (soundManagerSO.musicEnabled)
            {
                PlayBackgroundMusic(GetRandomClip(soundManagerSO.musicClips));
            }
            else
            {
                soundManagerSO.musicSource.Stop();
            }
        }
    }

    public void ToggleMusic()
    {
        soundManagerSO.musicEnabled = !soundManagerSO.musicEnabled;
        UpdateMusic();
    }

    public void ToggleSFX()
    {
        soundManagerSO.sfxEnabled = !soundManagerSO.sfxEnabled;
    }

    public bool IsMusicEnabled()
    {
        return soundManagerSO.musicEnabled;
    }

    public bool IsSFXEnabled()
    {
        return soundManagerSO.sfxEnabled;
    }
}
