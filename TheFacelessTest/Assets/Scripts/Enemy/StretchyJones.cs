using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchyJones : EnemyBase
{
    [Header("Unique Components")]
    [Header("- Unique Properties -")]
    #region COMPONENETS
    public GameObject burst;
    public Transform burstPoint;
    public ParticleSystem explosion;
    public PatrolRoute patrolPath;
    Transform aoePoint;
    Transform hitPoint;
    #endregion

    [Header("Unique Variables")]
    #region VARIABLES
    public float aoeRadius;
    public float hitRadius;
    public float attackDistance;
    public float attackDelay = 1f;
    public float pursueDelay;
    public float suspicionTime;
    public float pursueSpeed;
    public int attackDamage;
    public float waypointTolerance;
    public float waypointWaitTime;
    #endregion

    #region INTERNAL PARAMETERS
    private int currentWaypointIndex;
    private float pursueDelayTimer;
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    private bool discharging;
    #endregion


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
        aoePoint = transform.GetChild(0).transform;
        hitPoint = transform.GetChild(1).transform;
        currentWaypointIndex = 0;
        transform.position = GetCurrentWaypoint();


        if (patrolPath == null)
            state_ = STATE.IDLE;
        else
            state_ = STATE.PATROL;

        discharging = false;

        if (useGlobalSettings)
        {
            SetUp();
        }
        currentHealth = maxHealth;

    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();
        timeSinceArrivedAtWaypoint += Time.deltaTime;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        Gizmos.DrawWireSphere(aoePoint.position, aoeRadius);

        Gizmos.DrawWireSphere(transform.position, sightDistance);

    }

    protected override void Decide()
    {

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
            if (distanceToPlayer > attackDistance && !discharging)
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
            if (!discharging)
                StartCoroutine(AoeSlam());
        }

    }

    IEnumerator AoeSlam()
    {

        Stop();
        discharging = true;

        yield return new WaitForSeconds(1f);

        //explosion.Play();

        yield return new WaitForSeconds(1f);

        //Instantiate(burst, burstPoint.position, Quaternion.Euler(90, 0, 0));
        if (Physics.CheckSphere(aoePoint.position, aoeRadius, playerMask))
        {
            player.GetComponent<playerController>().Stun();
            print("player stunned");
        }
        if (Physics.CheckSphere(hitPoint.position, hitRadius, playerMask))
        {
            player.GetComponent<playerController>().TakeDamage(attackDamage);
            print("player damaged");
        }

        yield return new WaitForSeconds(5f);

        discharging = false;

    }

    private bool CanAttack()
    {
        if (distanceToPlayer < attackDistance)
            return true;
        else
            return false;
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

    void SetUp()
    {
        sightDistance = blackboard.JonesSightDistance;
        attackDistance = blackboard.JonesAttackDistance;
        maxHealth = blackboard.JonesMaxHealth;
        initialSpeed = blackboard.JonesInitialSpeed;
        pursueSpeed = blackboard.JonesPursueSpeed;
        attackDamage = blackboard.JonesAttackDamage;
        pursueDelay = blackboard.JonesPusueDelay;
        suspicionTime = blackboard.JonesSuspicionTime;
        aoeRadius = blackboard.JonesAoeRadius;
        waypointTolerance = blackboard.JonesWaypointTolerance;
        waypointWaitTime = blackboard.JonesWaypointWaitTime;
    }
}
