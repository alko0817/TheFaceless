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


    private void Start()
    {

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
