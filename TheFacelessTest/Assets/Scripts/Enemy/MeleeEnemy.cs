using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class MeleeEnemy : EnemyBase
{
    Transform attackPoint;

    public PatrolRoute patrolPath;

    public float pursueSpeed;

    #region SOUND
    public AudioClip BlockSound;
    public AudioClip DodgeSound;
    #endregion

    #region BOOLEANS
    private bool canHitPlayer;
    private bool attackThrown;
    private bool blocking;
    private bool dodge;
    private bool block;
    #endregion

    #region PATROLLING VARIABLES
    public float waypointTolerance;
    public float waypointWaitTime;
    private int currentWaypointIndex;
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    #endregion

    #region COMBAT VARIABLES
    public float attackHitBox = 1f;

    public int attackDamage;
    public float attackDistance;

    public int dodgeChanceOutOf10;
    public int blockChanceOutOf10;
    #endregion


    #region TIME AND TIMERS
    public float attackDelay = 1f;
    public float pursueDelay;
    private float pursueDelayTimer;
    public float suspicionTime;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attackPoint = transform.GetChild(1).transform;

        currentWaypointIndex = 0;
        transform.position = GetCurrentWaypoint();

        state_ = STATE.PATROL;
    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();
        timeSinceArrivedAtWaypoint += Time.deltaTime;

    }

    #region AI LOOP
    protected override void Decide()
    {
        base.Decide();
        if (!playerDetected && timeSinceLastSawPlayer > suspicionTime)
        {
            state_ = STATE.PATROL;
        }
        if (!playerDetected && timeSinceLastSawPlayer < suspicionTime)
        {
            state_ = STATE.SUSPICIOUS;
        }

        if (playerDetected && distanceToPlayer > attackDistance && engaging)
        {
            state_ = STATE.PURSUE;
        }

        if (distanceToPlayer < attackDistance && !blocking && !stunned)
        {
            canHitPlayer = true;
        }
        else
        {
            canHitPlayer = false;
        }


        if (CanAttack())
        {
            int rand = -1;
            if (player.GetComponent<PlayerAttack>().GetAttacking())
            {

                rand = UnityEngine.Random.Range(0, 10);
                print(rand);

                if (0 <= rand && rand < dodgeChanceOutOf10)
                {
                    dodge = true;
                    block = false;
                }
                else if (dodgeChanceOutOf10 <= rand && rand < dodgeChanceOutOf10 + blockChanceOutOf10)
                {
                    dodge = false;
                    block = true;
                }
                else if (rand >= blockChanceOutOf10 + dodgeChanceOutOf10)
                {
                    print("neither");
                    dodge = false;
                    block = false;
                }

            }

        }
        else if (!CanAttack())
        {
            dodge = false;
            block = false;
        }

        if (CanAttack() && !dodge && !block)
        {
            state_ = STATE.ATTACK;
        }

        else if (dodge)
        {
            state_ = STATE.DODGE;
        }

        else if (block)
        {
            state_ = STATE.BLOCK;
        }

    }

    protected override void Act()
    {
        base.Act();
        if (state_ == STATE.PATROL)
        {
            pursueDelayTimer = 0f;

            Patrol();
        }

        if (state_ == STATE.SUSPICIOUS)
        {
            pursueDelayTimer = 0f;
            Stop();
        }

        if (state_ == STATE.PURSUE)
        {

            pursueDelayTimer += Time.deltaTime;

            if (pursueDelayTimer > pursueDelay)
            {
                MoveTo(lastKnownPlayerLocation, pursueSpeed);
            }

        }

        if (state_ == STATE.ATTACK)
        {
            Stop();

            pursueDelayTimer = 0f;

            if (!attackThrown)
            {
                StartCoroutine(Attack());
            }

        }


        if (state_ == STATE.DODGE)
        {

            pursueDelayTimer = 0f;

            StartCoroutine(Dodge());
        }


        if (state_ == STATE.BLOCK)
        {

            pursueDelayTimer = 0f;

            Stop();
            if (!blocking)
            {
                StartCoroutine(Block());
            }
        }


    }
    #endregion

    private bool CanAttack()
    {
        if (canHitPlayer)
        {
            if (Physics.CheckSphere(attackPoint.position, attackHitBox, playerMask))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    #region ACTIONS
    IEnumerator Attack()
    {
        attackThrown = true;
        //transform.LookAt(player.transform);
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(attackDelay);
        if (!CanAttack())
            yield break;


        player.GetComponent<playerController>().TakeDamage(attackDamage);

        yield return new WaitForSeconds(2f);
        attackThrown = false;


    }

    IEnumerator Dodge()
    {
        print("dodge");
        audioSource.PlayOneShot(DodgeSound);
        Vector3 direction = (player.transform.position - transform.position) * -1;
        direction.Normalize();
        
        navMeshAgent.Move(direction / 5f);
        yield return new WaitForSeconds(0.5f);
        dodge = false;
    }

    IEnumerator Block()
    {
        blocking = true;
        print(gameObject.name + " is blocking");
        yield return new WaitForSeconds(2f);
        blocking = false;

    }


    void Patrol()
    {
        if (patrolPath != null)
        {
            if (AtWaypoint())
            {
                timeSinceArrivedAtWaypoint = 0;
                CycleWaypoint();

            }

            if (timeSinceArrivedAtWaypoint > waypointWaitTime)
            {
                Vector3 nextPosition;
                nextPosition = GetCurrentWaypoint();
                MoveTo(nextPosition, initialSpeed);

            }

        }

    }
    #endregion

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
}
