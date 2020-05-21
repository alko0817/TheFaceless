using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class meleeIndicator : MonoBehaviour
{
    MeleeEnemy controller;
    public Image ind;
    [Range(.008f, .5f)]
    public float rate;
    Color color;

    private void Start()
    {
        controller = GetComponent<MeleeEnemy>();

        //ind.fillAmount = 1f;
        //ind.enabled = false;
        color = Color.white;
        color.a = 0f;
        ind.color = color;
    }

    private void Update()
    {
        //FILL METHOD 
        //ind.enabled = controller.attackThrown;

        //if (controller.attackThrown)
        //{
        //    ind.fillAmount += rate;
        //}

        //else ind.fillAmount = 0f;


        //FLASH IN METHOD
        //ind.enabled = controller.attackThrown;

        //FADE IN METHOD
        if (controller.attackThrown)
        {
            color.a += Time.deltaTime;
        }

        else color.a = 0f;

        ind.color = color;
    }
}
