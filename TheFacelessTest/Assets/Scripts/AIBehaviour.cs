using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class AIBehaviour : MonoBehaviour
{
    enum BEHAVIOUR_STATE
    {
        PATROL,
        SUSPICIOUS,
        PURSUE,
        ATTACK,
        BLOCK
    }

    NavMeshAgent navMeshAgent;

    #region Patrolling Paramenters

    public PatrolRoute patrolPath;
    public float waypointTolerance;
    int currentWaypointIndex;
    #endregion

    public float sightDistance;
    public float attackDistance;

    #region Health Parameters

    public float maxHealth;
    private float currentHealth;
    public Image healthBar;
    #endregion

    private static Vector3 startPosition;


    #region Player Parameters

    private GameObject player;
    private Vector3 lastKnownPlayerLocation;
    private float distanceToPlayer;

    private bool playerDetected;
    private float timeSinceLastSawPlayer = Mathf.Infinity;
    private float suspisionTime = 5.0f;
    #endregion

    #region Sensing Parameters
    public float senseFrequency;
    private float senseTimer;
    #endregion

    BEHAVIOUR_STATE state;



    void Start()
    {
        playerDetected = false;
        senseTimer = 0.0f;
        player = GameObject.FindWithTag("Player");
        state = BEHAVIOUR_STATE.PATROL; 
        currentHealth = maxHealth;

        navMeshAgent = GetComponent<NavMeshAgent>();

        currentWaypointIndex = 0;
        waypointTolerance = 1.5f;
        transform.position = GetCurrentWaypoint();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

        senseTimer += Time.deltaTime;


        if (senseTimer > senseFrequency)
        {
            senseTimer = 0.0f;
            Sense();
            Decide();

        }
        Act();

        if (currentHealth <= 0)
        {
            Die();
        }

        timeSinceLastSawPlayer += Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

    void Sense()
    {

        if (distanceToPlayer < sightDistance)
        {
            lastKnownPlayerLocation = player.transform.position;

            playerDetected = true;
            timeSinceLastSawPlayer = 0.0f;
            print("Player sighted by " + gameObject.name);

        }
        else
        {
            playerDetected = false;
            print(gameObject.name + " lost sight of Player");
        }

    }

    void Decide()
    {
        if(!playerDetected && timeSinceLastSawPlayer > suspisionTime)
        {
            state = BEHAVIOUR_STATE.PATROL;
        }
        //if (!playerDetected && timeSinceLastSawPlayer < suspisionTime)
        //{
        //    state = BEHAVIOUR_STATE.SUSPICIOUS;
        //}

        if (playerDetected && distanceToPlayer > attackDistance)
        {
            state = BEHAVIOUR_STATE.PURSUE;
        }

        if(distanceToPlayer < attackDistance)
        {
            state = BEHAVIOUR_STATE.ATTACK;
        }
    }

    void Act()
    {


        if(state ==BEHAVIOUR_STATE.SUSPICIOUS)
        {
            print(gameObject.name + " is suspicious");
        }

        if (state == BEHAVIOUR_STATE.PATROL)
        {
            Patrol();

        }

        if (state == BEHAVIOUR_STATE.PURSUE)
        {
            MoveTo(lastKnownPlayerLocation);
            //if (transform.position == lastKnownPlayerLocation)
            //{
            //    state = BEHAVIOUR_STATE.PATROL;

            //}
        }

        if(state == BEHAVIOUR_STATE.ATTACK)
        {
            Attack();
        }

        if(state == BEHAVIOUR_STATE.BLOCK)
        {
            Block();
        }

    }

    void Attack()
    {
        Debug.Log(gameObject.name + " is attacking");
    }

    void Patrol()
    {

        

        if (patrolPath != null)
        {
            if (AtWaypoint())
            {
                CycleWaypoint();
                print("cycled waypoint");
            }
            Vector3 nextPosition;
            print(AtWaypoint());
            nextPosition = GetCurrentWaypoint();
            MoveTo(nextPosition);
            print(gameObject.name + " is patrolling");
            print("currentWaypointIndex: " + currentWaypointIndex);

        }

        // MoveTo(nextPosition);

    }

    #region Waypoint Finders
    private bool AtWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
        print(distanceToWaypoint);
        if (distanceToWaypoint <= waypointTolerance)
            return true;
        else
            return false;
    }

    private void CycleWaypoint()
    {
        currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
    }

    private Vector3 GetCurrentWaypoint()
    {
        return patrolPath.GetWaypoint(currentWaypointIndex);
    }

    #endregion


    void MoveTo(Vector3 location)
    {
        navMeshAgent.destination = location;
        navMeshAgent.isStopped = false;

    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
    }

    void Block()
    {
        Debug.Log(gameObject.name + " is blocking");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;

        //HURT ANIMATIONS
        Debug.Log("UGH");


    }

    void Die()
    {
        Debug.Log("Death");
        //DIE ANIMATION
    }

}
