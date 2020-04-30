using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerAttack : MonoBehaviour
{
    playerController controller;
    audioManager sounds;

    #region COMBAT-VARIABLES
    
    //CHECKS
    bool attacking = false;
    bool attackThrown = false;
    bool holding = false;
    bool isDischarge = false;

    //TIMERS 
    int combos = 0;
    int combosBlock = 0;
    float lastClick = 0;

    float attackDelay1;
    float attackDelay2;
    float attackDelay3;
    float attackDelay4;

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
    float hitLight3;
    float hitLight4;

    float hitHeavy;

    float hitDischarge;

    float hitBlock1;
    float hitBlock2;

    //DAMAGES
    int slashDamage;
    int slash2Damage;
    int slash3Damage;
    int slash4Damage;

    int heavyDamage;

    int dischargeDamage;

    int blockAttack1Dmg;
    int blockAttack2Dmg;

    #endregion  

    private void Start()
    {
        controller = GetComponent<playerController>();
        sounds = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();

        //CLICK TIMERS
        combos = controller.combos;
        combosBlock = controller.combosBlock;
        lastClick = controller.lastClick;

        attackDelay1 = controller.attackDelay1;
        attackDelay2 = controller.attackDelay2;
        attackDelay3 = controller.attackDelay3;
        attackDelay4 = controller.attackDelay4;

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
        hitLight3 = controller.hitLight3;
        hitLight4 = controller.hitLight4;

        hitHeavy = controller.hitHeavy;
        hitDischarge = controller.hitDischarge;

        hitBlock1 = controller.hitBlock1;
        hitBlock2 = controller.hitBlock2;

        //DAMAGES
        slashDamage = controller.slashDamage;
        slash2Damage = controller.slash2Damage;
        slash3Damage = controller.slash3Damage;
        slash4Damage = controller.slash4Damage;

        heavyDamage = controller.heavyDamage;
        dischargeDamage = controller.dischargeDamage;

        blockAttack1Dmg = controller.blockAttack1Dmg;
        blockAttack2Dmg = controller.blockAttack2Dmg;


    }

    private void Update()
    {
        controller.attacking = attacking;
        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;

        #region Attacks&Discharge
        //CHECK FOR LAST TIME ATTACKED
        if (lastClick <= 0 && !holding && !controller.blocking)
        {


            //LIGHT ATTACK
            if (Input.GetButtonUp("Fire1") && (combos == 0))
            {
                combos = 1;
                Attack(hitLight1, attackDelay1, slashDamage, "isSlash", controller.detectPoint.position, controller.attackRadius, nextAttack);
                StartCoroutine(AttackSound(hitLight1, controller.lightAttack1Sound));

                nextCombo = nextAttack;

            }



            //DISCHARGE
            if (Input.GetButton("discharge") && controller.canDischarge && controller.GetComponent<vThirdPersonMotor>().isGrounded)
            {
                isDischarge = true;
                Attack(hitDischarge, dischargeDelay, dischargeDamage, "discharge", controller.aoePoint.position, controller.aoeRadius, nextAttack);

                StartCoroutine(Discharge());
            }

        }

        //COMBOS
        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonUp("Fire1") && (combos == 1))
            {
                combos = 2;
                Attack(hitLight2, attackDelay2, slash2Damage, "isSlash2", controller.detectPoint.position, controller.attackRadius, nextAttack);
                StartCoroutine(AttackSound(hitLight2, controller.lightAttack2Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 2))
            {
                combos = 3;
                Attack(hitLight3, attackDelay3, slash3Damage, "isSlash3", controller.detectPoint.position, controller.attackRadius, nextAttack);
                StartCoroutine(AttackSound(hitLight3, controller.lightAttack3Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 3))
            {
                combos = 0;
                Attack(hitLight4, attackDelay4, slash4Damage, "isSlash4", controller.detectPoint.position, controller.attackRadius, nextAttack);
                StartCoroutine(AttackSound(hitLight4, controller.lightAttack4Sound));
            }
        }

        //RESET COMBOS
        if (nextCombo <= 0)
        {
            combos = 0;
            combosBlock = 0;
            attacking = false;

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
                Attack(hitHeavy, heavyDelay1, heavyDamage, "isHeavy", controller.detectPoint.position, controller.attackRadius, nextHeavyAttack);
                StartCoroutine(AttackSound(hitHeavy, controller.heavyAttackSound));
            }
        }
        #endregion

        #region Block
        if (controller.stamina.canBlock)
        {
            if (Input.GetButtonDown("Fire2"))
            {

                controller.blocking = true;
                controller.anim.SetBool("blocking", controller.blocking);
                controller.anim.SetTrigger("startBlock");

            }

            //IF BUTTON RELEASED STOP BLOCKING
            if (Input.GetButtonUp("Fire2"))
            {
                gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
                controller.blocking = false;
                controller.anim.SetBool("blocking", controller.blocking);
            }

            //WHILE BUTTON IS PRESSED 
            if (Input.GetButton("Fire2"))
            {
                if (controller.blocking)
                {
                    gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.blockingSpeed;
                }
            }

            //IF ATTACKING THROUGH BLOCK
            if (lastClick <= 0 && controller.blocking)
            {
                if (Input.GetButtonDown("Fire1") && (combosBlock == 0))
                {
                    Attack(hitBlock1, blockAttackDelay1, blockAttack1Dmg, "blockAttack", controller.detectPoint.position, controller.attackRadius, nextBlockAttack);
                    StartCoroutine(AttackSound(hitBlock1, controller.blockAttack1Sound));
                    controller.anim.SetBool("blocking", !controller.blocking);
                    combosBlock = 1;
                    nextCombo = nextBlockAttack;

                }
            }
            //BLOCK ATTACK COMBO
            if (lastClick <= 0 && nextCombo > 0)
            {
                if (Input.GetButtonDown("Fire1") && (combosBlock == 1))
                {
                    Attack(hitBlock2, blockAttackDelay2, blockAttack2Dmg, "blockAttack2", controller.detectPoint.position, controller.attackRadius, nextBlockAttack);
                    StartCoroutine(AttackSound(hitBlock2, controller.blockAttack2Sound));
                    controller.anim.SetBool("blocking", !controller.blocking);
                    combosBlock = 0;


                }
            }
        }

        else gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
        #endregion
    }

    IEnumerator Holding()
    {
        yield return new WaitForSeconds(holdForHeavy);
        holding = true;
    }

    IEnumerator AttackSound (float connectDelay, AudioClip sound)
    {
        yield return new WaitForSeconds(connectDelay);
        //sounds.Play(sound, sounds.PlayerEffects);
        controller.SwordSounds.PlayOneShot(sound);
    }

    public void Attack (float connectDelay, float clickDelay, int damage, string animation,Vector3 AreaOfEffect, float aoeRadius, float comboTimer)
    {
        lastClick = clickDelay;
        controller.anim.SetTrigger(animation);
        attacking = true;
        attackThrown = true;
        StartCoroutine(AttackConnection(connectDelay, damage, AreaOfEffect, aoeRadius));
        nextCombo = comboTimer;

    }

    IEnumerator AttackConnection (float delay, int damage, Vector3 aoe, float aoeRadius)
    {
        yield return new WaitForEndOfFrame();
        attackThrown = false;
        yield return new WaitForSeconds(delay);

        Collider[] hitEnemies = Physics.OverlapSphere(aoe, aoeRadius, controller.enemyLayer);

        //APPLY DPS
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<AIBehaviour>().TakeDamage(damage);

            controller.Charge();
            if (isDischarge)
            {
                enemy.GetComponent<AIBehaviour>().SetStunned(true);
                isDischarge = false;
            }
        }
    }

    IEnumerator Discharge ()
    {
        
        controller.health.Immortality(true);
        controller.canDischarge = false;

        yield return new WaitForSeconds(.7f);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;

        yield return new WaitForSeconds(.3f);

        controller.fullCharge.Stop();
        controller.discharging = true;
        controller.explosion.Play();
        controller.SwordSounds.PlayOneShot(controller.DischargeFirst);

        if(controller.timeManager != null)
        {
            controller.timeManager.GetComponent<TimeManager>().slowmoDuration = controller.dischargeSlowDuration;
            controller.timeManager.Slowmo();
        }
        controller.foving.FovOut();

        yield return new WaitForSeconds(.7f);
        controller.electricityCharge.Stop();
        StartCoroutine(controller.camShake.Shake(controller.shakeDuration, controller.shakeMagnitude));

        //controller.burst.Play();
        Instantiate(controller.burst, controller.burstPoint.position, Quaternion.Euler(90,0,0));

        //sounds.Play("Discharge_Second", sounds.PlayerEffects);
        controller.SwordSounds.PlayOneShot(controller.DischargeSecond);

        

        yield return new WaitForSeconds(.8f);
        controller.foving.FovIn();
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;

        controller.health.Immortality(false);

        controller.discharging = false;
    }

    public bool GetAttacking ()
    {
        return attackThrown;
    }
}
