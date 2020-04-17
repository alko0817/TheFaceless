using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

public class playerController : MonoBehaviour
{
    public Animator anim;
    PlayerHealth health; 

    //ENEMY DETECT
    [Header("- Player Attack Point/Radius & Enemy Layer")]
    public Transform detectPoint;
    public Transform aoePoint;
    [Tooltip("By default, this needs to be 'Enemy'")]
    public LayerMask enemyLayer;
    [Tooltip("Radius at which damage is applied by simple attacks. Requires fine-tunning!")]
    public float attackRadius = .5f;
    [Tooltip("Area of effect for the Discharge attack")]
    public float aoeRadius = 5f;

    [Header("Slow Motion & Camera FX")]
    public TimeManager timeManager;
    public camFov foving;

    #region COMBAT_VARIABLES
    //COMBAT

    int combos = 0;
    int combosHeavy = 0;
    int combosBlock = 0;
    float lastClick = 0f;

    [Header("- Attack Intervals")]
    [Tooltip("Input delay for the first light attack")]
    public float attackDelay1 = 1.5f;
    [Tooltip("Input delay for the second light attack")]
    public float attackDelay2 = 1.5f;
    [Tooltip("Inpute delay for the heavy attack")]
    public float heavyDelay1 = 1f;
    public float heavyDelay2 = 1f;
    [Tooltip("Input delay for the discharge attack")]
    public float dischargeDelay = 2f;
    public float blockAttackDelay1 = 2f;
    public float blockAttackDelay2 = 2f;
    [Tooltip("Input timer before next light combo")]
    public float nextAttack = 2f;
    [Tooltip("Input timer before next heavy combo")]
    public float nextHeavyAttack = 2f;
    [Tooltip("Input timer before next block combo")]
    public float nextBlockAttack = 2f;
    float nextCombo = 0f;

    [Tooltip("How long has the player to hold the button before triggering the heavy attack. Requires fine-tunning!")]
    [Range(.1f, 1f)]
    public float holdForHeavy = .2f;

    float heavyHold = 0;
    bool holding = false;
    bool attacking = false;

    [Header("- Attack Damage")]
    public int slashDamage = 20;
    public int slash2Damage = 25;
    public int heavyDamage = 40;
    public int heavy2Damage = 40;
    public int dischargeDamage = 40;
    public int blockAttack1Dmg = 30;
    public int blockAttack2Dmg = 30;

    //DODGE
    [Header("- Dodge Cooldown")]
    [Tooltip("Input delay before player can dodge again")]
    public float dodgeCooldown = 1f;
    [Tooltip("Temporary movement speed boost while dodging")]
    public float dodgeDashBoost = 4f;

    [Tooltip("How long has the player to hold directional buttons to trigger the dodge. Requires fine-tunning!")]
    [Range(.1f, 1f)]
    public float axisThreshold = .1f;

    float dodgeCd = 0;

    //BLOCK-PARRY
    [Header("Blocking/Parrying")]
    [Tooltip("Player movement speed while blocking")]
    public float blockingSpeed = 2f;

    [HideInInspector]
    public bool blocking = false;

    float blockHold = 0f;


    //DISCHARGE
    [Header("- Discharge Mechanic")]
    public float maxCharge = 100f;
    [Tooltip("How fast the sword actually charges")]
    public float chargeRate = 10f;
    [Tooltip("How fast the UI updates the charge")]
    public float UIChargeMultiplier = 2f;
    public ParticleSystem explosion;
    float currentCharge = 0f;
    float lastCharge = 0f;
    Image swordFill;

    [HideInInspector]
    public bool canDischarge = false;

    public cameraShake camShake;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    [Tooltip("Slow motion duration")]
    public float dischargeSlowDuration = 2f;
    public ParticleSystem electricityCharge;
    public ParticleSystem burst;

    //SOUNDS
    [Header("Sound Delays")]
    public float lightAttack1 = 0.4f;
    public float lightAttack2 = 0.6f;
    public float heavyAttack1 = 1.2f;
    public float blockAttack1 = 1f;
    public float blockAttack2 = 1f;
    public float enemyHit = .2f;

    [Header("Sound Clips")]
    [Tooltip("Copy-paste the clip name from the Audio Manager")]
    public string lightAttack1Sound;
    public string lightAttack2Sound;
    public string heavyAttackSound;
    public string blockAttack1Sound;
    public string blockAttack2Sound;
    public string enemyHitSound;
    public string[] otherSounds;
    public int otherSoundsIndex;
    


    #endregion

    //TESTING VARS
    protected float originSpeed;
    protected float originTimeReset;
    
    

