﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

public class playerController : MonoBehaviour
{
    //public GameObject player;
    public Animator anim;

    //ENEMY DETECT
    [Header("- Player Attack Point/Radius & Enemy Layer")]
    public Transform detectPoint;
    public Transform aoePoint;
    public LayerMask enemyLayer;
    public float attackRadius = .5f;
    public float aoeRadius = 5f;

    #region COMBAT_VARIABLES
    //COMBAT

    int combos = 0;
    int combosHeavy = 0;
    float lastClick = 0f;

    [Header("- Attack Delays")]
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float heavyDelay = 1f;
    public float dischargeDelay = 2f;
    public float nextAttack = 2f;
    float nextCombo = 0f;
    public bool attacking = false;

    [Header("- Attack Damage")]
    public int slashDamage = 20;
    public int slash2Damage = 25;
    public int heavyDamage = 40;
    public int heavy2Damage = 40;
    public int dischargeDamage = 40;

    //DODGE
    [Header("- Dodge Cooldown")]
    public float dodgeCooldown = 1f;
    public float dodgeDashBoost = 4f;

    [Range(.1f, 1f)]
    public float axisThreshold = .1f;


    float dodgeCd = 0;

    //DISCHARGE
    [Header("- Discharge Mechanic")]
    public float maxCharge = 100f;
    public float chargeRate = 10f;
    public float UIChargeMultiplier = 2f;
    public ParticleSystem explosion;
    float currentCharge = 0f;
    float lastCharge = 0f;
    public Image swordFill;
    bool canDischarge = false;
    public cameraShake camShake;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    public ParticleSystem electricityCharge;
    public ParticleSystem burst;
    #endregion

    //TESTING VARS
    protected float originSpeed;


