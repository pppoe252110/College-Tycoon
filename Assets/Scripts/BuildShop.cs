using System.Collections.Generic;
using UnityEngine;

public class BuildShop : MonoBehaviour
{
    public GameObject shop;
    public Transform shopContent;
    public BuildingShopItem shopItemPrefab;
    public BuildingItem[] buildingItems;
    private Builder builder;

    void Start()
    {
        builder=GetComponent<Builder>();
        shop.SetActive(false);
        GenerateBuildingsInShop();
        builder.SetBuilding(null);
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
        if(!shop.activeSelf)
            builder.SetBuilding(null);
    }
}
