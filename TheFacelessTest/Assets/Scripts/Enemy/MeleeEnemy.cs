using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class MeleeEnemy : EnemyBase
{

    [Header("Unique Properties")]

    Transform attackPoint;

    public PatrolRoute patrolPath;

    public float pursueSpeed;

    #region SOUND
    //public AudioClip BlockSound;
    //public AudioClip DodgeSound;
    #endregion

    #region BOOLEANS
    private bool canHitPlayer;
    private bool attackThrown;
    private bool combatActionInProgress;
    #endregion

    #region PATROLLING VARIABLES
    public float waypointTolerance;
    public float waypointWaitTime;
    private int currentWaypointIndex;
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
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attackPoint = transform.GetChild(0).transform;

        currentWaypointIndex = 0;
        transform.position = GetCurrentWaypoint();

        canHitPlayer = true;
        attackThrown = false;
        combatActionInProgress = false;

        state_ = STATE.PATROL;
    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();
        timeSinceArrivedAtWaypoint += Time.deltaTime;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackHitBox);

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
            if (timeSinceLastSawPlayer > suspicionTime)
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
            print(player.GetComponent<PlayerAttack>().GetAttacking());
            if (player.GetComponent<PlayerAttack>().GetAttacking())
            {
                rand = UnityEngine.Random.Range(0, 10);
                print(rand);

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
        anim.SetTrigger("attack");
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
        Vector3 direction = (player.transform.position - transform.position) * -1;
        direction.Normalize();
        navMeshAgent.Move(direction * 2);
        yield return new WaitForSeconds(1f);
        combatActionInProgress = false;

    }

    IEnumerator Block()
    {
        combatActionInProgress = true;
        blocking = true;
        print(gameObject.name + " is blocking");
        yield return new WaitForSeconds(2f);
        blocking = false;
        combatActionInProgress = false;

    }


    void Patrol()
    {
        if (patrolPath != null)
        {
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


}