    private void Start()
    {
        //ORIGINAL MOVEMENT SPEED SET
        originSpeed = gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed;
        
        //HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        #region UI/VFX

        //UI SWORD CHARGE
        if (currentCharge > lastCharge)
        {
            lastCharge += Time.deltaTime * UIChargeMultiplier;
            swordFill.fillAmount = lastCharge / maxCharge;
        }


       





        #endregion

        #region ATTACKS

        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;

        //CHECK FOR LAST TIME ATTACKED
        if (lastClick <= 0)
        {
            //LIGHT ATTACK
            if (Input.GetButtonDown("Fire1") && (combos == 0))
            {
                lastClick = attackDelay1;
                combos = 1;
                Slash();
                
                nextCombo = nextAttack;

            }

            //HEAVY ATTACK
            if (Input.GetButtonDown("Fire2") /*&& (combosHeavy == 0)*/)
            {
                lastClick = heavyDelay;
                //combosHeavy = 1;
                heavyAttack();

                //nextCombo = nextAttack;
            }

            //DISCHARGE
            if (Input.GetButtonDown("discharge") && canDischarge)
            {
                lastClick = dischargeDelay;
                Discharge();
                
            }

        }

        //LIGHT ATTACK COMBO
        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 1))
            {
                lastClick = attackDelay2;
                combos = 0;
                Slash2();
            }

           
        }

        //RESET TIMERS AND COMBOS
        if (nextCombo <= 0)
        {
            combos = 0;
            combosHeavy = 0;

        }

        #endregion

        #region DODGE

        dodgeCd -= Time.deltaTime;

        float inputZ = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        //CHECK FOR LAST TIME DODGED
        if (dodgeCd <=0)
        {
            if (Input.GetButtonDown("Dodge") && inputX < -axisThreshold)
            {
                dodgeCd = dodgeCooldown;
                DodgeLeft();
            }

            if (Input.GetButtonDown("Dodge") && inputX > axisThreshold)
            {
                dodgeCd = dodgeCooldown;
                DodgeRight();
            }

            if (Input.GetButtonDown("Dodge") && inputZ < -axisThreshold)
            {
                dodgeCd = dodgeCooldown;
                DodgeBack();
            }

            if (Input.GetButtonDown("Dodge") && inputZ > axisThreshold)
            {
                dodgeCd = dodgeCooldown;
                DodgeRoll();
            }
        }
        #endregion

    }



    #region DodgingFunctions

    //FUNCTIONS FOR DODGING. DIRECTION IS SET IN UPDATE
    void DodgeLeft()
    {
        StartCoroutine("Dash");
        anim.SetTrigger("dodgingLeft");
    }

    void DodgeRight()
    {
        StartCoroutine("Dash");
        anim.SetTrigger("dodgingRight");
    }

    void DodgeBack()
    {
        StartCoroutine("Dash");
        anim.SetTrigger("dodgingBack");
    }

    void DodgeRoll()
    {
        StartCoroutine("Dash");
        anim.SetTrigger("dodgingRoll");
    }

    //TEMPORARILY INCREASES PLAYER SPEED WHILE DODGING
    IEnumerator Dash ()
    {
        

        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;

        yield return new WaitForSeconds(.7f);

        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = originSpeed;

    }

    #endregion

    #region AttackingFunctions

    
    void Slash()
    {
        anim.SetTrigger("isSlash");
        DPS(slashDamage);


    }

    void Slash2()
    {
        anim.SetTrigger("isSlash2");
        DPS(slash2Damage);
        
    }

    
    void heavyAttack()
    {
        anim.SetTrigger("isHeavy");
        StartCoroutine("heavyAtt");
        
    }

    //HEAVY ATTACK DELAY AND DAMAGE APPLICATION
    IEnumerator heavyAtt ()
    {
        yield return new WaitForSeconds(1f);
        DPS(heavyDamage);
       
    }

    //void heavyAttack2()
    //{
    //    anim.SetTrigger("isHeavy2");
    //    DPS(heavy2Damage);
       
    //}

    //FUNCTION FOR DISCHARGE ATTACK. TEMPORARILY STOPS ALL PLAYER MOVEMENT
    void Discharge ()
    {
        anim.SetTrigger("discharge");
        

        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;
        StartCoroutine("explode");
        canDischarge = false;
        swordFill.fillAmount = 0;
        currentCharge = 0;
        lastCharge = 0;
    }

    //DISCHARGE ATTACK DELAY AND DAMAGE APPLICATION. PLAYER MOVEMENT SET TO NORMAL
    IEnumerator explode()
    {
        yield return new WaitForSeconds(.4f);
        burst.Play();

        yield return new WaitForSeconds(1.3f);

        electricityCharge.Stop();

        StartCoroutine(camShake.Shake(shakeDuration, shakeMagnitude));

        explosion.Play();

        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;

        Collider[] hitEnemies = Physics.OverlapSphere(aoePoint.position, aoeRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            //Rigidbody rb = enemy.GetComponent<Rigidbody>();

            //float force = 1001f;
            //float up = 1001f;
            //rb.AddExplosionForce(force, aoePoint.position, aoeRadius, up);

            enemy.GetComponent<enemyController>().takeDamage(dischargeDamage);
            
        }
        
    }
    #endregion




    //DAMAGE APPLICATION

    void DPS (int damageDone)
    {

        //ENEMY DETECT
        Collider[] hitEnemies = Physics.OverlapSphere(detectPoint.position, attackRadius, enemyLayer);

        //APPLY DPS
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<enemyController>().takeDamage(damageDone);
            Charge();
        }
    }

    //SWORD CHARGE
    void Charge()
    {
        if (currentCharge < maxCharge) currentCharge += chargeRate;
        else if (currentCharge >= maxCharge)
        {
            currentCharge = maxCharge;
            canDischarge = true;
            electricityCharge.Play();

        }

    }

    //VISUALS FOR THE AREAS OF ATTACK
    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null || aoePoint == null) return;

        Gizmos.DrawWireSphere(detectPoint.position, attackRadius);
        Gizmos.DrawWireSphere(aoePoint.position, aoeRadius);



    }
}
