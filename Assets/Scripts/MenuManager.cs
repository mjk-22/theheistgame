using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (GameManager.Instance != null )
        {
            Destroy(GameManager.Instance);
        }
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
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
