using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

public class playerController : MonoBehaviour
{
    public Animator anim;
    PlayerHealth health; 
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

    internal int combos = 0;
    internal int combosBlock = 0;
    internal float lastClick = 0f;

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
    internal float nextCombo = 0f;

    [Header("- Delay before attack lands and applies damage")]
    public float hitLight1 = 0f;
    public float hitLight2 = 0f;
    public float hitHeavy = 0f;
    public float hitDischarge = 0f;
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

    internal float dodgeCd = 0;

    //BLOCK-PARRY
    [Header("Blocking/Parrying")]
    [Tooltip("Player movement speed while blocking")]
    public float blockingSpeed = 2f;

    [HideInInspector]
    public bool blocking = false;

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

    }

    void Update()
    {
        //UI SWORD CHARGE
        if (currentCharge > lastCharge)
        {
            lastCharge += Time.deltaTime * UIChargeMultiplier;
            swordFill.fillAmount = lastCharge / maxCharge;
        }

        // MAKE SWORD DISCHARGE GRADUALLY

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
