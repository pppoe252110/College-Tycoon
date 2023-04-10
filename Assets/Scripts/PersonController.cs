using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    // Update is called once per frame
    void LateUpdate()
    {
        ai.destination = target.position;
        animator.SetFloat(speedHash, ai.velocity.magnitude / ai.maxSpeed);
    }
}
