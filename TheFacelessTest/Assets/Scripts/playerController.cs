using System.Collections;
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

    [Header("Slow Motion & Camera FX")]
    public TimeManager timeManager;
    public camFov foving;

    #region COMBAT_VARIABLES
    //COMBAT

    int combos = 0;
    int combosHeavy = 0;
    float lastClick = 0f;

    [Header("- Attack Intervals")]
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float heavyDelay = 1f;
    public float dischargeDelay = 2f;
    public float nextAttack = 2f;
    float nextCombo = 0f;
    float holdClick = 0;
    [Range(.1f, 1f)]
    public float holdFor = .2f;
    //bool holding = false;
    //bool attacked = true;
    //bool canHeavy = true;

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

    //SOUNDS
    [Header("Sound Delays")]
    public float lightAttack1 = 0.4f;
    public float lightAttack2 = 0.6f;
    public float heavyAttack1 = 1.2f;
    public float enemyHit = .2f;

    [Header("Sound Clips")]
    [Tooltip("Copy-paste the clip name from the Audio Manager")]
    public string lightAttack1Sound;
    public string lightAttack2Sound;
    public string heavyAttackSound;
    public string enemyHitSound;
    public string[] otherSounds;
    public int otherSoundsIndex;
    


    #endregion

    //TESTING VARS
    protected float originSpeed;
    float clickHold = 0;

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
            if (Input.GetButtonUp("Fire1") && (combos == 0))
            {
                lastClick = attackDelay1;
                combos = 1;
                Slash();
                
                nextCombo = nextAttack;

            }

            //NEW HEAVY ATTACK
               

            //if (Input.GetButton("Fire1") && canHeavy)
            //{
            //    attacked = false;
            //    holdClick += Time.deltaTime;
            //    heavyAttack();

            //}

            //DISCHARGE
            if (Input.GetButton("discharge") && canDischarge)
            {
                lastClick = dischargeDelay;
                Discharge();

                
            }

            //OLD HEAVY ATTACK
            if (Input.GetButtonDown("Fire2") /*&& (combosHeavy == 0)*/)
            {
                lastClick = heavyDelay;
                //combosHeavy = 1;
                heavyAttack();

                //nextCombo = nextAttack;
            }

        }

       // if (Input.GetButtonUp("Fire1")) holdClick = 0;

        //LIGHT ATTACK COMBO
        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonUp("Fire1") && (combos == 1))
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
        StartCoroutine(AttackConnect(lightAttack1, lightAttack1Sound));
        

    }

    

    void Slash2()
    {
        anim.SetTrigger("isSlash2");        
        DPS(slash2Damage);
        StartCoroutine(AttackConnect(lightAttack2, lightAttack2Sound));

    }


    void heavyAttack()
    {
        //if (holdClick > holdFor && !attacked)
        //{
        //    Debug.LogError("holding " + holdClick);
        //    anim.SetTrigger("isHeavy");
        //    //StartCoroutine("heavyAtt");
        //    attacked = true;
        //    holdClick = 0;
        //}

        anim.SetTrigger("isHeavy");
        StartCoroutine("heavyAtt");
        StartCoroutine(AttackConnect(heavyAttack1, heavyAttackSound));

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
        
        explosion.Play();
        FindObjectOfType<audioManager>().Play("Discharge_First");

        timeManager.Slowmo();

        //Put sound here for when the character "loads" the Discharge.

        foving.FovIn();

        yield return new WaitForSeconds(1.3f);

        foving.FovOut();

        electricityCharge.Stop();

        StartCoroutine(camShake.Shake(shakeDuration, shakeMagnitude));
        burst.Play();
        

        // Put sound here for when the character smashes the ground.
        FindObjectOfType<audioManager>().Play("Discharge_Second");


        

        Collider[] hitEnemies = Physics.OverlapSphere(aoePoint.position, aoeRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {

            enemy.GetComponent<AIBehaviour>().TakeDamage(dischargeDamage);
            
        }

        yield return new WaitForSeconds(1.8f);
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;
    }

    IEnumerator AttackConnect(float delay, string clip)
    {
        yield return new WaitForSeconds(delay);

        if (clip == null)
        {
            yield return null;
            Debug.LogWarning("No clip"); 
        }

        else FindObjectOfType<audioManager>().Play(clip);


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
            StartCoroutine(AttackConnect(enemyHit, enemyHitSound));

            enemy.GetComponent<AIBehaviour>().TakeDamage(damageDone);
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
            //Put Sound here if you want to play a sound when sword is fully charged!

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
