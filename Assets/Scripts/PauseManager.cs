using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    public GameObject pauseMenuUI;    
    public GameObject settingsMenuUI;  

    public Slider volumeSlider;
    public Slider brightnessSlider;

    public Light mainLight;
    public float minLightIntensity = 0.2f;
    public float maxLightIntensity = 2.0f;

    // private bool isPaused = false;
    private CursorLockMode prePauseLockState = CursorLockMode.None;
    private bool prePauseCursorVisible = true;

    private void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsMenuUI != null) settingsMenuUI.SetActive(false);
         // volume slider
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(AudioListener.volume);
            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        // brightness slider
        if (brightnessSlider != null && mainLight != null)
        {
            float t = Mathf.InverseLerp(minLightIntensity, maxLightIntensity, mainLight.intensity);
            brightnessSlider.SetValueWithoutNotify(t);
            brightnessSlider.onValueChanged.AddListener(SetBrightness01);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    private void SetPaused(bool paused)
    {
        IsPaused = paused;

        if (!IsPaused && settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(IsPaused);

        if (IsPaused)
        {
            prePauseLockState = Cursor.lockState;
            prePauseCursorVisible = Cursor.visible;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = prePauseLockState;
            Cursor.visible = prePauseCursorVisible;
        }

        AudioListener.pause = IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }


    public void OpenSettings()
    {
        if (settingsMenuUI != null) settingsMenuUI.SetActive(true);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsMenuUI != null) settingsMenuUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
    }

    public void SetBrightness01(float value01)
    {
        if (mainLight == null) return;
        float v = Mathf.Clamp01(value01);
        mainLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, v);
    }
      public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
