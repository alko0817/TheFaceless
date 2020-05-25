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
    bool CanReact = true;

    //TIMERS 
    int combos = 0;
    float lastClick = 0;
    float blockCd = 0;

    int heavyComb = 0;

    float attackDelay1;
    float attackDelay2;
    float attackDelay3;
    float attackDelay4;
    float attackDelay5;

    float parryDelay;

    float heavyDelay1;

    float dischargeDelay;


    float nextAttack;
    float nextHeavyAttack;
    float nextParry;
    float nextCombo;

    float holdForHeavy;

    //CONNECT TIMERS
    float hitLight1;
    float hitLight2;
    float hitLight3;
    float hitLight4;
    float hitLight5;

    float hitParry;

    float hitHeavy;

    float hitDischarge;


    //DAMAGES
    int slashDamage;
    int slash2Damage;
    int slash3Damage;
    int slash4Damage;
    int slash5Damage;

    int parryDamage;

    int heavyDamage;

    int dischargeDamage;
    #endregion  

    public ParticleSystem heavySlash;
    public ParticleSystem parryDeflect;

    public float dischargeForce = 100f;
    public LayerMask movables;
    public static bool blocker = false;

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

        parryDelay = controller.parryDelay;

        heavyDelay1 = controller.heavyDelay1;
        dischargeDelay = controller.dischargeDelay;

        attackDelay5 = controller.attackDelay5;

        nextAttack = controller.nextAttack;
        nextHeavyAttack = controller.nextHeavyAttack;
        nextParry = controller.nextParry;
        nextCombo = controller.nextCombo;
        holdForHeavy = controller.holdForHeavy;

        //CONNECT TIMERS
        hitLight1 = controller.hitLight1;
        hitLight2 = controller.hitLight2;
        hitLight3 = controller.hitLight3;
        hitLight4 = controller.hitLight4;
        hitLight5 = controller.hitLight5;

        hitParry = controller.hitParry;

        hitHeavy = controller.hitHeavy;
        hitDischarge = controller.hitDischarge;


        //DAMAGES
        slashDamage = controller.slashDamage;
        slash2Damage = controller.slash2Damage;
        slash3Damage = controller.slash3Damage;
        slash4Damage = controller.slash4Damage;
        slash5Damage = controller.slash5Damage;

        parryDamage = controller.ParryDamage;

        heavyDamage = controller.heavyDamage;
        dischargeDamage = controller.dischargeDamage;


    }
    
    private void Update()
    {
        controller.attacking = attacking;
        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;
        blockCd -= Time.deltaTime;

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
                Attack(hitLight1, attackDelay1, slashDamage, "isSlash", controller.detectPoint, nextAttack, controller.LightCost);
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

            //FIRE DISCHARGE
            if (blocker /*Input.GetKeyDown(KeyCode.R)*/)
            {
                Attack(hitDischarge, dischargeDelay, dischargeDamage, "slam",
                    controller.aoePoint, nextAttack, 0);
                StartCoroutine(Flame());
            }

        }

        //COMBOS
        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonUp("Fire1") && (combos == 1))
            {
                combos = 2;
                Attack(hitLight2, attackDelay2, slash2Damage, "isSlash2", 
                    controller.detectPoint, nextAttack, controller.LightCost);

                StartCoroutine(AttackSound(hitLight2, controller.lightAttack2Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 2))
            {
                combos = 3;
                Attack(hitLight3, attackDelay3, slash3Damage, "isSlash3", 
                    controller.detectPoint, nextAttack, controller.LightCost);
                StartCoroutine(AttackSound(hitLight3, controller.lightAttack3Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 3))
            {
                combos = 4;
                Attack(hitLight4, attackDelay4, slash4Damage, "isSlash4", 
                    controller.detectPoint, nextAttack, controller.LightCost);
                StartCoroutine(AttackSound(hitLight4, controller.lightAttack4Sound));
            }

            else if (Input.GetButtonUp("Fire1") && (combos == 4))
            {
                combos = 0;
                Attack(hitLight5, attackDelay5, slash5Damage, "blockAttack", 
                    controller.detectPoint, nextAttack, controller.LightCost);
                StartCoroutine(AttackSound(hitLight5, controller.blockAttack1Sound));
            }
        }

        //RESET COMBOS
        if (nextCombo <= 0)
        {
            combos = 0;
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
            GetComponent<vThirdPersonMotor>().stopMove = false;
            holding = false;
            heavySlash.Stop();
            heavyComb = 0;
        }

        if (Input.GetButton("Fire1"))
        {

            //ACTUAL HEAVY ATTACK
            if (lastClick <= 0 && holding && !attacking)
            {
                Attack(hitHeavy, heavyDelay1, heavyDamage, "isHeavy", 
                    controller.heavyPoint, nextHeavyAttack, controller.HeavyCost);
                StartCoroutine(AttackSound(.45f, controller.heavyAttackSound));
            }
        }
        #endregion

        #region ParryBlock
        if (controller.stamina.canBlock)
        {
            if (blockCd <= 0 && !holding && CanReact && !controller.discharging)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    StartCoroutine("Blocking");
                    blockCd = 1.1f;
                }
            }

            if (controller.blocking)
            {
                Collider[] enemies = Physics.OverlapBox(controller.heavyPoint.position,
                    controller.heavyPoint.localScale / 2, Quaternion.identity, controller.enemyLayer);

                foreach (Collider enemy in enemies)
                {
                    if (enemy.GetComponent<MeleeEnemy>() == null) continue;
                    
                    bool attacked = enemy.GetComponent<MeleeEnemy>().attackThrown;
                    if (attacked && CanReact)
                    {
                        CanReact = false;
                        StartCoroutine(Parrying(.2f, .3f, false));
                        Attack(hitParry, parryDelay, parryDamage, "react", controller.detectPoint, nextParry, controller.parryCost);
                        StartCoroutine(AttackSound(.1f, controller.ParrySound));
                        StartCoroutine(AttackSound(hitParry, controller.ParryAttackSound));
                        break;
                    }
                    
                }
            }
            else CanReact = true;
        }

        #endregion
    }

    IEnumerator Holding()
    {
        yield return new WaitForSeconds(holdForHeavy);
        holding = true;
        GetComponent<vThirdPersonMotor>().stopMove = true;
        heavySlash.Play();
    }

    IEnumerator Blocking()
    {
        controller.anim.SetTrigger("startBlock");
        GetComponent<vThirdPersonMotor>().stopMove = true;
        controller.health.Immortality(true);
        controller.blocking = true;
        controller.stamina.Drain(controller.blockCost);

        yield return new WaitForSeconds(.1f);
        controller.blocking = false;

        yield return new WaitForSeconds(.5f);
        GetComponent<vThirdPersonMotor>().stopMove = false;

        yield return new WaitForSeconds(.4f);
        controller.health.Immortality(false);
    }

    IEnumerator Parrying(float delay, float duration, bool slow)
    {
        yield return new WaitForSeconds(delay);
        parryDeflect.Play();
        if (slow)
        {
            controller.timeManager.slowmoDuration = duration;
            controller.timeManager.Slowmo();
        }
        yield return new WaitUntil(() => !GetComponent<vThirdPersonMotor>().stopMove);

        GetComponent<vThirdPersonMotor>().stopMove = true;
        yield return new WaitForSeconds(.7f);
        GetComponent<vThirdPersonMotor>().stopMove = false;
    }


    
    IEnumerator AttackSound (float connectDelay, AudioClip sound)
    {
        yield return new WaitForSeconds(connectDelay);
        //sounds.Play(sound, sounds.PlayerEffects);
        controller.SwordSounds.PlayOneShot(sound);
    }


    /// <summary>
    /// Throws and casts an attack for the player.
    /// </summary>
    /// <param name="connectDelay"></param>
    /// <param name="clickDelay"></param>
    /// <param name="damage"></param>
    /// <param name="animation"></param>
    /// <param name="AreaOfEffect"></param>
    /// <param name="comboTimer"></param>
    /// <param name="staminaDrain"></param>
    public void Attack (float connectDelay, float clickDelay, int damage, string animation,
                        Transform AreaOfEffect, float comboTimer, float staminaDrain)
    {
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

    IEnumerator Flame()
    {

        controller.health.Immortality(true);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;
        yield return new WaitForSeconds(.8f);
        controller.timeManager.slowmoDuration = 1f;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(.6f);
        StartCoroutine(controller.camShake.Shake(controller.shakeDuration, controller.shakeMagnitude));
        yield return new WaitForSeconds(.4f);
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

        Collider[] props = Physics.OverlapSphere(controller.aoePoint.position, controller.aoeRadius, movables);

        foreach (Collider prop in props)
        {
            Rigidbody rb = prop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(dischargeForce, transform.position, controller.aoeRadius, 1f);
            }
        }


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
