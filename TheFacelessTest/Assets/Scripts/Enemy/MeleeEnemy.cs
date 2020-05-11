using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Security.Permissions;

public class MeleeEnemy : EnemyBase
{
    [Header("Unique Components")]
    [Header("- Unique Properties -")]
    #region COMPONENETS
    public PatrolRoute patrolPath;
    Transform attackPoint;
    #endregion

    [Header("Unique Variables")]
    #region VARIABLES
    public float pursueSpeed;
    public int attackDamage;
    public float attackDistance;
    public float attackHitBox = 1f;
    public int dodgeChanceOutOf10;
    public int blockChanceOutOf10;
    public float waypointTolerance;
    public float waypointWaitTime;
    public float attackDelay = 1f;
    public float pursueDelay;
    public float suspicionTime;
    [Space]
    public AudioClip attackSound;
    public AudioClip blockSound;
    #endregion

    #region INTERNAL PARAMETERS
    private int currentWaypointIndex;
    private float pursueDelayTimer;
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    #endregion

    #region BOOLEANS
    private bool canHitPlayer;
    internal bool attackThrown;
    internal bool dodging = false;
    private bool combatActionInProgress;
    #endregion


    #region SOUND
    //public AudioClip BlockSound;
    //public AudioClip DodgeSound;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
        attackPoint = transform.GetChild(0).transform;

        currentWaypointIndex = 0;
        transform.position = GetCurrentWaypoint();

        canHitPlayer = true;
        attackThrown = false;
        combatActionInProgress = false;

        if (patrolPath == null)
            state_ = STATE.IDLE;
        else
            state_ = STATE.PATROL;

        if(useGlobalSettings)
        {
            SetUp();
        }
    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();
        timeSinceArrivedAtWaypoint += Time.deltaTime;

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(attackPoint.position, attackHitBox);

        Gizmos.DrawWireSphere(transform.position, sightDistance);

    }

    #region AI LOOP

    protected override void Sense()
    {
        base.Sense();
        if (blocking || stunned)
        {
            canHitPlayer = false;
        }
        else
            canHitPlayer = true;
    }
    protected override void Decide()
    {
        base.Decide();

        if (!engaging)
        {
            if (patrolPath == null)
            {
                state_ = STATE.IDLE;
            }
            else if (timeSinceLastSawPlayer > suspicionTime)
            {
                state_ = STATE.PATROL;
            }

            if (timeSinceLastSawPlayer < suspicionTime)
            {
                state_ = STATE.SUSPICIOUS;
            }
        }

        if (engaging)
        {
            if (distanceToPlayer > attackDistance && !combatActionInProgress)
            {
                state_ = STATE.PURSUE;
            }

            if (CanAttack())
            {
                state_ = STATE.IN_COMBAT;
            }
        }
        print("State of " + gameObject.name + ": " + state_);
    }

    protected override void Act()
    {
        base.Act();
        if (state_ == STATE.IDLE)
        {
            Guard();
        }
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

        if (state_ == STATE.IN_COMBAT)
        {
            Stop();
            ResolveCombatAction();
        }
    }
    #endregion
    private bool CanAttack()
    {
        if (canHitPlayer && engaging)
        {
            if (distanceToPlayer < attackDistance)
            {
                transform.LookAt(player.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

                return true;
            }
        }

        return false;
    }
    #region ACTIONS

    void ResolveCombatAction()
    {
        int rand = -1;

        if (!combatActionInProgress)
        {
            if (player.GetComponent<PlayerAttack>().GetAttacking())
            {
                rand = UnityEngine.Random.Range(0, 10);

                if (0 <= rand && rand < dodgeChanceOutOf10)
                {
                    StartCoroutine(Dodge());
                }
                else if (dodgeChanceOutOf10 <= rand && rand < dodgeChanceOutOf10 + blockChanceOutOf10)
                {
                    StartCoroutine(Block());
                }
                else if (rand >= blockChanceOutOf10 + dodgeChanceOutOf10)
                {
                    StartCoroutine(Attack());
                }

            }
            else
            {
                StartCoroutine(Attack());
            }
        }

    }

    IEnumerator Attack()
    {
        Stop();
        attackThrown = true;
        combatActionInProgress = true;
        anim.Attack();
        audioSource.PlayOneShot(attackSound);
        yield return new WaitForSeconds(attackDelay);
        if (Physics.CheckSphere(attackPoint.position, attackHitBox, playerMask))
        {
            player.GetComponent<playerController>().TakeDamage(attackDamage);
        }
        yield return new WaitForSeconds(1f);
        attackThrown = false;
        combatActionInProgress = false;

    }

    IEnumerator Dodge()
    {
        combatActionInProgress = true;
        dodging = true;
        anim.Dodge();
        Vector3 direction = (player.transform.position - transform.position) * -1;
        direction.Normalize();
        navMeshAgent.Move(direction * 2);
        yield return new WaitForSeconds(1f);
        dodging = false;
        combatActionInProgress = false;

    }

    IEnumerator Block()
    {
        combatActionInProgress = true;
        blocking = true;
        audioSource.PlayOneShot(blockSound);
        anim.Block();
        print(gameObject.name + " is blocking");
        yield return new WaitForSeconds(2f);
        blocking = false;
        combatActionInProgress = false;

    }


    void Patrol()
    {
        if (patrolPath != null)
        {
            audioSource.PlayOneShot(StaticSound);
            if (AtWaypoint())
            {
                Stop();
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

    public void SetUp()
    {
        sightDistance =  blackboard.MeleeSightDistance;
        attackDistance = blackboard.MeleeAttackDistance;
        maxHealth = blackboard.MeleeMaxHealth;
        initialSpeed = blackboard.MeleeInitialSpeed;
        pursueSpeed = blackboard.MeleePursueSpeed;
        attackDamage = blackboard.MeleeAttackDamage;
        pursueDelay = blackboard.MeleePusueDelay;
        suspicionTime = blackboard.MeleeSuspicionTime;
        dodgeChanceOutOf10 = blackboard.DodgeChanceOutOf10;
        blockChanceOutOf10 = blackboard.BlockChanceOutOf10;
        waypointTolerance = blackboard.MeleeWaypointTolerance;
        waypointWaitTime = blackboard.MeleeWaypointWaitTime;
    }
}
