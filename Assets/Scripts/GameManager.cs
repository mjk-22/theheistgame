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

    }
    public void GameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoseScreen");
    }
}
