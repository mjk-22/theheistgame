using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemType itemType;
    public KeyCode interactKey = KeyCode.E;
    //public GameObject promptUI; 

    private bool playerInRange = false;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        //if (promptUI != null) promptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        //if (promptUI != null) promptUI.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        //if (promptUI != null) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            GameManager.Instance.RegisterItemPickup(itemType);

            //if (promptUI != null) promptUI.SetActive(false);

            Destroy(gameObject);
        }
    }
}
