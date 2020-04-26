using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public GameObject[] anchors;
    public GameObject[] enemies;
    public Material[] indicators;
    AIBehaviour enemyController;
    internal Color intensity;
    
    private void Start()
    {
        intensity.r = 1f;
        intensity.g = 1f;
        foreach(Material element in indicators) element.SetColor("_EmissionColor", intensity);
    }

    private void Update()
    {
        #region OldCodeSnippet
        //detected = Physics.CheckSphere(detectPoint.position, detectRange, controller.enemyLayer);

        //if (detected)
        //{
        //    Collider[] enemies = Physics.OverlapSphere(detectPoint.position, detectRange, controller.enemyLayer);
        //    int i = 0;
        //    foreach (Collider enemy in enemies)
        //    {
        //        Track(enemy, anchors[i], indicators[i]);
        //        if (enemy.GetComponent<AIBehaviour>().dying)
        //        {
        //            Untrack(anchors[i]);
        //        }
        //        i++;
        //        if (i >= anchors.Length) break;

        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < anchors.Length; i++)
        //    {
        //        anchors[i].SetActive(false);
        //    }
        //}
        #endregion

        int i = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Track(enemy, anchors[i], indicators[i]);
            }

            else if (enemy == null || enemy.GetComponent<AIBehaviour>().dying) Untrack(anchors[i], indicators[i]);
            i++;
        }

        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (other.gameObject == enemies[i]) break;
                
                if (enemies[i] == null)
                {
                    enemies[i] = other.gameObject;
                    break;
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       for (int i = 0; i < enemies.Length; i++)
       {
            if (other.gameObject == enemies[i])
            {
                enemies[i] = null;
                break;
            }
       }
    }

    void Track(GameObject target, GameObject anchor, Material indicator)
    {
        //START TRACKING
        anchor.SetActive(true);
        anchor.transform.LookAt(target.transform);

        //TRACK ENEMY SHOT CHARGE
        enemyController = target.GetComponent<AIBehaviour>();
        intensity.g = Mathf.Abs((enemyController.shootTimer / enemyController.fireRate) - 1);
        indicator.SetColor("_EmissionColor", intensity);
    }

    void Untrack(GameObject anchor, Material indicator)
    {
        //RESET COLORS AND STOP TRACKING
        intensity.g = 1f;
        indicator.SetColor("_EmissionColor", intensity);
        anchor.SetActive(false);
    }
}