    private void Start()
    {
        //ORIGINAL MOVEMENT SPEED SET
        originSpeed = gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed;
        
        //HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //OTHER STUFF
        swordFill = GameObject.Find("swordChargeFill").GetComponent<Image>();

        //ORIGINAL TIME RESET RATE
        originTimeReset = timeManager.GetComponent<TimeManager>().slowmoDuration;

        health = GameObject.Find("stateOfHealth").GetComponent<PlayerHealth>();
        //currentHealth = maxHealth;
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
        if (lastClick <= 0 && !holding && !blocking)
        {


            //LIGHT ATTACK
            if (Input.GetButtonUp("Fire1") && (combos == 0))
            {
                lastClick = attackDelay1;
                combos = 1;
                Slash();

                nextCombo = nextAttack;

            }



            //DISCHARGE
            if (Input.GetButton("discharge") && canDischarge)
            {
                lastClick = dischargeDelay;
                Discharge();


            }


            //OLD HEAVY ATTACK
            //if (Input.GetButtonDown("Fire2") && combosHeavy == 0)
            //{
            //    lastClick = heavyDelay1;
            //    combosHeavy++;
            //    heavyAttack();
            //    nextCombo = nextHeavyAttack;

            //}





        }

        //COMBOS
        if (lastClick <= 0 && nextCombo > 0)
        {
            //LIGHT
            if (Input.GetButtonUp("Fire1") && (combos == 1))
            {
                lastClick = attackDelay2;
                combos = 0;
                Slash2();
            }

            //HEAVY
            //if (Input.GetButtonDown("Fire2") && combosHeavy == 1)
            //{
            //    lastClick = heavyDelay2;
            //    combosHeavy++;
            //    heavyAttack2();
            //}


        }

        //RESET COMBOS
        if (nextCombo <= 0)
        {
            combos = 0;
            combosHeavy = 0;
            combosBlock = 0;
            //if (blocking)
            //{
            //    anim.SetTrigger("startBlock");
            //    anim.SetBool("blocking", blocking);
            //}

        }

        #region Some_weird_shit_that_idk_why_works

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine("SmallEnum");

        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine("SmallEnum");
            holding = false;
            heavyHold = 0;
        }

        if (Input.GetButton("Fire1"))
        {

            //ACTUAL HEAVY ATTACK
            if (lastClick<=0 && holding && !attacking)
            {
                heavyHold += Time.deltaTime;
                heavyAttack();

                attacking = true;
            }

            //ADD COMBOS HERE
        }
        #endregion

        #endregion

        #region DODGE

        dodgeCd -= Time.deltaTime;

        float inputZ = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        //CHECK FOR LAST TIME DODGED
        if (dodgeCd <= 0)
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

        #region BLOCK

        //CHECK FOR BUTTON PRESS
        if (Input.GetButtonDown("Fire2"))
        {

            blocking = true;
            anim.SetBool("blocking", blocking);
            anim.SetTrigger("startBlock");


        }

        //IF BUTTON RELEASED STOP BLOCKING
        if (Input.GetButtonUp("Fire2"))
        {
            gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = originSpeed;
            blocking = false;
            blockHold = 0;
            anim.SetBool("blocking", blocking);
        }

        //WHILE BUTTON IS PRESSED 
        if (Input.GetButton("Fire2"))
        {
            
            if (blocking)
            {
                blockHold += Time.deltaTime;
                gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = blockingSpeed;
            }
        }

        //IF ATTACKING THROUGH BLOCK
        if (lastClick <= 0 && blocking)
        {
            if (Input.GetButtonDown("Fire1") && (combosBlock == 0))
            {
                lastClick = blockAttackDelay1;
                combosBlock = 1;


                BlockAttack1();

                //jump attack
                nextCombo = nextBlockAttack;

            }
        }
        //BLOCK ATTACK COMBO
        if (lastClick <= 0 && nextCombo > 0 && blocking)
        {
            if (Input.GetButtonDown("Fire1") && (combosBlock == 1))
            {
                lastClick = blockAttackDelay2;
                combosBlock = 0;

                BlockAttack2();
                //swirle attack

            }
        }
        #endregion
    }

    IEnumerator SmallEnum ()
    {
        yield return new WaitForSeconds(holdForHeavy);
        holding = true;
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

    void BlockAttack1 ()
    {
        anim.SetTrigger("blockAttack");
        anim.SetBool("blocking", false);
        DPS(blockAttack1Dmg);
        StartCoroutine(AttackConnect(blockAttack1, blockAttack1Sound));
    }

    void BlockAttack2()
    {
        anim.SetTrigger("blockAttack2");
        DPS(blockAttack1Dmg);
        StartCoroutine(AttackConnect(blockAttack2, blockAttack2Sound));
    }

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
        
        anim.SetTrigger("isHeavy");
        StartCoroutine("heavyAtt");
        StartCoroutine(AttackConnect(heavyAttack1, heavyAttackSound));

    }
    IEnumerator heavyAtt()
    {
        yield return new WaitForSeconds(1f);
        DPS(heavyDamage);
        attacking = false;
        //yield return new WaitForSeconds(.2f);
        //heavyStart = false;
        //anim.SetBool("secondHeavy", heavyStart);

    }

    void heavyAttack2()
    {

        anim.SetTrigger("isHeavy2");
        StartCoroutine("heavyAtt2");
        StartCoroutine(AttackConnect(heavyAttack1, heavyAttackSound));

    }

    //HEAVY ATTACK DELAY AND DAMAGE APPLICATION
    
    IEnumerator heavyAtt2()
    {
        yield return new WaitForSeconds(1f);
        DPS(heavyDamage);
        
        //anim.SetBool("secondHeavy", heavyStart);

    }




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

        if (timeManager != null)
        {
            timeManager.GetComponent<TimeManager>().slowmoDuration = dischargeSlowDuration;
            timeManager.Slowmo();
        }


        //Put sound here for when the character "loads" the Discharge.

        foving.FovOut();

        yield return new WaitForSeconds(1.3f);

        

        electricityCharge.Stop();

        StartCoroutine(camShake.Shake(shakeDuration, shakeMagnitude));
        burst.Play();
        

        // Put sound here for when the character smashes the ground.
        FindObjectOfType<audioManager>().Play("Discharge_Second");


        

        Collider[] hitEnemies = Physics.OverlapSphere(aoePoint.position, aoeRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<AIBehaviour>().SetStunned(true);
            enemy.GetComponent<AIBehaviour>().TakeDamage(dischargeDamage);
            
        }

        yield return new WaitForSeconds(.8f);
        foving.FovIn();
        yield return new WaitForSeconds(1f);
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

    public void TakeDamage(int damage)
    {
        health.Damage(damage);
    }
}
