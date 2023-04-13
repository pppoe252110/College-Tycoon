using UnityEngine;
[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/BuildingItem", order = 1)]
public class BuildingItem : ScriptableObject
{
    public Building buildingPrefab;
    public Sprite icon;
    public string buildingItemName;
    public int quality = 1;
    public float price = 1000;
}
