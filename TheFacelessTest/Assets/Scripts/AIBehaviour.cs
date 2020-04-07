using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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

    public float maxHealth;
    private float currentHealth;

    public Image healthBar;
    private static Transform startPosition;


    #region Player Parameters

    private GameObject player;
    private Transform lastKnownPlayerLocation;
    private float distanceToPlayer;

    private bool playerDetected;

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

        startPosition = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

        senseTimer += Time.deltaTime;

        Debug.Log("Start position = " + startPosition.position);

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
        Debug.Log("Last Known PLayer position = " + lastKnownPlayerLocation.position);

    }


    void Sense()
    {

        if (distanceToPlayer < sightDistance)
        {
            lastKnownPlayerLocation = player.transform;

            playerDetected = true;
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
            Debug.Log(gameObject.name + " is idling");
        }

        if (state == BEHAVIOUR_STATE.PATROL)
        {
            Patrol();
        }

        if (state == BEHAVIOUR_STATE.PURSUE)
        {
            MoveTo(lastKnownPlayerLocation);
            if(transform == lastKnownPlayerLocation)
            {
                state = BEHAVIOUR_STATE.PATROL;
            }
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
        MoveTo(startPosition);
    }

    void MoveTo(Transform location)
    {
        //Vector3 direction = location.position - transform.position;
        //direction.Normalize();

        //transform.position = transform.position + (direction * speed * Time.deltaTime);

        GetComponent<NavMeshAgent>().destination = location.position;
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
