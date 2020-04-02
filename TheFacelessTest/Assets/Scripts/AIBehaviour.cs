using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    enum BEHAVIOUR_STATE
    {
        IDLE,
        PATROL,
        PURSUE,
        ATTACK,
        BLOCK
    }

    public float sightDistance;
    public float attackDistance;
    public float speed;


    private GameObject player;
    private Transform lastKnownPlayerLocation;
    private float distanceToPlayer;

    public float senseFrequency;
    private float senseTimer;

    private bool playerDetected;

    BEHAVIOUR_STATE state;

    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
        senseTimer = 0.0f;
        player = GameObject.FindWithTag("Player");
        state = BEHAVIOUR_STATE.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

        senseTimer += Time.deltaTime;

        if(senseTimer > senseFrequency)
        {
            senseTimer = 0.0f;
            Sense();
            Decide();
        }

        Act();
    }


    void Sense()
    {

        if (distanceToPlayer < sightDistance)
        {
            playerDetected = true;
            lastKnownPlayerLocation = player.transform;
            Debug.Log("Player sighted by " + gameObject.name);
        }
        else
        {
            playerDetected = false;
            Debug.Log(gameObject.name + " lost sight of Player");
        }

    }

    void Decide()
    {
        if(!playerDetected && state != BEHAVIOUR_STATE.PURSUE)
        {
            state = BEHAVIOUR_STATE.PATROL;
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

    void Attack()
    {
        Debug.Log(gameObject.name + " is attacking");
    }

    void Patrol()
    {
        Debug.Log(gameObject.name + " is patrolling");
    }

    void MoveTo(Transform location)
    {
        Vector3 direction = location.position - transform.position;
        direction.Normalize();

        transform.position = transform.position + (direction * speed * Time.deltaTime);
    }

    void Block()
    {
        Debug.Log(gameObject.name + " is blocking");
    }
}
