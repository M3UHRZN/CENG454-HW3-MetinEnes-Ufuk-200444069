using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip coreDamagedClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameEventBus.OnEnemyDied   += HandleEnemyDied;
        GameEventBus.OnCoreDamaged += HandleCoreDamaged;
    }

    private void OnDisable()
    {
        GameEventBus.OnEnemyDied   -= HandleEnemyDied;
        GameEventBus.OnCoreDamaged -= HandleCoreDamaged;
    }

    private void HandleEnemyDied(int killCount)
    {
        if (enemyDeathClip != null)
            audioSource.PlayOneShot(enemyDeathClip);
    }

    private void HandleCoreDamaged(float current, float max)
    {
        if (coreDamagedClip != null)
            audioSource.PlayOneShot(coreDamagedClip);
    }
}
