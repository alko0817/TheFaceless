using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using UnityEditor.UIElements;

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
    bool isHeavy = false;

    //TIMERS 
    int combos = 0;
    float lastClick = 0;

    int heavyComb = 0;

    float attackDelay1;
    float attackDelay2;
    float attackDelay3;
    float attackDelay4;

    float heavyDelay1;

    float dischargeDelay;

    float blockAttackDelay1;

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

    //DAMAGES
    int slashDamage;
    int slash2Damage;
    int slash3Damage;
    int slash4Damage;

    int heavyDamage;

    int dischargeDamage;

    int blockAttack1Dmg;

    public ParticleSystem heavySlash;

    #endregion  

    private void Start()
    {
        controller = GetComponent<playerController>();
        sounds = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();

        //CLICK TIMERS
        combos = controller.combos;
        lastClick = controller.lastClick;

        attackDelay1 = controller.attackDelay1;
        attackDelay2 = controller.attackDelay2;
        attackDelay3 = controller.attackDelay3;
        attackDelay4 = controller.attackDelay4;

        heavyDelay1 = controller.heavyDelay1;
        dischargeDelay = controller.dischargeDelay;

        blockAttackDelay1 = controller.blockAttackDelay1;

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

        //DAMAGES
        slashDamage = controller.slashDamage;
        slash2Damage = controller.slash2Damage;
        slash3Damage = controller.slash3Damage;
        slash4Damage = controller.slash4Damage;

        heavyDamage = controller.heavyDamage;
        dischargeDamage = controller.dischargeDamage;

        blockAttack1Dmg = controller.blockAttack1Dmg;


    }

    private void Update()
    {
        controller.attacking = attacking;
        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;

        if (controller.stunned || controller.health.dead)
        {
            StopCoroutine("Blocking");
            controller.blocking = false;
            controller.anim.SetBool("blocking", false);
            gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;

            return;
        }
        

        #region Attacks&Discharge
        //CHECK FOR LAST TIME ATTACKED
        if (lastClick <= 0 && !holding && !controller.blocking && !controller.dodging)
        {


            //LIGHT ATTACK
            if (Input.GetButtonUp("Fire1") && (combos == 0))
            {
                combos = 1;
                Attack(hitLight1, attackDelay1, slashDamage, "isSlash", controller.detectPoint, nextAttack, controller.LAStamCost);
                StartCoroutine(AttackSound(hitLight1, controller.lightAttack1Sound));

                nextCombo = nextAttack;

            }



            //ELECTRIC DISCHARGE
            if (Input.GetButton("discharge") && controller.canDischarge && controller.GetComponent<vThirdPersonMotor>().isGrounded)
            {
                isDischarge = true;
                controller.canDischarge = false;
                Attack(hitDischarge, dischargeDelay, dischargeDamage, "discharge", 
                    controller.aoePoint, nextAttack, 0);

                StartCoroutine(Discharge());
            }

            //FROST DISCHARGE
            if (Input.GetKeyDown(KeyCode.R))
            {
                Attack(hitDischarge, dischargeDelay, dischargeDamage, "slam",
                    controller.frostPoint, nextAttack, 0);
                StartCoroutine(FrostSlam());
            }

        }

        //COMBOS
        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonUp("Fire1") && (combos == 1))
            {
                combos = 2;
                Attack(hitLight2, attackDelay2, slash2Damage, "isSlash2", 
                    controller.detectPoint, nextAttack, controller.LAStamCost);

                StartCoroutine(AttackSound(hitLight2, controller.lightAttack2Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 2))
            {
                combos = 3;
                Attack(hitLight3, attackDelay3, slash3Damage, "isSlash3", 
                    controller.detectPoint, nextAttack, controller.LAStamCost);
                StartCoroutine(AttackSound(hitLight3, controller.lightAttack3Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 3))
            {
                combos = 4;
                Attack(hitLight4, attackDelay4, slash4Damage, "isSlash4", 
                    controller.detectPoint, nextAttack, controller.LAStamCost);
                StartCoroutine(AttackSound(hitLight4, controller.lightAttack4Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 4))
            {
                combos = 0;
                Attack(hitBlock1, blockAttackDelay1, blockAttack1Dmg, "blockAttack", 
                    controller.detectPoint, nextBlockAttack, controller.LAStamCost);
                StartCoroutine(AttackSound(hitBlock1, controller.blockAttack1Sound));
            }
        }

        //RESET COMBOS
        if (nextCombo <= 0)
        {
            combos = 0;
            attacking = false;
            controller.stamina.drainingAtt = false;

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
            heavySlash.Stop();
            heavyComb = 0;
        }

        if (Input.GetButton("Fire1"))
        {

            //ACTUAL HEAVY ATTACK
            if (lastClick <= 0 && holding && !attacking && heavyComb == 0)
            {
                Attack(hitHeavy, heavyDelay1, heavyDamage, "isHeavy", 
                    controller.heavyPoint, nextHeavyAttack, controller.HAStamCost);
                StartCoroutine(AttackSound(hitHeavy, controller.heavyAttackSound));
            }
        }
        #endregion

        #region Block

        if (controller.stamina.canBlock)
        {
            if (Input.GetButtonDown("Fire2"))
            {

                StartCoroutine("Blocking");

            }

            //IF BUTTON RELEASED STOP BLOCKING
            if (Input.GetButtonUp("Fire2"))
            {
                StopCoroutine("Blocking");
                gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
                controller.blocking = false;
                controller.anim.SetBool("blocking", false);
            }

            //WHILE BUTTON IS PRESSED 
            if (Input.GetButton("Fire2"))
            {
                if (controller.blocking)
                {
                    gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.blockingSpeed;
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
        heavySlash.Play();
    }

    IEnumerator Blocking()
    {
        yield return new WaitForSeconds(.2f);
        controller.blocking = true;
        controller.anim.SetBool("blocking", controller.blocking);
        controller.anim.SetTrigger("startBlock");
    }

    IEnumerator EpicLand(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        controller.timeManager.slowmoDuration = duration;
        controller.timeManager.Slowmo();
    }

    IEnumerator AttackSound (float connectDelay, AudioClip sound)
    {
        yield return new WaitForSeconds(connectDelay);
        //sounds.Play(sound, sounds.PlayerEffects);
        controller.SwordSounds.PlayOneShot(sound);
    }

    public void Attack (float connectDelay, float clickDelay, int damage, string animation,
                        Transform AreaOfEffect, float comboTimer, float staminaDrain)
    {
        controller.stamina.drainingAtt = true;
        lastClick = clickDelay;
        controller.anim.SetTrigger(animation);
        attacking = true;
        attackThrown = true;
        StartCoroutine(AttackConnection(connectDelay, damage, AreaOfEffect, staminaDrain));
        nextCombo = comboTimer;

    }

    IEnumerator AttackConnection (float delay, int damage, Transform aoe, float drain)
    {
        yield return new WaitForSeconds(delay);
        controller.stamina.Drain(drain);
        if (!controller.stamina.fullAtt) damage = damage / 3;

        attackThrown = false;
        attacking = false;
        //Collider[] hitEnemies = Physics.OverlapSphere(aoe, aoeRadius, controller.enemyLayer);
        Collider[] hitEnemies = Physics.OverlapBox(aoe.position, aoe.localScale / 2, Quaternion.identity, controller.enemyLayer);
        //APPLY DPS
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyBase>().TakeDamage(damage);
            if (isDischarge)
            {
                if(!enemy.GetComponent<EnemyBase>().GetStunned())
                    enemy.GetComponent<EnemyBase>().SetStunned(true);
            }
            else controller.Charge();
        }

        if (!isDischarge)
        {
            Collider[] walls = Physics.OverlapBox(aoe.position, aoe.localScale/2);
            foreach (Collider wall in walls)
            {
                if (wall.tag == "Map")
                {
                    controller.SwordSounds.PlayOneShot(controller.wallHit);
                }
            }
        }

        isDischarge = false;
    }

    IEnumerator FrostSlam()
    {
        controller.health.Immortality(true);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;
        yield return new WaitForSeconds(.8f);
        controller.timeManager.slowmoDuration = 1f;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(1f);

        controller.health.Immortality(false);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;
    }

    IEnumerator Discharge ()
    {
        
        controller.health.Immortality(true);

        yield return new WaitForSeconds(.7f);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;

        yield return new WaitForSeconds(.3f);

        controller.fullCharge.Stop();
        controller.discharging = true;
        controller.explosion.Play();
        controller.SwordSounds.PlayOneShot(controller.DischargeFirst);

        if(controller.timeManager != null)
        {
            controller.timeManager.slowmoDuration = controller.dischargeSlowDuration;
            controller.timeManager.Slowmo();
        }
        controller.foving.FovOut();

        yield return new WaitForSeconds(.7f);
        controller.electricityCharge.Stop();
        StartCoroutine(controller.camShake.Shake(controller.shakeDuration, controller.shakeMagnitude));
        Instantiate(controller.burst, controller.burstPoint.position, Quaternion.Euler(90,0,0));

        float temp = controller.SwordSounds.volume;
        controller.SwordSounds.volume += .4f;
        controller.SwordSounds.PlayOneShot(controller.DischargeSecond);



        yield return new WaitForSeconds(.8f);
        controller.foving.FovIn();

        controller.SwordSounds.volume = temp;
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
