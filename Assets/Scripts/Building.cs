using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Transform enterence;
    public BuildingItem buildingItem;
    public List<PersonController> people = new List<PersonController>();
    public int maxPeople = 5;

    private void OnDestroy()
    {
        people.ForEach(a=>a.gameObject.SetActive(true));
    }
}
