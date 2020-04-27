using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStamina : MonoBehaviour
{
    public Image bar;
    [Tooltip("Rate at which stamina depletes while blocking")]
    public float depleteRate = 1f;
    [Tooltip("Rate at which stamina recharges")]
    public float rechargeRate = 1f;
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

        if (controller.blocking && canBlock && !controller.attacking)
        {
            canRecharge = false;
            if (bar.fillAmount > 0)
            {
                bar.fillAmount -= Time.deltaTime * depleteRate;
            }

            if (bar.fillAmount == 0)
            {
                canBlock = false;
                controller.blocking = false;
                controller.anim.SetBool("blocking", false);
            }

        }
        
        if ((!controller.blocking && !canRecharge) || (controller.blocking && controller.attacking))
        {
            StartCoroutine(Delay());
        }


        if (canRecharge)
        {
            Regen();
        }


    }

    IEnumerator Delay ()
    {
        yield return new WaitForSeconds(chargeDelay);
        canRecharge = true;

    }

    public void Regen()
    {
        bar.fillAmount = Mathf.Clamp01(bar.fillAmount + (Time.deltaTime * rechargeRate));
        if (bar.fillAmount > 0) canBlock = true;
    }
}
