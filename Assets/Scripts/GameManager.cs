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

    public Text healthText;
    public int maxHealth = 3;
    private int currentHealth;
   
    public GameObject pauseMenuUI;   
    private bool isPaused = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentHealth = maxHealth;
        UpdateHUD();
        UpdateInventoryUI();
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

    // health hud
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHUD();

        if (currentHealth <= 0)
        {
            ReloadCurrentScene();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }

    private void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
    // pause menu 
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;
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

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
