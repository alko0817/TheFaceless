using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : EnemyBase
{
    [Header("Unique Components")]
    [Header("- Unique Properties -")]
    #region COMPONENETS
    public GameObject projectile;
    public GameObject[] projectiles;
    Transform projectileSpawn;
    #endregion

    [Header("Unique Variables")]
    #region VARIABLES
    public float fleeDistance;
    public float fleeSpeed;
    public int damage;
    public float fleeTime;
    public float fireRate;
    public AudioClip shot;
    #endregion

    #region INTERNAL PARAMETERS
    internal float shootTimer;
    private float fleeTimer;
    internal bool shooting;
    #endregion

    protected override void Start()
    {
        base.Start();
        shootTimer = 0f;
        fleeTimer = Mathf.Infinity;
        shooting = false;

        projectileSpawn = transform.GetChild(0).transform;

        initialPosition = transform.position;

        if(useGlobalSettings)
        {
            SetUp();
        }

        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i] = Instantiate(projectile);

            projectiles[i].transform.position = projectileSpawn.position;
            projectiles[i].transform.rotation = projectileSpawn.rotation;
            projectiles[i].GetComponent<Projectile>().SetDamage(damage);
            projectiles[i].SetActive(false);
        }


    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(transform.position, sightDistance);
        Gizmos.DrawWireSphere(transform.position, fleeDistance);

    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();
        fleeTimer += Time.deltaTime;
    }

    protected override void Sense()
    {
        if (distanceToPlayer < sightDistance && IsPlayerVisible())
        {
            lastKnownPlayerLocation = player.transform.position;

            timeSinceLastSawPlayer = 0.0f;

            playerDetected = true;

        }
        else
        {
            playerDetected = false;
        }

        if (distanceToPlayer < fleeDistance)
        {
            fleeTimer = 0f;
        }

    }

    protected override void Decide()
    {
        base.Decide();
        if (!stunned)
        {
            if (fleeTimer < fleeTime)
            {
                state_ = STATE.FLEE;
            }
            else if (playerDetected && canHitPlayer)
            {
                state_ = STATE.IN_COMBAT;
            }
            else
            {
                state_ = STATE.IDLE;
            }
        }


    }

    protected override void Act()
    {
        base.Act();
        if (state_ == STATE.IN_COMBAT)
        {
            Shoot();
        }

        if (state_ == STATE.FLEE)
        {
            Flee();
        }

        if (state_ == STATE.IDLE)
        {
            Guard();
        }
    }

    void Shoot()
    {
        Stop();
        transform.LookAt(player.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        shootTimer += Time.deltaTime;
        if (shootTimer > (fireRate / 3) * 2 && !shooting)
        {
            shooting = true;
            anim.Shoot();
        }

        if (shootTimer > fireRate)
        {
            shootTimer = 0f;
            shooting = false;
            audioSource.PlayOneShot(shot);
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (!projectiles[i].activeInHierarchy)
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

    void Flee()
    {
        Vector3 direction = (player.transform.position - transform.position) * -1;
        direction.Normalize();
        Vector3 destination = transform.position + direction;
        MoveTo(destination, fleeSpeed);

    }

    void SetUp()
    {
        sightDistance = blackboard.ShooterSightDistance;
        maxHealth = blackboard.ShooterMaxHealth;
        initialSpeed = blackboard.ShooterInitialSpeed;
        fleeSpeed = blackboard.ShooterFleeSpeed;
        fleeTime = blackboard.ShooterFleeTime;
        fleeDistance = blackboard.ShooterFleeDistance;
        damage = blackboard.ShooterDamage;
        fireRate = blackboard.ShooterFireRate;
        stunDuration = blackboard.StunDuration;
    }
}
