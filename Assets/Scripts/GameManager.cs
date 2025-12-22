using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // items needed for full score
    public int itemsNeededForLevel = 5;
    public Text scoreText;
    public Text itemsText;

    // items we have
    private HashSet<ItemType> ownedItems = new HashSet<ItemType>();

    // how many items we picked up in total 
    private int itemsCollected = 0;
   
    public GameObject pauseMenuUI;   
    private bool isPaused = false;
    private CursorLockMode prePauseLockState = CursorLockMode.None;
    private bool prePauseCursorVisible = true;
    public static bool IsPaused => Instance != null && Instance.isPaused;

    //Settings UI
    public GameObject settingsMenuUI;
    public Slider volumeSlider;
    public Slider brightnessSlider;

    //Lighting
    public Light mainLight;
    public float minLightIntensity = 0.2f;
    public float maxLightIntensity = 2.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        UpdateInventoryUI();

        // Volume slider
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        // Brightness slider
        if (brightnessSlider != null && mainLight != null)
        {
            float t = Mathf.InverseLerp(minLightIntensity, maxLightIntensity, mainLight.intensity);
            brightnessSlider.value = t;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
            
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    //inventory
    public void RegisterItemPickup(ItemType itemType)
{
    ownedItems.Add(itemType);
    itemsCollected++;

    UpdateInventoryUI();

    FindObjectOfType<InventoryUI>()?.Refresh(); 
}


    public bool HasItem(ItemType itemType)
    {
        return ownedItems.Contains(itemType);
    }

    public float GetScorePercent()
    {
        if (itemsNeededForLevel <= 0) return 0f;
        return (itemsCollected / (float)itemsNeededForLevel) * 100f;
    }

    public int GetItemsCollected()
    {
        return itemsCollected;
    }

    public int GetItemsNeeded()
    {
        return itemsNeededForLevel;
    }

    private void UpdateInventoryUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {itemsCollected} / {itemsNeededForLevel}";
        }

        if (itemsText != null)
        {
            itemsText.text = $"Items: {itemsCollected}";
        }
    }
 
    // pause menu 
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (!isPaused && settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(isPaused);

        if (isPaused)
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

        AudioListener.pause = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        // Handle cursor visibility and lock state
        if (isPaused)
        {
            // Show and unlock cursor when paused
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Hide and lock cursor when resuming
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        TogglePause();
    }

    public void StartGame(string levelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
    }

    public void SetBrightness(float value01)
    {
        if (mainLight == null) return;
        mainLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, Mathf.Clamp01(value01));
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
