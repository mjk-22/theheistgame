using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public ItemType keyType = ItemType.EscapeKey;
    public KeyCode interactKey = KeyCode.E;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (!Input.GetKeyDown(interactKey)) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterItemPickup(keyType);
            Debug.Log("Picked up key: " + keyType);
        }

        Destroy(gameObject);
    }
}
