using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

[RequireComponent(typeof(TimeManager))]
public class playerController : MonoBehaviour
{
    public Animator anim;
    internal PlayerHealth health;
    internal PlayerStamina stamina;
    public GameObject swordTrail;
    [Header("- Player Attack Pointers")]
    public Transform detectPoint;
    public Transform aoePoint;
    [Tooltip("By default, this needs to be 'Enemy'")]
    public LayerMask enemyLayer;
    [Tooltip("Radius at which damage is applied by simple attacks. Requires fine-tunning!")]
    public float attackRadius = .5f;
    [Tooltip("Area of effect for the Discharge attack")]
    public float aoeRadius = 5f;

    [Header("- Slow Motion & Camera FX")]
    public TimeManager timeManager;
    public camFov foving;

    #region COMBAT_VARIABLES

    internal int combos = 0;
    internal int combosBlock = 0;
    internal float lastClick = 0f;

    [Header("- Attack Intervals")]
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float attackDelay3 = 1.5f;
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

    [Header("- Attack Damage")]
    public int slashDamage = 20;
    public int slash2Damage = 25;
    public int slash3Damage = 25;
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

    
    internal bool canDischarge = false;
    internal bool discharging = false;

    public cameraShake camShake;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    [Tooltip("Slow motion duration")]
    public float dischargeSlowDuration = 2f;
    public ParticleSystem electricityCharge;
    public ParticleSystem burst;
    #endregion

    #region SOUNDS
    [Header("- Sound Clips")]
    [Tooltip("Copy-paste the clip name from the Audio Manager")]
    public string lightAttack1Sound;
    public string lightAttack2Sound;
    public string lightAttack3Sound;
    [Space]
    public string heavyAttackSound;
    [Space]
    public string blockAttack1Sound;
    public string blockAttack2Sound;
    [Space]
    public string enemyHitSound;
    [Space]
    public string[] otherSounds;
    public int otherSoundsIndex;
    #endregion

    internal float originSpeed;
    internal float originTimeReset;
    
    

    private void Awake()
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
        stamina = GameObject.FindGameObjectWithTag("Stamina").GetComponent<PlayerStamina>();
    }

    void Update()
    {
        //UI SWORD CHARGE
        if (currentCharge > lastCharge && !discharging)
        {
            lastCharge += Time.deltaTime * UIChargeMultiplier;
            swordFill.fillAmount = lastCharge / maxCharge;
        }

        // MAKE SWORD DISCHARGE GRADUALLY
        if (discharging)
        {
            if (lastCharge > 0)
            {
                lastCharge -= Time.deltaTime * UIChargeMultiplier;
                swordFill.fillAmount = lastCharge / maxCharge;
                currentCharge = lastCharge;
            }
        }

        //SWORD FX
        if (attacking) swordTrail.SetActive(true);
        else swordTrail.SetActive(false);

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
