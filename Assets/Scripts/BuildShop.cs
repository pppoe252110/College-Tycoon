using System.Linq;
using UnityEngine;

public class BuildShop : MonoBehaviour
{
    public GameObject shop;
    public Transform shopContent;
    public BuildingShopItem shopItemPrefab;
    public BuildingItem[] buildingItems;
    private Builder builder;
    public static BuildShop Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        builder = GetComponent<Builder>();
        shop.SetActive(false);
        GenerateBuildingsInShop();
        builder.SetBuilding(null);
    }

    public void SwitchDestroyMode()
    {
        builder.SwitchDestroyMode();
    }

    public void GenerateBuildingsInShop()
    {
        for (int i = 0; i < buildingItems.Length; i++)
        {
            var g = Instantiate(shopItemPrefab, shopContent);
            int v = i;
            g.InitItem(i, buildingItems[i]);
            g.button.onClick.AddListener(delegate { builder.SetBuilding(buildingItems[v]); });
        }
    }

    public void ShowShop()
    {
        shop.SetActive(!shop.activeSelf);
        if (!shop.activeSelf)
            builder.SetBuilding(null);
        builder.destroyMode = false;
    }
}
