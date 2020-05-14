using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStamina : MonoBehaviour
{
    //public Image bar;
    RectTransform anchor;

    Quaternion max;
    Quaternion minim;

    internal float unit;

    [Tooltip("Rate at which stamina depletes while blocking")]
    public float blockDepleteRate = 1f;
    [Tooltip("Rate at which stamina depletes while sprinting")]
    public float sprintDepleteRate = .5f;
    [Tooltip("Rate at which stamina recharges")]
    public float rechargeRate = 1f;
    [Tooltip("Delay before stamina starts recharging")]
    public float chargeDelay = .5f;

    public Image fatigue;
    Animator anim;
    bool tired = false;


    playerController controller;
    internal bool canBlock = true;
    internal bool canSprint = true;
    internal bool canRecharge = true;
    internal bool fullAtt = true;

    internal bool drainingBlock = false;
    internal bool drainingSprint = false;
    internal bool drainingDodge = false;
    internal bool drainingAtt = false;

    private void Start()
    {
        max = Quaternion.Euler(0, 180, 180f);
        minim = Quaternion.Euler(0, 180, 0f);
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        anim = GetComponent<Animator>();
        unit = 1f;
        //bar.fillAmount = 1f;

        anchor = GetComponent<RectTransform>();
        anchor.rotation = max;

    }

    //if you want to return a value between 0 - 1, just fucking multiple it by the max no. of the other scale. The other scale MUST start at 0

    private void Update()
    {
        unit = Mathf.Clamp01(unit);
        anchor.rotation = Quaternion.Euler(0, 180f, unit * 180f);

        if (controller.blocking && canBlock && !controller.attacking)
        {
            
            
            drainingBlock = true;
            StopCoroutine(ChargeDelay());
            canRecharge = false;
            if (unit > 0)
            {
                unit -= Time.deltaTime * blockDepleteRate;
            }

            if (unit == 0)
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
            if (unit > 0)
            {
                unit -= Time.deltaTime * sprintDepleteRate;
            }

            if (unit == 0)
            {
                canSprint = false;
                drainingSprint = false;
            }
        }
        else drainingSprint = false;

        if (unit <= 0 && fullAtt)
        {
            tired = true;
            fullAtt = false;
            drainingAtt = false;
        }

        if (!drainingBlock && !drainingSprint && !drainingDodge && !drainingAtt)
        {
            StartCoroutine(ChargeDelay());
        }


        if (canRecharge)
        {
            Regen();
        }

        #region shitCode
        if (tired)
        {
            anim.SetTrigger("tired");
            tired = false;
        }
        #endregion
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
        unit = Mathf.Clamp01(unit + (Time.deltaTime * rechargeRate));
        if (unit > .6f)
        {
            canBlock = true;
            canSprint = true;
            fullAtt = true;
        }

    }


    /// <summary>
    /// Drain an amount of stamina
    /// </summary>
    /// <param name="drain"></param>
    /// <returns></returns>
    public void Drain(float drain)
    {
        canRecharge = false;
        unit -= drain;

    }
}
