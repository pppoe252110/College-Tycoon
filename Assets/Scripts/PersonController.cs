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
        var r = GridBuilder.Instance.GetRandomBuildingTypeOf<ColledgeBuilding>();
        if (r && (ai.reachedDestination || destination==null))
        {
            ai.destination = GridBuilder.Instance.GetRandomBuildingTypeOf<ColledgeBuilding>().enterence.position;
            destination = r;
        }
        else if (ai.isStopped || ai.reachedDestination)
        {
            ai.destination = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
            destination = null;
        }
        if (ai.reachedDestination && destination)
        {
            destination.people.Add(this);
            destination = null;
            gameObject.SetActive(false);
        }
        ai.isStopped = false;
        animator.SetFloat(speedHash, ai.velocity.magnitude / ai.maxSpeed);
    }
}
