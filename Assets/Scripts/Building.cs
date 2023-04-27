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
        if(people.Count > 0)
            ReleasePeople();
    }

    public virtual void ReleasePeople()
    {
        people.ForEach(a => a.gameObject.SetActive(true));
    }

    public virtual void Update()
    {
        var period = TimeManager.Instance.GetCurrentAction();
        if (people.Count > 0 && (period == PeriodAction.FreeTime))
        {
            ReleasePeople();
        }
    }
}
