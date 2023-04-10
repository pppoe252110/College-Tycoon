using UnityEngine;
using UnityEngine.UI;

public class BuildingShopItem : MonoBehaviour
{
    public Text text;
    public Image outline;
    public Image icon;
    public Button button;
    private int itemId;

    public void InitItem(int itemId, BuildingItem item)
    {
        this.itemId = itemId;
        text.text = item.buildingItemName;
        //text.text += string.Format("\n{0} Уровень", item.quality + 1);
        icon.sprite = item.icon;
        outline.color = IdentityManager.Instance.identityPreset.qualityColors[item.quality];
    }
}
