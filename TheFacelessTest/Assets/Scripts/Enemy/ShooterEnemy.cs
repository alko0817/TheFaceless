using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : EnemyBase
{

    [Header("Unique Properties")]
    Vector3 initialPosition;
    Transform projectileSpawn;
    public GameObject projectile;
    public GameObject[] projectiles;

    #region BOOLEANS
    private bool shooting;
    #endregion

    #region TIME & TIMERS
    private float shootTimer;
    private float fleeTimer;
    public float fleeTime;
    public float fireRate;
    #endregion

    
    public float fleeDistance;

    public float fleeSpeed;


    protected override void Start()
    {
        base.Start();
        shootTimer = 0f;
        fleeTimer = Mathf.Infinity;
        shooting = false;

        projectileSpawn = transform.GetChild(0).transform;

        initialPosition = transform.position;

        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i] = Instantiate(projectile);

            projectiles[i].transform.position = projectileSpawn.position;
            projectiles[i].transform.rotation = projectileSpawn.rotation;
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
        base.Sense();
        if (distanceToPlayer < fleeDistance)
        {
            fleeTimer = 0f;
        }

    }

    protected override void Decide()
    {
        base.Decide();
        if (fleeTimer < fleeTime)
        {
            state_ = STATE.FLEE;
        }
        else if (playerDetected)
        {
            state_ = STATE.IN_COMBAT;
        }
        else
        {
            state_ = STATE.IDLE;
        }



    }

    protected override void Act()
    {
        base.Act();
        if(state_ == STATE.IN_COMBAT)
        {
            Shoot();
        }

        if(state_ == STATE.FLEE)
        {
            Flee();
        }

        if(state_ == STATE.IDLE)
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
        if (shootTimer > fireRate)
        {
            shootTimer = 0f;


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

    void Guard()
    {
        Vector3 vectorToInitialPos = initialPosition - transform.position;
        float dist = vectorToInitialPos.magnitude;

        if (Math.Abs(dist) > 2f)
            MoveTo(initialPosition, initialSpeed);
        else
            Stop();
    }
}
