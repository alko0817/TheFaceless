using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStamina : MonoBehaviour
{
    public Image bar;
    [Tooltip("Rate at which stamina depletes while blocking")]
    public float blockDepleteRate = 1f;
    [Tooltip("Rate at which stamina depletes while sprinting")]
    public float sprintDepleteRate = .5f;
    [Tooltip("Rate at which stamina recharges")]
    public float rechargeRate = 1f;
    [Tooltip("Delay before stamina starts recharging")]
    public float chargeDelay = .5f;

    playerController controller;
    internal bool canBlock = true;
    internal bool canSprint = true;
    internal bool canRecharge = true;
    internal bool drainingBlock = false;
    internal bool drainingSprint = false;
    internal bool drainingDodge = false;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        bar.fillAmount = 1f;
    }

    private void Update()
    {

        if (controller.blocking && canBlock && !controller.attacking)
        {
            
            
            drainingBlock = true;
            StopCoroutine(ChargeDelay());
            canRecharge = false;
            if (bar.fillAmount > 0)
            {
                bar.fillAmount -= Time.deltaTime * blockDepleteRate;
            }

            if (bar.fillAmount == 0)
            {
                canBlock = false;
                controller.blocking = false;
                drainingBlock = false;
                controller.anim.SetBool("blocking", false);
            }

        }
        else drainingBlock = false;

        if (controller.sprinting && canSprint)
        {
            
            
            drainingSprint = true;
            StopCoroutine(ChargeDelay());
            canRecharge = false;
            if (bar.fillAmount > 0)
            {
                bar.fillAmount -= Time.deltaTime * sprintDepleteRate;
            }

            if (bar.fillAmount == 0)
            {
                canSprint = false;
                drainingSprint = false;
            }
        }
        else drainingSprint = false;

        if (!drainingBlock && !drainingSprint && !drainingDodge) //((!controller.blocking && !canRecharge) || (controller.blocking && controller.attacking) || (!controller.sprinting && !canRecharge))
        {
            StartCoroutine(ChargeDelay());
        }


        if (canRecharge)
        {
            Regen();
        }

        
    }

    IEnumerator ChargeDelay ()
    {
        if (!canRecharge)
        {
            yield return new WaitForSeconds(chargeDelay);
            canRecharge = true;
        }

    }

    public void Regen()
    {
        bar.fillAmount = Mathf.Clamp01(bar.fillAmount + (Time.deltaTime * rechargeRate));
        if (bar.fillAmount > 0)
        {
            canBlock = true;
            canSprint = true;
        }

    }
}
