using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the player's score on the win screen.
/// Attach this script to a GameObject in the WinScreen scene.
/// </summary>
public class WinScreenManager : MonoBehaviour
{
    public Text scoreText;

    private void Start()
    {
        // Make sure cursor is visible and unlocked on win screen
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Update the score display
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("WinScreenManager: scoreText is not assigned!");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("WinScreenManager: GameManager.Instance is null!");
            scoreText.text = "Score: N/A";
            return;
        }

        // Get score data from GameManager
        int itemsCollected = GameManager.Instance.GetItemsCollected();
        int itemsNeeded = GameManager.Instance.GetItemsNeeded();
        float scorePercent = GameManager.Instance.GetScorePercent();

        scoreText.text = $"Score: {scorePercent:F0}";
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

