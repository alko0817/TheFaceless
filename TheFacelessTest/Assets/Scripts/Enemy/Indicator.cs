using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject[] bolts;
    ShooterEnemy controller;

    private void Start()
    {
        controller = GetComponentInParent<ShooterEnemy>();
    }

    private void Update()
    {
        if (!controller.shooting)
        {
            foreach (GameObject bolt in bolts)
            {
                bolt.SetActive(false);
            }

        }

        if (controller.shooting)
        {
            if (controller.shootTimer >= (controller.fireRate / 3) * 2) bolts[0].SetActive(true);

            if (controller.shootTimer >= (controller.fireRate / 3) * 2.5f) bolts[1].SetActive(true);

            if (controller.shootTimer >= controller.fireRate - .2f) bolts[2].SetActive(true);
        }
    }
}
