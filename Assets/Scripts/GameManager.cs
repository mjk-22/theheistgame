using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // items needed for full score
    public int totalItemsInGame  = 12;
    public Text scoreText;
    public Text itemsText;
    

    // items we have
    private HashSet<ItemType> ownedItems = new HashSet<ItemType>();

    // how many items we picked up in total 
    private int itemsCollected = 0;
   
  

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
         SceneManager.sceneLoaded += OnSceneLoaded;
        
        UpdateInventoryUI();   
    }
     private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Update()
    {
      
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
        if (totalItemsInGame  <= 0) return 0f;
        return (itemsCollected / (float)totalItemsInGame ) * 100f;
    }

    public int GetItemsCollected()
    {
        return itemsCollected;
    }

    public int GetItemsNeeded()
    {
        return totalItemsInGame ;
    }

    private void UpdateInventoryUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {itemsCollected} / {totalItemsInGame }";
        }

        if (itemsText != null)
        {
            itemsText.text = $"Items: {itemsCollected}";
        }
    }
 

    public void StartGame(string levelName)
    {
        Time.timeScale = 1f;
         if (GameManager.Instance != null)
        GameManager.Instance.ResetRun();
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

     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
         UpdateInventoryUI();
         
    }

    public void SetHUD(Text newScoreText, Text newItemsText)
    {
        scoreText = newScoreText;
        itemsText = newItemsText;
        UpdateInventoryUI();
    }
    public void ResetRun()
{
    ownedItems.Clear();
    itemsCollected = 0;
    UpdateInventoryUI();
    FindObjectOfType<InventoryUI>()?.Refresh();
}

}
