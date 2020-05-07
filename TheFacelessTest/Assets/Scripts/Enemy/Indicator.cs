using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject[] bolts;
    EnemyBase controller;

    private void Start()
    {
        controller = GetComponentInParent<EnemyBase>();
    }

    private void Update()
    {
        //if (!controller.shoot)
        //{
        //    foreach (GameObject bolt in bolts)
        //    {
        //        bolt.SetActive(false);
        //    }

        //}

        //if (controller.shooting)
        //{
        //    if (controller.shootTimer >= 0.1f) bolts[0].SetActive(true);

        //    if (controller.shootTimer >= 1.2f) bolts[1].SetActive(true);

        //    if (controller.shootTimer >= controller.fireRate - .3f) bolts[2].SetActive(true);
        //}
    }
}
