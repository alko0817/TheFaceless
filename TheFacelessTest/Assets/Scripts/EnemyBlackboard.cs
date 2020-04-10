using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyBlackboard : MonoBehaviour
{
    public int maxNumberOfPursuingEnemies;
    private List<GameObject> enemiesPursuingPlayer;
    private List<GameObject> enemiesInSightOfPlayer;

    // Update is called once per frame
    void Update()
    {
        enemiesPursuingPlayer.Capacity = maxNumberOfPursuingEnemies;
        print(enemiesInSightOfPlayer.Count);
        AddPursuingEnemy();
    }


    private void AddPursuingEnemy()
    {
        if (enemiesPursuingPlayer.Count < enemiesPursuingPlayer.Capacity)
        {
            enemiesPursuingPlayer.Add(enemiesInSightOfPlayer.ElementAt(0));
            enemiesInSightOfPlayer.RemoveAt(0);
        }
        for(int i = 0; i < enemiesPursuingPlayer.Count; i++)
        {
            enemiesPursuingPlayer.ElementAt(i).GetComponent<AIBehaviour>().SetPursuing(true);
        }
    }

    public void RemovePursuingEnemy(GameObject enemy)
    {
        if (enemiesPursuingPlayer.Contains(enemy))
        {
            enemy.GetComponent<AIBehaviour>().SetPursuing(false);
            enemiesPursuingPlayer.Remove(enemy);
        }

    }

    public void AddEnemyInSight(GameObject enemy)
    {
        if (!enemiesInSightOfPlayer.Contains(enemy))
            enemiesInSightOfPlayer.Add(enemy);

    }
    
    public void RemoveEnemyInSight(GameObject enemy)
    {
        if (enemiesInSightOfPlayer.Contains(enemy))
            enemiesInSightOfPlayer.Remove(enemy);

    }
}
