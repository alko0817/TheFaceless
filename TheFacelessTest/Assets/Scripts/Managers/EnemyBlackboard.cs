using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyBlackboard : MonoBehaviour
{
    public int maxNumberOfPursuingEnemies;
    public List<GameObject> enemiesPursuingPlayer;
    public List<GameObject> enemiesInSightOfPlayer;

    MeleeEnemy[] meleeEnemies;
    ShooterEnemy[] shooterEnemies;
    StretchyJones[] stretchies;

    [Header("Variables for the Melee enemies")]
    public float MeleeSightDistance;
    public float MeleeAttackDistance;
    public float MeleeMaxHealth;
    public float MeleeInitialSpeed;
    public float MeleePursueSpeed;
    public int MeleeAttackDamage;
    public int DodgeChanceOutOf10;
    public int BlockChanceOutOf10;
    public float MeleePusueDelay;
    public float MeleeSuspicionTime;
    public float MeleeWaypointTolerance;
    public float MeleeWaypointWaitTime;


    [Header("Variables for the Jones enemies")]
    public float JonesSightDistance;
    public float JonesAttackDistance;
    public float JonesMaxHealth;
    public float JonesInitialSpeed;
    public float JonesPursueSpeed;
    public int JonesAttackDamage;
    public float JonesPusueDelay;
    public float JonesSuspicionTime;
    public float JonesAoeRadius;
    public float JonesWaypointTolerance;
    public float JonesWaypointWaitTime;

    [Header("Variables for the Shooter enemies")]
    public float ShooterSightDistance;
    public float ShooterMaxHealth;
    public float ShooterInitialSpeed;
    public float ShooterFleeSpeed;
    public float ShooterFleeTime;
    [Tooltip("Distance away from the player at which the enemy will start to flee")]
    public float ShooterFleeDistance;
    public int ShooterDamage;
    public float ShooterFireRate;


    private void Start()
    {
        meleeEnemies = GameObject.FindObjectsOfType<MeleeEnemy>();
        shooterEnemies = GameObject.FindObjectsOfType<ShooterEnemy>();
        stretchies = GameObject.FindObjectsOfType<StretchyJones>();


    }
    // Update is called once per frame
    void Update()
    {
        enemiesPursuingPlayer.Capacity = maxNumberOfPursuingEnemies;
        AddPursuingEnemy();



    }




    private void AddPursuingEnemy()
    {
        if (enemiesInSightOfPlayer.Count != 0)
        {
            if (enemiesPursuingPlayer.Count < enemiesPursuingPlayer.Capacity)
            {
                if (!enemiesPursuingPlayer.Contains(enemiesInSightOfPlayer.ElementAt(0)))
                {
                    enemiesPursuingPlayer.Add(enemiesInSightOfPlayer.ElementAt(0));
                    enemiesInSightOfPlayer.RemoveAt(0);
                    enemiesPursuingPlayer.ElementAt(enemiesPursuingPlayer.Count - 1).GetComponent<EnemyBase>().SetEngaging(true);
                }
            }
            
        }
        else
            return;
    }

    public void RemovePursuingEnemy(GameObject enemy)
    {
        if (enemiesPursuingPlayer.Contains(enemy))
        {
            enemy.GetComponent<EnemyBase>().SetEngaging(false);
            enemiesPursuingPlayer.Remove(enemy);
        }
        else
            return;

    }

    public void AddEnemyInSight(GameObject enemy)
    {
        if (!enemiesInSightOfPlayer.Contains(enemy))
            enemiesInSightOfPlayer.Add(enemy);
        else
            return;

    }

    public void RemoveEnemyInSight(GameObject enemy)
    {
        if (enemiesInSightOfPlayer.Contains(enemy))
            enemiesInSightOfPlayer.Remove(enemy);
        else
            return;
    }
}
