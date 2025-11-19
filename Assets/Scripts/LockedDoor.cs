using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public ItemType requiredKey = ItemType.EscapeKey;

    public GameObject needKeyUI;    
    public GameObject openUI;      
    public Animator doorAnimator;   //for now no animation
    public string openTriggerName = "Open";

    private bool playerInRange = false;
    private bool isOpen = false;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        if (needKeyUI != null) needKeyUI.SetActive(false);
        if (openUI != null) openUI.SetActive(false);
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
        if (needKeyUI != null) needKeyUI.SetActive(false);
        if (openUI != null) openUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || isOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpen();
        }
    }

    private void UpdatePrompt()
    {
        bool hasKey = GameManager.Instance.HasItem(requiredKey);

        if (hasKey)
        {
            if (openUI != null) openUI.SetActive(true);
            if (needKeyUI != null) needKeyUI.SetActive(false);
        }
        else
        {
            if (needKeyUI != null) needKeyUI.SetActive(true);
            if (openUI != null) openUI.SetActive(false);
        }
    }

    private void TryOpen()
    {
        if (!GameManager.Instance.HasItem(requiredKey))
        {
            UpdatePrompt();
            return;
        }

        isOpen = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            GetComponent<Collider>().enabled = false;
        }

        if (openUI != null) openUI.SetActive(false);
        if (needKeyUI != null) needKeyUI.SetActive(false);
    }
}
