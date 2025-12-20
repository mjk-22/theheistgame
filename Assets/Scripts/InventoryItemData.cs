using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class InventoryItemData : ScriptableObject
{
    public ItemType type;
    public string displayName;
    public Sprite icon;
    [TextArea] public string notCollectedText = "Not stolen yet";
}
