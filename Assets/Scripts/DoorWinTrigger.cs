using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorWinTrigger : MonoBehaviour
{
    public string winSceneName = "WinScreen";
    public ItemType requiredKey = ItemType.EscapeKey;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger when the Player touches
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null || !GameManager.Instance.HasItem(requiredKey))
            return;

        // Load the win screen scene
        SceneManager.LoadScene(winSceneName);
    }
}
