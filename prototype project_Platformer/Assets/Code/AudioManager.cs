using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip collectSmallSound;
    public AudioClip collectBigSound;
    public AudioClip deathSound;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}