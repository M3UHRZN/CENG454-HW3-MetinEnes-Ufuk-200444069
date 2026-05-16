using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private Image coreHealthFill;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI killText;

    private void OnEnable()
    {
        GameEventBus.OnCoreDamaged   += HandleCoreDamaged;
        GameEventBus.OnEnemyDied     += HandleEnemyDied;
        GameEventBus.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        GameEventBus.OnCoreDamaged   -= HandleCoreDamaged;
        GameEventBus.OnEnemyDied     -= HandleEnemyDied;
        GameEventBus.OnWaveCompleted -= HandleWaveCompleted;
    }

    private void HandleCoreDamaged(float current, float max)
    {
        if (coreHealthFill != null)
            coreHealthFill.fillAmount = current / max;
    }

    private void HandleEnemyDied(int killCount, string killerName)
    {
        if (killText != null)
            if (killerName == "Player")
                killText.text = $"Kills: {killCount}";
    }

    private void HandleWaveCompleted(int waveIndex)
    {
        if (waveText != null)
            waveText.text = $"Wave: {waveIndex + 1}";
    }
}
