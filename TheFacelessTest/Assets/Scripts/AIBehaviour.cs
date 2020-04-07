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
        IDLE,
        PATROL,
        SUSPICIOUS,
        PURSUE,
        ATTACK,
        BLOCK
    }

    public VisualizePatrol patrolPath;
    public float waypointTolerance = 1.0f;
    int currentWaypointIndex = 0;

    public float sightDistance;
    public float attackDistance;
    public float speed; 

    public float maxHealth;
    private float currentHealth;

    public Image healthBar;
    private static Vector3 startPosition;


    #region Player Parameters

    private GameObject player;
    private Vector3 lastKnownPlayerLocation;
    private float distanceToPlayer;

    private bool playerDetected;
    private float timeSinceLastSawPlayer = Mathf.Infinity;
    private float suspisionTime = 5.0f;
    #endregion

    public float senseFrequency;
    private float senseTimer;


    BEHAVIOUR_STATE state;

    // Start is called before the first frame update




    void Start()
    {
        playerDetected = false;
        senseTimer = 0.0f;
        player = GameObject.FindWithTag("Player");
        state = BEHAVIOUR_STATE.IDLE;
        currentHealth = maxHealth;

        GetComponent<NavMeshAgent>().stoppingDistance = attackDistance;

        transform.position = patrolPath.GetWaypoint(0);
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

        senseTimer += Time.deltaTime;


        //Debug.Log("Start position = " + startPosition.position);

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
        //Debug.Log("Last Known PLayer position = " + lastKnownPlayerLocation.position);
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
            Debug.Log("Player sighted by " + gameObject.name);
            timeSinceLastSawPlayer = 0.0f;
        }
        else
        {
            playerDetected = false;
            Debug.Log(gameObject.name + " lost sight of Player");
        }

    }

    void Decide()
    {
        if(!playerDetected && timeSinceLastSawPlayer > suspisionTime)
        {
            state = BEHAVIOUR_STATE.PATROL;
        }
        else if(!playerDetected && timeSinceLastSawPlayer < suspisionTime)
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
        if (state == BEHAVIOUR_STATE.IDLE)
        {
            Debug.Log(gameObject.name + " is idling");
        }

        if(state ==BEHAVIOUR_STATE.SUSPICIOUS)
        {
            Debug.Log(gameObject.name + " is suspicious");
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
        //MoveTo(startPosition);
        Vector3 nextPosition = startPosition;

        if (patrolPath != null)
        {
            if (AtWaypoint())
            {
                CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();
        }

        MoveTo(nextPosition);

        Debug.Log(gameObject.name + " is patrolling");

    }

    #region Waypoint Finders
    private bool AtWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());

        return distanceToWaypoint < waypointTolerance;
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
        //Vector3 direction = location.position - transform.position;
        //direction.Normalize();

        //transform.position = transform.position + (direction * speed * Time.deltaTime);

        GetComponent<NavMeshAgent>().destination = location;
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
