using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStamina : MonoBehaviour
{
    Image bar;

    /// <summary>
    /// Base stamina unit. Is clamped between 0 - 1
    /// </summary>
    internal float unit;

    [Tooltip("Rate at which stamina depletes while sprinting")]
    public float sprintDepleteRate = .5f;
    [Tooltip("Rate at which stamina recharges")]
    public float rechargeRate = 1f;
    [Tooltip("Delay before stamina starts recharging")]
    public float chargeDelay = .5f;

    Animator anim;
    bool animate = false;
    bool tired = false;


    playerController controller;
    internal bool canBlock = true;
    internal bool canSprint = true;
    internal bool canRecharge = true;
    internal bool fullAtt = true;

    internal bool drainingSprint = false;
    internal bool channeling = false;
    internal bool draining = false;
    float timer = 1f;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        bar = GetComponent<Image>();
        anim = GetComponent<Animator>();
        unit = 1f;

    }

    //if you want to translate a value between 0 - 1, just fucking multiple it by the max no. of the other scale. The other scale MUST start at 0

    private void Update()
    {
        unit = Mathf.Clamp01(unit);
        bar.fillAmount = unit;
        draining = drainCheck();
        timer -= Time.deltaTime;
        if (timer < 0f) channeling = false;

        if (controller.sprinting && canSprint)
        {
            draining = true;
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
            fullAtt = false;
            canBlock = false;
        }

        if (unit <= .2f)
        {
            tired = true;
        }

        #region shitCode
        if (tired)
        {
            if (!animate)
            {
                animate = true;
                anim.SetTrigger("play");
                anim.SetBool("tired", true);
            }

            if (unit >= .6f)
            {
                tired = false;
                animate = false;
                anim.SetBool("tired", false);
            }
            
        }
        #endregion



        if (!draining)
        {
            StartCoroutine(ChargeDelay());
        }
        else StopCoroutine(ChargeDelay());


        if (canRecharge)
        {
            Regen();
        }

    }
    private bool drainCheck()
    {
        if (!channeling && !drainingSprint) return false;
        else return true;
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
        if (unit > .7f)
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
        timer = 1.2f;
        channeling = true;
        canRecharge = false;
        unit -= drain;
    }
}
