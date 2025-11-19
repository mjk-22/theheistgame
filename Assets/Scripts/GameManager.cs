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
    private int score = 0;
    
    [Header("Respawn Settings")]
    public Transform playerSpawnPoint; // Assign in inspector, or it will use player's starting position
    private Vector3 defaultSpawnPosition;
    private Quaternion defaultSpawnRotation;
    private GameObject playerObject;

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

    private void Start()
    {
        // Find player and store spawn position
        InitializeSpawnPosition();
    }

    private void InitializeSpawnPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerObject = player;
            if (playerSpawnPoint != null)
            {
                defaultSpawnPosition = playerSpawnPoint.position;
                defaultSpawnRotation = playerSpawnPoint.rotation;
            }
            else
            {
                // Use player's starting position as spawn point
                defaultSpawnPosition = player.transform.position;
                defaultSpawnRotation = player.transform.rotation;
            }
        }
    }

    public void AddScore(int amount)
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

    public void RespawnPlayer()
    {
        // Initialize spawn position if not set
        if (defaultSpawnPosition == Vector3.zero)
        {
            InitializeSpawnPosition();
        }

        // Find player if not cached
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerObject == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        // Update spawn position if spawn point is assigned
        if (playerSpawnPoint != null)
        {
            defaultSpawnPosition = playerSpawnPoint.position;
            defaultSpawnRotation = playerSpawnPoint.rotation;
        }

        // Reset player position and rotation
        playerObject.transform.position = defaultSpawnPosition;
        playerObject.transform.rotation = defaultSpawnRotation;

        // Reset player's rigidbody velocity if it has one
        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Reset CharacterController if it exists
        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            cc.enabled = true;
        }
    }
    // health hud modify it later si that the player die go to lose screen when caught by enemy
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
