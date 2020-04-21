using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerAttack : MonoBehaviour
{
    playerController controller;

    #region COMBAT-VARIABLES
    
    //CHECKS
    bool attacking = false;
    bool holding = false;
    bool blocking = false;

    //TIMERS 
    int combos = 0;
    int combosBlock = 0;
    float lastClick = 0;

    float attackDelay1;
    float attackDelay2;

    float heavyDelay1;

    float dischargeDelay;

    float blockAttackDelay1;
    float blockAttackDelay2;

    float nextAttack;
    float nextHeavyAttack;
    float nextBlockAttack;
    float nextCombo;

    float holdForHeavy;

    //CONNECT TIMERS
    float hitLight1;
    float hitLight2;
    float hitHeavy;
    float hitDischarge;
    float hitBlock1;
    float hitBlock2;

    //DAMAGES
    int slashDamage;
    int slash2Damage;
    int heavyDamage;
    int dischargeDamage;
    int blockAttack1Dmg;
    int blockAttack2Dmg;

    #endregion  

    private void Start()
    {
        controller = GetComponent<playerController>();

        //CLICK TIMERS
        combos = controller.combos;
        combosBlock = controller.combosBlock;
        lastClick = controller.lastClick;
        attackDelay1 = controller.attackDelay1;
        attackDelay2 = controller.attackDelay2;
        heavyDelay1 = controller.heavyDelay1;
        dischargeDelay = controller.dischargeDelay;
        blockAttackDelay1 = controller.blockAttackDelay1;
        blockAttackDelay2 = controller.blockAttackDelay2;
        nextAttack = controller.nextAttack;
        nextHeavyAttack = controller.nextHeavyAttack;
        nextBlockAttack = controller.nextBlockAttack;
        nextCombo = controller.nextCombo;
        holdForHeavy = controller.holdForHeavy;

        //CONNECT TIMERS
        hitLight1 = controller.hitLight1;
        hitLight2 = controller.hitLight2;
        hitHeavy = controller.hitHeavy;
        hitDischarge = controller.hitDischarge;
        hitBlock1 = controller.hitBlock1;
        hitBlock2 = controller.hitBlock2;

        //DAMAGES
        slashDamage = controller.slashDamage;
        slash2Damage = controller.slash2Damage;
        heavyDamage = controller.heavyDamage;
        dischargeDamage = controller.dischargeDamage;
        blockAttack1Dmg = controller.blockAttack1Dmg;
        blockAttack2Dmg = controller.blockAttack2Dmg;


    }

    private void Update()
    {
        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;
        controller.blocking = blocking;

        #region Attacks&Discharge
        //CHECK FOR LAST TIME ATTACKED
        if (lastClick <= 0 && !holding && !blocking)
        {


            //LIGHT ATTACK
            if (Input.GetButtonUp("Fire1") && (combos == 0))
            {
                combos = 1;
                Attack(hitLight1, attackDelay1, slashDamage, "isSlash", controller.detectPoint.position, controller.attackRadius);

                nextCombo = nextAttack;

            }



            //DISCHARGE
            if (Input.GetButton("discharge") && controller.canDischarge)
            {
                Attack(hitDischarge, dischargeDelay, dischargeDamage, "discharge", controller.aoePoint.position, controller.aoeRadius);
                StartCoroutine(Discharge());
            }

        }

        //COMBOS
        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonUp("Fire1") && (combos == 1))
            {
                combos = 0;
                Attack(hitLight2, attackDelay2, slash2Damage, "isSlash2", controller.detectPoint.position, controller.attackRadius);
            }
        }

        //RESET COMBOS
        if (nextCombo <= 0)
        {
            combos = 0;
            combosBlock = 0;

        }
        #endregion

        #region HeavyAttack

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine("Holding");

        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine("Holding");
            holding = false;
        }

        if (Input.GetButton("Fire1"))
        {

            //ACTUAL HEAVY ATTACK
            if (lastClick <= 0 && holding && !attacking)
            {
                Attack(hitHeavy, heavyDelay1, heavyDamage, "isHeavy", controller.detectPoint.position, controller.attackRadius);
            }
        }
        #endregion

        #region Block
        if (Input.GetButtonDown("Fire2"))
        {

            blocking = true;
            controller.anim.SetBool("blocking", blocking);
            controller.anim.SetTrigger("startBlock");


        }

        //IF BUTTON RELEASED STOP BLOCKING
        if (Input.GetButtonUp("Fire2"))
        {
            gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
            blocking = false;
            controller.anim.SetBool("blocking", blocking);
        }

        //WHILE BUTTON IS PRESSED 
        if (Input.GetButton("Fire2"))
        {

            if (blocking)
            {
                gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.blockingSpeed;
            }
        }

        //IF ATTACKING THROUGH BLOCK
        if (lastClick <= 0 && blocking)
        {
            if (Input.GetButtonDown("Fire1") && (combosBlock == 0))
            {
                Attack(hitBlock1, blockAttackDelay1, blockAttack1Dmg, "blockAttack", controller.detectPoint.position, controller.attackRadius);
                controller.anim.SetBool("blocking", !blocking);
                combosBlock = 1;
                nextCombo = nextBlockAttack;

            }
        }
        //BLOCK ATTACK COMBO
        if (lastClick <= 0 && nextCombo > 0 && blocking)
        {
            if (Input.GetButtonDown("Fire1") && (combosBlock == 1))
            {
                Attack(hitBlock2, blockAttackDelay2, blockAttack2Dmg, "blockAttack2", controller.detectPoint.position, controller.attackRadius);
                controller.anim.SetBool("blocking", !blocking);
                combosBlock = 0;


            }
        }
        #endregion

        if (nextCombo < 0)
        {
            combos = 0;
            combosBlock = 0;
        }

    }

    IEnumerator Holding()
    {
        yield return new WaitForSeconds(holdForHeavy);
        holding = true;
    }

    public void Attack (float connectDelay, float clickDelay, int damage, string animation,Vector3 AreaOfEffect, float aoeRadius)
    {
        lastClick = clickDelay;
        controller.anim.SetTrigger(animation);
        attacking = true;
        StartCoroutine(AttackConnection(connectDelay, damage, AreaOfEffect, aoeRadius));

    }

    IEnumerator AttackConnection (float delay, int damage, Vector3 aoe, float aoeRadius)
    {
        yield return new WaitForSeconds(delay);

        Collider[] hitEnemies = Physics.OverlapSphere(aoe, aoeRadius, controller.enemyLayer);

        //APPLY DPS
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<AIBehaviour>().TakeDamage(damage);
            controller.Charge();
        }

        attacking = false;
    }

    IEnumerator Discharge ()
    {
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;

        yield return new WaitForSeconds(.4f);
        controller.explosion.Play();
        FindObjectOfType<audioManager>().Play("Discharge_First");
        if(controller.timeManager != null)
        {
            controller.timeManager.GetComponent<TimeManager>().slowmoDuration = controller.dischargeSlowDuration;
            controller.timeManager.Slowmo();
        }
        controller.foving.FovOut();

        yield return new WaitForSeconds(1.3f);
        controller.electricityCharge.Stop();
        StartCoroutine(controller.camShake.Shake(controller.shakeDuration, controller.shakeMagnitude));
        controller.burst.Play();
        FindObjectOfType<audioManager>().Play("Discharge_Second");
        yield return new WaitForSeconds(.8f);
        controller.foving.FovIn();
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;


        controller.canDischarge = false;
        controller.swordFill.fillAmount = 0;
        controller.currentCharge = 0;
        controller.lastCharge = 0;
    }

    public bool GetAttacking ()
    {
        return attacking;
    }


}
