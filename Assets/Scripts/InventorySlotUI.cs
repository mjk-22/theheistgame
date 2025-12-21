using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public Text labelText;

    private InventoryItemData data;

    public void Setup(InventoryItemData itemData)
    {
        data = itemData;
        iconImage.sprite = data.icon;
        labelText.text = data.displayName;
    }

    public void SetOwned(bool owned)
    {
        if (owned)
        {
            iconImage.color = Color.white;
            labelText.text = data.displayName;
        }
        else
        {
            iconImage.color = new Color(1f, 1f, 1f, 0.7f); // faded
            labelText.text = data.notCollectedText; // e.g., "Not stolen yet"
        }
    }
}
