using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Text scoreText;
    private int score = 0;
    
    [Header("Respawn Settings")]
    public Transform playerSpawnPoint; // Assign in inspector, or it will use player's starting position
    private Vector3 defaultSpawnPosition;
    private Quaternion defaultSpawnRotation;
    private GameObject playerObject;

    private void Awake()
    {
        // Enforce one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
    {
        score += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
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
}
