using Pathfinding;
using UnityEngine;

public class PersonController : MonoBehaviour
{
    private AIPath ai;
    private Animator animator;
    private int speedHash = Animator.StringToHash("Speed");
    private Building destination;

    public void Init()
    {
        SelectRandomSkin();
        ai = GetComponent<AIPath>();
        ai.isStopped = true;
        animator = GetComponentInChildren<Animator>();
        PeopleController.Instance.people.Add(this);
    }
    public void SelectRandomSkin()
    {
        var r = Random.Range(0, PeopleController.Instance.skins.Length);
        Instantiate(PeopleController.Instance.skins[r], transform);
    }

    void Update()
    {
        UpdateDestination();
    }

    public void UpdateDestination()
    {
        if (ai.reachedDestination && destination)
        {
            destination.people.Add(this);
            destination = null;
            gameObject.SetActive(false);
        }
        if (ai.reachedDestination || (destination == null && ai.reachedDestination)||(destination==null && TimeManager.Instance.GetCurrentAction() != PeriodAction.FreeTime))
        {
            switch (TimeManager.Instance.GetCurrentAction())
            {
                case PeriodAction.Sleep:
                    var d = GridBuilder.Instance.GetRandomBuildingTypeOf<DormitoryBuilding>();
                    if (d)
                    {
                        ai.destination = GridBuilder.Instance.GetRandomBuildingTypeOf<DormitoryBuilding>().enterence.position;
                        destination = d;
                    }
                    else
                    {
                        ai.destination = new Vector3(Random.Range(-140f, 140f), 0, Random.Range(-140f, 140f));
                        destination = null;
                    }
                    break;
                case PeriodAction.Study:
                    var c = GridBuilder.Instance.GetRandomBuildingTypeOf<ColledgeBuilding>();
                    if (c)
                    {
                        ai.destination = GridBuilder.Instance.GetRandomBuildingTypeOf<ColledgeBuilding>().enterence.position;
                        destination = c;
                    }
                    else
                    {
                        ai.destination = new Vector3(Random.Range(-140f, 140f), 0, Random.Range(-140f, 140f));
                        destination = null;
                    }
                    break;
                case PeriodAction.FreeTime:
                    ai.destination = new Vector3(Random.Range(-140f, 140f), 0, Random.Range(-140f, 140f));
                    destination = null;
                    break;
            }
        }
        else if (ai.isStopped || ai.reachedDestination)
        {
            ai.destination = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
            destination = null;
        }

        ai.isStopped = false;
        animator.SetFloat(speedHash, ai.velocity.magnitude / ai.maxSpeed);
    }
}
