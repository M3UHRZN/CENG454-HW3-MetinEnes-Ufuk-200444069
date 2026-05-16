using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip coreDamagedClip;
    [SerializeField] private AudioClip playerFireClip;
    [SerializeField] private AudioClip enemyFireClip;

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
        GameEventBus.OnEnemyDied    += HandleEnemyDied;
        GameEventBus.OnCoreDamaged  += HandleCoreDamaged;
        GameEventBus.OnWeaponFired  += HandleWeaponFired;
    }

    private void OnDisable()
    {
        GameEventBus.OnEnemyDied    -= HandleEnemyDied;
        GameEventBus.OnCoreDamaged  -= HandleCoreDamaged;
        GameEventBus.OnWeaponFired  -= HandleWeaponFired;
    }

    private void HandleWeaponFired(string shooterName)
    {
        AudioClip clip = shooterName == "Player" ? playerFireClip : enemyFireClip;
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void HandleEnemyDied(int killCount, string killerName)
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
