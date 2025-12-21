using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;

    [Header("Items to show in UI (all possible items)")]
    public List<InventoryItemData> items;

    // Keep spawned slots so we can update them
    private Dictionary<ItemType, InventorySlotUI> slotByType = new();

    private void Start()
    {
        BuildGrid();
        Refresh();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        // Optional: open/close with I key
        if (Input.GetKeyDown(KeyCode.I))
            Toggle();
    }

    public void Toggle()
    {
        if (inventoryPanel == null) return;
        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);

        if (newState) Refresh(); // update whenever opened
    }

    private void BuildGrid()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        slotByType.Clear();

        foreach (var item in items)
        {
            var slotObj = Instantiate(slotPrefab, gridParent);
            var slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUI.Setup(item);
            slotByType[item.type] = slotUI;
        }
    }

    public void Refresh()
    {
        if (GameManager.Instance == null) return;

        foreach (var kvp in slotByType)
        {
            ItemType type = kvp.Key;
            bool owned = GameManager.Instance.HasItem(type);
            kvp.Value.SetOwned(owned);
        }
    }
}
