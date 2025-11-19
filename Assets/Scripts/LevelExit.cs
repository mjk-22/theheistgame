using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LevelExit : MonoBehaviour
{
    public ItemType requiredKey = ItemType.EscapeKey;

    public string nextSceneName;    // name of next level or "MainMenu"

    public GameObject interactUI;   // "Press E to leave"
    public GameObject needKeyUI;    // "You need the escape key"

    private bool playerInRange = false;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        if (interactUI != null) interactUI.SetActive(false);
        if (needKeyUI != null) needKeyUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        UpdatePrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (interactUI != null) interactUI.SetActive(false);
        if (needKeyUI != null) needKeyUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryFinishLevel();
        }
    }

    private void UpdatePrompt()
    {
        bool hasKey = GameManager.Instance.HasItem(requiredKey);

        if (hasKey)
        {
            if (interactUI != null) interactUI.SetActive(true);
            if (needKeyUI != null) needKeyUI.SetActive(false);
        }
        else
        {
            if (needKeyUI != null) needKeyUI.SetActive(true);
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    private void TryFinishLevel()
    {
        if (!GameManager.Instance.HasItem(requiredKey))
        {
            UpdatePrompt();
            return;
        }

        float percent = GameManager.Instance.GetScorePercent();
        Debug.Log($"Level complete! Score: {percent:0}%");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("LevelExit: nextSceneName is empty.");
        }
    }
}
