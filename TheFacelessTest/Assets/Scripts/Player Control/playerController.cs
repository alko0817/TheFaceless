using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

[System.Serializable]
public class playerController : MonoBehaviour
{
    
    public Animator anim;
    internal PlayerHealth health;
    internal PlayerStamina stamina;
    public GameObject swordTrail;
    [Header("- Player Attack Pointers")]
    public Transform detectPoint;
    public Transform heavyPoint;
    public Transform aoePoint;
    [Tooltip("By default, this needs to be 'Enemy'")]
    public LayerMask enemyLayer;
    [Tooltip("Radius at which damage is applied by simple attacks. Requires fine-tunning!")]
    public float attackRadius = .5f;
    public float heavyRadius = 2f;
    [Tooltip("Area of effect for the Discharge attack")]
    public float aoeRadius = 5f;
    [Space]
    public bool canDie = false;
    [Header("- Slow Motion & Camera FX")]
    internal TimeManager timeManager;
    public camFov foving;

    #region COMBAT_VARIABLES

    internal int combos = 0;
    internal int combosBlock = 0;
    internal float lastClick = 0f;

    [Header("- Attack Intervals")]
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float attackDelay3 = 1.5f;
    public float attackDelay4 = 1.5f;
    [Space]
    public float heavyDelay1 = 1f;
    public float heavyDelay2 = 1f;
    [Space]
    public float dischargeDelay = 2f;
    [Space]
    public float blockAttackDelay1 = 2f;
    public float blockAttackDelay2 = 2f;
    [Space]
    [Tooltip("Input timer before next light combo")]
    public float nextAttack = 2f;
    [Tooltip("Input timer before next heavy combo")]
    public float nextHeavyAttack = 2f;
    [Tooltip("Input timer before next block combo")]
    public float nextBlockAttack = 2f;
    internal float nextCombo = 0f;

    [Header("- Delay before attack lands and applies damage")]
    public float hitLight1 = 0f;
    public float hitLight2 = 0f;
    public float hitLight3 = 0f;
    public float hitLight4 = 0f;
    [Space]
    public float hitHeavy = 0f;
    [Space]
    public float hitDischarge = 0f;
    [Space]
    public float hitBlock1 = 0f;
    public float hitBlock2 = 0f;

    [Tooltip("How long has the player to hold the button before triggering the heavy attack. Requires fine-tunning!")]
    [Range(.1f, 1f)]
    public float holdForHeavy = .2f;

    internal bool holding = false;
    internal bool attacking = false;
    internal bool dodging = false;
    internal bool stunned = false;

    [Header("- Attack Damage")]
    public int slashDamage = 20;
    public int slash2Damage = 25;
    public int slash3Damage = 25;
    public int slash4Damage = 25;
    [Space]
    public int heavyDamage = 40;
    public int heavy2Damage = 40;
    [Space]
    public int dischargeDamage = 40;
    [Space]
    public int blockAttack1Dmg = 30;
    public int blockAttack2Dmg = 30;

    //DODGE
    [Header("- Dodging")]
    [Tooltip("Input delay before player can dodge again")]
    public float dodgeCooldown = 1f;
    [Tooltip("Temporary movement speed boost while dodging")]
    public float dodgeDashBoost = 4f;
    [Tooltip("Stamina cost for dodging. Requires fine-tunning!")]
    [Range(.1f, .8f)]
    public float dodgeCost = .2f;

    [Tooltip("How long has the player to hold directional buttons to trigger the dodge. Requires fine-tunning!")]
    [Range(.1f, 1f)]
    public float axisThreshold = .1f;

    internal float dodgeCd = 0;

    //BLOCK-PARRY
    [Header("Blocking/Parrying")]
    [Tooltip("Player movement speed while blocking")]
    public float blockingSpeed = 2f;
    internal bool blocking = false;

    //DISCHARGE
    [Header("- Discharge Mechanic")]
    public float maxCharge = 100f;
    [Tooltip("How fast the sword actually charges")]
    public float chargeRate = 10f;
    [Tooltip("How fast the UI updates the charge")]
    public float UIChargeMultiplier = 2f;
    public ParticleSystem explosion;
    internal float currentCharge = 0f;
    internal float lastCharge = 0f;
    internal Image swordFill;
    public AudioSource fullCharge;


    internal bool canDischarge = false;
    internal bool discharging = false;

    public cameraShake camShake;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    [Tooltip("Slow motion duration")]
    public float dischargeSlowDuration = 2f;
    public ParticleSystem electricityCharge;
    public GameObject burst;
    public Transform burstPoint;
    #endregion

