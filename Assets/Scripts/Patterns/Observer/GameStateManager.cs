using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private void Awake()
    {
        if (winPanel  != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEventBus.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEventBus.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(bool playerWon)
    {
        StartCoroutine(GameOverRoutine(playerWon));
    }

    private System.Collections.IEnumerator GameOverRoutine(bool playerWon)
    {
        yield return new WaitForSeconds(4f);

        Time.timeScale = 0f;

        if (playerWon)
        {
            if (winPanel  != null) winPanel.SetActive(true);
        }
        else
        {
            if (losePanel != null) losePanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
