using Pathfinding;
using UnityEngine;

public class PersonController : MonoBehaviour
{
    public Transform target;
    private AIPath ai;
    private Animator animator;
    private int speedHash = Animator.StringToHash("Speed");

    void Start()
    {
        ai = GetComponent<AIPath>();
        animator = GetComponentInChildren<Animator>();

    }

    void Update()
    {
        ai.destination = target.position;
        animator.SetFloat(speedHash, ai.velocity.magnitude / ai.maxSpeed);
    }
}
