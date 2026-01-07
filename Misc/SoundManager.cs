using UnityEngine;

// launch a sound effect like this;
// SoundManager.Instance.PlaySound(SoundManager.Instance.jump);

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sounds")]
    public AudioClip attack;
    public AudioClip enemyDeath;
    public AudioClip fireball;
    public AudioClip fireballImpact;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip reachedGoal;
    public AudioClip playerDeath;
    public AudioClip playerDeathGrunt;
    public AudioClip playerDeathHit;
    public AudioClip preTeleport;
    public AudioClip teleport;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of SoundManager detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
        }
        else audioSource.PlayOneShot(clip);
    }
}
