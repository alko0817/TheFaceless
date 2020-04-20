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
        BLOCK,
        STUNNED,
        SHOOTING,
        FLEE
    }

    NavMeshAgent navMeshAgent;
    BEHAVIOUR_STATE state;
    private static Vector3 startPosition;
    EnemyBlackboard blackboard;

    #region Shooter Parameters
    [Header("- Shooter Parameters")]
    public bool shooter;
    public float fleeDistance;
    public float fireRate;
    private float shootTimer;


    Transform projectileSpawn;
    public GameObject projectile;
    public GameObject[] projectiles;

    #endregion
    public float pursueDelay;
    private float pursueDelayTimer;
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

    #region Player Parameters
    [Header("- Player Parameters")]
    private GameObject player;
    private Vector3 lastKnownPlayerLocation;
    private float distanceToPlayer;

    private bool playerDetected;
    private bool canHitPlayer;

    private float timeSinceLastSawPlayer = Mathf.Infinity;
    [Tooltip("The number of seconds this enemy will wait after losing sight of the player before returning to its patrol route.")]
    public float suspicionTime;
    #endregion

    #region Sensing Parameters
    [Header("- Sensing Parameters")]
    [Tooltip("The frequency that this enemy gathers information about the player. The number of seconds between each Sense() action.")]
    public float senseFrequency;
    private float senseTimer;
    private bool pursuing;
    #endregion

    #region Combat Parameters
    public Transform attackPoint;
    public float attackHitBox = 1f;


    public int attackDamage;
    private bool attackThrown;
    private bool blocking;
    private bool stunned;

    
    SpawnEffect dissolving;
  //  Animator anim;
    #endregion
    void Start()
    {
        playerDetected = false;
        canHitPlayer = false;
        attackThrown = false;
        blocking = false;
        pursuing = false;
        stunned = false;

        senseTimer = 0.0f;
        shootTimer = 0f;

        player = GameObject.FindWithTag("Player");
        state = BEHAVIOUR_STATE.PATROL; 
        currentHealth = maxHealth;

        navMeshAgent = GetComponent<NavMeshAgent>();
        //attackPoint = transform.GetChild(2).transform;
        projectileSpawn = transform.GetChild(3).transform;

        currentWaypointIndex = 0;
        transform.position = GetCurrentWaypoint();
        startPosition = transform.position;

        blackboard = GameObject.FindWithTag("Blackboard").GetComponent<EnemyBlackboard>();
        dissolving = GetComponent<SpawnEffect>();

        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i] = Instantiate(projectile);

            projectiles[i].transform.position = projectileSpawn.position;
            projectiles[i].transform.rotation = projectileSpawn.rotation;
            projectiles[i].SetActive(false);
        }


     //   anim = GetComponent<Animator>();
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
        Gizmos.DrawWireSphere(attackPoint.position, attackHitBox);

    }

    #region AI LOOP
    void Sense()
    {

        if (distanceToPlayer < sightDistance && IsPlayerVisible())
        {
            if(!pursuing)
                blackboard.AddEnemyInSight(this.gameObject);

            lastKnownPlayerLocation = player.transform.position;

            playerDetected = true;
            timeSinceLastSawPlayer = 0.0f;
            print("Player sighted by " + gameObject.name);

        }
        else
        {

            blackboard.RemoveEnemyInSight(this.gameObject);
            blackboard.RemovePursuingEnemy(this.gameObject);
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

        if (playerDetected && distanceToPlayer > attackDistance && pursuing && !shooter)
        {
            state = BEHAVIOUR_STATE.PURSUE;
        }

        if(playerDetected && shooter)
        {
            state = BEHAVIOUR_STATE.SHOOTING;
        }

        if(distanceToPlayer < attackDistance && CanAttack())
        {
            state = BEHAVIOUR_STATE.ATTACK;
        }
        if(stunned)
        {
            state = BEHAVIOUR_STATE.STUNNED;
        }

        if(distanceToPlayer < fleeDistance && shooter)
        {
            state = BEHAVIOUR_STATE.FLEE;
        }
    }

    void Act()
    {

        if(state == BEHAVIOUR_STATE.SUSPICIOUS)
        {
            pursueDelayTimer = 0f;
            Stop();
            print(gameObject.name + " is suspicious");
        }

        if (state == BEHAVIOUR_STATE.PATROL)
        {
            pursueDelayTimer = 0f;

            Patrol();
        }

        if(state == BEHAVIOUR_STATE.SHOOTING)
        {
            pursueDelayTimer = 0f;
            Stop();
            Shoot();
        }

        if (state == BEHAVIOUR_STATE.PURSUE)
        {
            pursueDelayTimer += Time.deltaTime;

            if (pursueDelayTimer > pursueDelay)
            {
                MoveTo(lastKnownPlayerLocation);
            }

        }

        if (state == BEHAVIOUR_STATE.ATTACK)
        {
            Stop();

            pursueDelayTimer = 0f;

            if (!attackThrown)
            {
                StartCoroutine(Attack());
                StartCoroutine(ResetAttack());

            }

        }

        if (state == BEHAVIOUR_STATE.BLOCK)
        {
            pursueDelayTimer = 0f;

            Stop();
            if(!blocking)
            {
                Block();
                StartCoroutine(ResetBlock());
            }
        }

        if(state == BEHAVIOUR_STATE.STUNNED)
        {
            pursueDelayTimer = 0f;

            Stunned();
        }

        if(state == BEHAVIOUR_STATE.FLEE)
        {
            Flee();
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

            if(timeSinceArrivedAtWaypoint > waypointWaitTime)
            {
                Vector3 nextPosition;
                nextPosition = GetCurrentWaypoint();
                MoveTo(nextPosition);

            }
            print(gameObject.name + "  patrolling");

        }


    }

    bool IsPlayerVisible()
    {
        NavMeshHit hit;
        if (!navMeshAgent.Raycast(player.transform.position, out hit))
        {
            return true;
        }
        else
            return false;
    }

    public void SetPursuing(bool value)
    {
        pursuing = value;
    }
    public bool GetPursuing()
    {
        return pursuing;
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
        if (patrolPath != null)
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }
    }

    private Vector3 GetCurrentWaypoint()
    {
        if (patrolPath != null)
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }
        else return transform.position;
    }

    #endregion

    #region MOVEMENT
    void MoveTo(Vector3 location)
    {
        navMeshAgent.destination = location;
        navMeshAgent.isStopped = false;

    }

    void Flee()
    {

    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
    }
    #endregion

    #region COMBAT
    IEnumerator Attack()
    {
        attackThrown = true;
       // anim.SetTrigger("attack");
        yield return new WaitForSeconds(3f);
        if (!CanAttack())
            yield break;

        
        player.GetComponent<playerController>().TakeDamage(attackDamage);
        print(gameObject.name + " hit player");

    }

    IEnumerator ResetAttack()
    {
       yield return new WaitForSeconds(2f);
        attackThrown = false;

    }

    private bool CanAttack()
    {
        if (canHitPlayer)
        {
            if (Physics.CheckSphere(attackPoint.position, attackHitBox))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    void Block()
    {
        blocking = true;
        canHitPlayer = false;
        print(gameObject.name + " is blocking");
    }

    IEnumerator ResetBlock()
    {
        yield return new WaitForSeconds(10f);
        blocking = false;
        canHitPlayer = true;
    }

    private void Stunned()
    {
        Stop();
        StartCoroutine(Stun());
    }

    IEnumerator Stun()
    {
        canHitPlayer = false;
        yield return new WaitForSeconds(3);
        canHitPlayer = true;
        stunned = false;
    }

    public void SetStunned(bool value)
    {
        stunned = value;
    }

    void Shoot()
    {
        canHitPlayer = false;
        transform.LookAt(player.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        shootTimer += Time.deltaTime;
        if(shootTimer > fireRate)
        {
            shootTimer = 0f;


            for(int i = 0; i < projectiles.Length; i++)
            {
                if(!projectiles[i].activeInHierarchy)
                {
                    projectiles[i].transform.position = projectileSpawn.position;
                    projectiles[i].transform.rotation = projectileSpawn.rotation;
                    projectiles[i].GetComponent<Projectile>().SetDirection(transform.forward);
                    projectiles[i].SetActive(true);
                    break;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {

        if (!blocking)
        {
            currentHealth -= damage;
            healthBar.fillAmount = currentHealth / maxHealth;
            Debug.Log("UGH");
        }
        else
            print("attack blocked");
        //HURT ANIMATIONS

    }

    void Die()
    {
        blackboard.RemoveEnemyInSight(this.gameObject);
        blackboard.RemovePursuingEnemy(this.gameObject);
        Stop();
        dissolving.enabled = true;
        Destroy(gameObject, 2f);
        //DIE ANIMATION
    }
    #endregion


}
