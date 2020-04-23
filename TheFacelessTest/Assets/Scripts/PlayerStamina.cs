using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStamina : MonoBehaviour
{
    public Image bar;
    [Tooltip("Rate at which stamina depletes while blocking")]
    public float deplete = 1f;
    [Tooltip("Rate at which stamina recharges")]
    public float recharge = 1f;
    [Tooltip("Delay before stamina starts recharging")]
    public float chargeDelay = .5f;

    playerController controller;
    internal bool canBlock = true;
    internal bool canRecharge = true;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        bar.fillAmount = 1f;
    }

    private void Update()
    {

        if (controller.blocking && canBlock)
        {
            canRecharge = false;
            if (bar.fillAmount > 0)
            {
                bar.fillAmount -= Time.deltaTime * deplete;
            }

            if (bar.fillAmount == 0)
            {
                canBlock = false;
                controller.blocking = false;
                controller.anim.SetBool("blocking", false);
            }

        }
        
        if (!controller.blocking && !canRecharge)
        {
            StartCoroutine(Delay());
        }


        if (canRecharge)
        {
            bar.fillAmount = Mathf.Clamp01(bar.fillAmount + (Time.deltaTime * recharge));
            if (bar.fillAmount > 0) canBlock = true;
        }


    }

    IEnumerator Delay ()
    {
        yield return new WaitForSeconds(chargeDelay);
        canRecharge = true;

    }
}
