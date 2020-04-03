using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

public class playerController : MonoBehaviour
{
    public GameObject player;
    public Animator anim;

    //ENEMY DETECT
    [Header("Player Attack Point/Radius & Enemy Layer")]
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

    [Header("Attack Delays")]
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float heavyDelay = 1f;
    public float nextAttack = 2f;
    float nextCombo = 0f;
    public bool attacking = false;

    [Header("Attack Damage")]
    public int slashDamage = 20;
    public int slash2Damage = 25;
    public int heavyDamage = 40;
    public int heavy2Damage = 40;
    public int dischargeDamage = 40;

    //DODGE
    [Header("Dodge Cooldown")]
    public float dodgeCooldown = 1f;
    public float dodgeDashBoost = 4f;

    [Range(.1f, 1f)]
    public float axisThreshold = .1f;


    float dodgeCd = 0;

    //DISCHARGE
    [Header("Discharge Mechanic")]
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
    #endregion

    //TESTING VARS
    protected float originSpeed;


    private void Start()
    {
        originSpeed = gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        #region UI/VFX
        if (currentCharge > lastCharge)
        {
            lastCharge += Time.deltaTime * UIChargeMultiplier;
            swordFill.fillAmount = lastCharge / maxCharge;
        }

        if (canDischarge) electricityCharge.Play();
        else electricityCharge.Stop();

       





        #endregion

        #region ATTACKS

        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;

        if (lastClick <= 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 0))
            {
                lastClick = attackDelay1;
                combos = 1;
                Slash();
                
                nextCombo = nextAttack;

            }

            if (Input.GetButtonDown("Fire2") /*&& (combosHeavy == 0)*/)
            {
                lastClick = heavyDelay;
                //combosHeavy = 1;
                heavyAttack();

                //nextCombo = nextAttack;
            }

            if (Input.GetButtonDown("discharge") && canDischarge)
            {
                lastClick = attackDelay1;
                Discharge();
                
            }

        }

        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 1))
            {
                lastClick = attackDelay2;
                combos = 0;
                Slash2();
            }

           
        }

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



    #region AnimationFunctions

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

    IEnumerator Dash ()
    {
        //float originSpeed = gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed;

        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;

        yield return new WaitForSeconds(.7f);

        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = originSpeed;

    }



    void Slash2()
    {
        anim.SetTrigger("isSlash2");
        DPS(slash2Damage);
       // Charge();
        
    }

    void Slash ()
    {
        anim.SetTrigger("isSlash");
        DPS(slashDamage);
        //Charge();

    }
    void heavyAttack()
    {
        anim.SetTrigger("isHeavy");
        StartCoroutine("heavyAtt");
        
    }

    IEnumerator heavyAtt ()
    {
        yield return new WaitForSeconds(1f);
        DPS(heavyDamage);
       // Charge();
    }

    void heavyAttack2()
    {
        anim.SetTrigger("isHeavy2");
        DPS(heavy2Damage);
        //Charge();
    }

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

    IEnumerator explode()
    {
        yield return new WaitForSeconds(1.7f);

        StartCoroutine(camShake.Shake(shakeDuration, shakeMagnitude));

        explosion.Play();

        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;

        Collider[] hitEnemies = Physics.OverlapSphere(aoePoint.position, aoeRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<enemyController>().takeDamage(dischargeDamage);
            
        }
        
    }
    #endregion


    void Charge()
    {
        if (currentCharge < maxCharge) currentCharge += chargeRate;
        else if (currentCharge >= maxCharge)
        {
            currentCharge = maxCharge;
            canDischarge = true;

        }

    }


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




    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null || aoePoint == null) return;

        Gizmos.DrawWireSphere(detectPoint.position, attackRadius);
        Gizmos.DrawWireSphere(aoePoint.position, aoeRadius);



    }
}