    [Space]
    public float LAStamCost;
    public float HAStamCost;
    [Space]

    #region SOUNDS
    [Header("- Sounds")]
    public AudioSource SwordSounds;
    public AudioSource MotionSounds;
    [Space]
    public AudioClip lightAttack1Sound;
    public AudioClip lightAttack2Sound;
    public AudioClip lightAttack3Sound;
    public AudioClip lightAttack4Sound;
    [Space]
    public AudioClip wallHit;
    [Space]
    public AudioClip heavyAttackSound;
    [Space]
    public AudioClip blockAttack1Sound;
    public AudioClip blockAttack2Sound;
    [Space]
    public AudioClip BlockSound;
    public AudioClip DodgeSound;
    [Space]
    public AudioClip DischargeFirst;
    public AudioClip DischargeSecond;
    [Space]
    public AudioClip ReceiveDmgSound;
    public AudioClip DeathSound;
    [Space]
    public AudioClip MoveSound;
    public AudioClip SprintSound;
    [Header("- Heartbeat effect")]
    public AudioClip SlowBeat;
    public AudioClip MediumSlowBeat;
    public AudioClip MediumFastBeat;
    public AudioClip FastBeat;
    [Space]
    public AudioClip[] otherSounds;
    public int otherSoundsIndex;
    #endregion

    internal float originSpeed;
    internal float originTimeReset;
    internal bool sprinting = false;
    internal bool jumping = false;

    bool played = false;

    Rigidbody rb;
    internal float speed;


    private void Awake()
    {
        timeManager = GameObject.FindGameObjectWithTag("Time Manager").GetComponent<TimeManager>();
        health = GameObject.Find("stateOfHealth").GetComponent<PlayerHealth>();
        stamina = GameObject.FindGameObjectWithTag("Stamina").GetComponent<PlayerStamina>();
        swordFill = GameObject.FindGameObjectWithTag("Charge").GetComponent<Image>();
        rb = GetComponent<Rigidbody>();

        //ORIGINAL MOVEMENT SPEED SET
        originSpeed = gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed;

        //HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //ORIGINAL TIME RESET RATE
        originTimeReset = timeManager.GetComponent<TimeManager>().slowmoDuration;

        
    }


    void Update()
    {
        //CHECKS
        sprinting = gameObject.GetComponent<vThirdPersonMotor>().isSprinting;
        jumping = !gameObject.GetComponent<vThirdPersonMotor>().isGrounded;
        speed = rb.velocity.magnitude;

        if (stamina.unit <= 0)
        {
            gameObject.GetComponent<vThirdPersonMotor>().isSprinting = false;
        }

        //UI SWORD CHARGE
        if (currentCharge > lastCharge && !discharging)
        {
            lastCharge += Time.deltaTime * UIChargeMultiplier;
            swordFill.fillAmount = lastCharge / maxCharge;
        }

        if (currentCharge == maxCharge && !played)
        {
            fullCharge.Play();
            played = true;
        }

        // MAKE SWORD DISCHARGE GRADUALLY
        if (discharging)
        {
            if (lastCharge > 0)
            {
                lastCharge -= Time.deltaTime * UIChargeMultiplier;
                swordFill.fillAmount = lastCharge / maxCharge;
                currentCharge = lastCharge;
                played = false;

            }
        }

        //SWORD FX
        if (attacking) swordTrail.SetActive(true);
        else swordTrail.SetActive(false);

        //STUN TEST
        if (Input.GetKeyDown(KeyCode.L))
        {
            Stun();
        }
    }

    //SWORD CHARGE
    public void Charge()
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
    private void OnDrawGizmos()
    {
        if (detectPoint == null || aoePoint == null || heavyPoint == null) return;

        //Gizmos.DrawWireSphere(detectPoint.position, attackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(aoePoint.position, aoePoint.localScale);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(heavyPoint.position, heavyPoint.localScale);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(detectPoint.position, detectPoint.localScale);
    }

    public void TakeDamage(int damage)
    {
        health.Damage(damage);
    }

    public void Stun()
    {
        if (health.immortal || jumping) return;
        if (!stunned)
        {
            if (!sprinting)
            {
                anim.SetTrigger("stun2");
                stunned = true;
                StartCoroutine(Stunning(1.5f));
            }

            else
            {
                anim.SetTrigger("stun");
                stunned = true;
                StartCoroutine(Stunning(3f));
            }
            
        }
        

    }

    IEnumerator Stunning(float delay)
    {
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = true;
        yield return new WaitForSeconds(delay);

        stunned = false;
        gameObject.GetComponent<vThirdPersonMotor>().stopMove = false;

    }
}

