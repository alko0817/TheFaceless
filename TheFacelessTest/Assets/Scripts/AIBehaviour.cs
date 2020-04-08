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
    [Header("- Patrolling Parameters")]
    [Tooltip("The Patrol Route of this enemy. Drag the Patrol Route you want this enemy to follow into this box.")]
    public PatrolRoute patrolPath;
    [Tooltip("The distance away from a waypoint that this enemy has to reach before moving to the next one. Must be no less than 1.2.")]
    public float waypointTolerance;
    [Tooltip("The number of seconds this enemy waits at each waypoint.")]
    public float waypointWaitTime;
    private int currentWaypointIndex;
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    #endregion

    #region Distancing Parameters
    [Header("- Distancing Parameters")]
    [Tooltip("The range at which this enemy will detect the player")]
    public float sightDistance;
    [Tooltip("The range at which this enemy will attack the player")]
    public float attackDistance;
    #endregion

    #region Health Parameters
    [Header("- Health Parameters")]
    [Tooltip("The max health of this enemy.")]
    public float maxHealth;
    private float currentHealth;
    [Tooltip("The health bar image associated with this enemy")]
    public Image healthBar;

    #endregion

    private static Vector3 startPosition;


    #region Player Parameters
    [Header("- Player Parameters")]
    private GameObject player;
    private Vector3 lastKnownPlayerLocation;
    private float distanceToPlayer;

    private bool playerDetected;
    private float timeSinceLastSawPlayer = Mathf.Infinity;
    [Tooltip("The number of seconds this enemy will wait after losing sight of the player before returning to its patrol route.")]
    public float suspicionTime;
    #endregion

    #region Sensing Parameters
    [Header("- Sensing Parameters")]
    [Tooltip("The frequency that this enemy gathers information about the player. The number of seconds between each Sense() action.")]
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
        transform.position = GetCurrentWaypoint();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

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

        UpdateTimers();
    }

    private void UpdateTimers()
    {
        timeSinceLastSawPlayer += Time.deltaTime;
        timeSinceArrivedAtWaypoint += Time.deltaTime;
        senseTimer += Time.deltaTime;

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

    #region AI LOOP
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
        if(!playerDetected && timeSinceLastSawPlayer > suspicionTime)
        {
            state = BEHAVIOUR_STATE.PATROL;
        }
        if (!playerDetected && timeSinceLastSawPlayer < suspicionTime)
        {
            state = BEHAVIOUR_STATE.SUSPICIOUS;
        }

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

        if(state == BEHAVIOUR_STATE.SUSPICIOUS)
        {
            Stop();
            print(gameObject.name + " is suspicious");
        }

        if (state == BEHAVIOUR_STATE.PATROL)
        {
            Patrol();
        }

        if (state == BEHAVIOUR_STATE.PURSUE)
        {
            MoveTo(lastKnownPlayerLocation);
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
    #endregion

    void Patrol()
    {

        if (patrolPath != null)
        {
            if (AtWaypoint())
            {
                timeSinceArrivedAtWaypoint = 0;
                CycleWaypoint();
                print("cycled waypoint");
                
            }
            print(AtWaypoint());

            if(timeSinceArrivedAtWaypoint > waypointWaitTime)
            {
                Vector3 nextPosition;
                nextPosition = GetCurrentWaypoint();
                MoveTo(nextPosition);

            }
            print(gameObject.name + "  patrolling");

        }


    }

    #region WAYPOINT FINDERS
    private bool AtWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
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

    #region MOVEMENT
    void MoveTo(Vector3 location)
    {
        navMeshAgent.destination = location;
        navMeshAgent.isStopped = false;

    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
    }
    #endregion

    #region COMBAT
    void Attack()
    {
        Stop();

        Debug.Log(gameObject.name + " is attacking");
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
    #endregion
}
