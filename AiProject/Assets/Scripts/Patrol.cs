using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Patrol : MonoBehaviour
{
    public float lookRadius = 10f;

    Transform target; //Reference to the player
    NavMeshAgent agent; //Reference to the NavMeshAgent
    CharacterCombat combat;

    public float speed;
    private float waitTime;

    public Transform[] moveSpots;
    private int randomSpot;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CharacterCombat>();
        waitTime = UnityEngine.Random.Range(0, 4);
}

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        randomSpot = UnityEngine.Random.Range(0, moveSpots.Length);
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            // Move towards the target
            agent.SetDestination(target.position);

            //If within attacking distance
            if (distance <= agent.stoppingDistance)
            {
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                if (targetStats != null)
                {
                    // Attack the target
                    combat.Attack(targetStats);
                }
                // Face the target
                FaceTarget();
            }
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        //moves to the random spot
        if (Vector3.Distance(transform.position, moveSpots[randomSpot].position) >= 2f)
        {
            agent.SetDestination(moveSpots[randomSpot].position);
        }
        //if reaches the spot, then waits
        if (Vector3.Distance(transform.position, moveSpots[randomSpot].position) < 2f)
        {
            //and finds another spot
            if (waitTime <= 0)
            {
                int currentSpot = randomSpot;
                randomSpot = UnityEngine.Random.Range(0, moveSpots.Length);
                if (moveSpots.Length > 1)
                {
                    while (currentSpot == randomSpot)
                    {
                        randomSpot = UnityEngine.Random.Range(0, moveSpots.Length);
                    }
                }
                waitTime = UnityEngine.Random.Range(0, 4);
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
