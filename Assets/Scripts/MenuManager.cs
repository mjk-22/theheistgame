using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public GameObject controlsUI;

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (GameManager.Instance != null )
        {
            Destroy(GameManager.Instance);
        }
        
        // Hide controls canvas on start
        if (controlsUI != null)
        {
            controlsUI.SetActive(false);
        }
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void StartTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TutorialLevel");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowControls()
    {
        if (controlsUI != null)
        {
            controlsUI.SetActive(true);
        }
    }

    public void HideControls()
    {
        if (controlsUI != null)
        {
            controlsUI.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();

    }
    public void GameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoseScreen");
    }
}
