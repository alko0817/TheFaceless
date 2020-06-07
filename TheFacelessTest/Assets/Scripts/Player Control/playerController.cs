using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

[System.Serializable]
public class playerController : MonoBehaviour
{
    
    public Animator anim;
    internal vThirdPersonCamera camSettings;
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
    public CameraView cameraView;

    #region COMBAT_VARIABLES
    internal bool shooting = false;
    internal bool aiming = false;
    internal bool holding = false;
    internal bool attacking = false;
    internal bool blocking = false;
    internal bool dodging = false;
    internal float global = 0f;
    internal bool isDischarge = false;
    internal bool stunned = false;
    internal bool canDischarge = false;
    internal bool discharging = false;

    //DISCHARGE
    [Header("- Discharge")]
    public float maxCharge = 100f;
    [Tooltip("How fast the sword actually charges")]
    public float chargeRate = 10f;
    [Tooltip("How fast the UI updates the charge")]
    public float UIChargeMultiplier = 2f;
    [Space]
    public ParticleSystem explosion;
    internal float currentCharge = 0f;
    internal float lastCharge = 0f;
    internal Image swordFill;
    public AudioSource fullCharge;
    [Space]
    public cameraShake camShake;
    public ParticleSystem electricityCharge;
    public GameObject burst;
    public Transform burstPoint;
    #endregion
    [Space]

    #region SOUNDS
    [Header("- Sounds")]
    public AudioSource SwordSounds;
    public AudioSource MotionSounds;
    [Space]
    public AudioClip wallHit;    
    [Space]
    public AudioClip[] ReceiveDmgSound;
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
    internal float halfSpeed;
    internal float originTimeReset;
    internal bool sprinting = false;
    internal bool jumping = false;

    bool played = false;

    internal Rigidbody rb;
    internal float speed;


    private void Awake()
    {
        timeManager = GameObject.FindGameObjectWithTag("Time Manager").GetComponent<TimeManager>();
        health = GameObject.Find("stateOfHealth").GetComponent<PlayerHealth>();
        stamina = GameObject.FindGameObjectWithTag("Stamina").GetComponent<PlayerStamina>();
        swordFill = GameObject.FindGameObjectWithTag("Charge").GetComponent<Image>();
        camSettings = GameObject.FindGameObjectWithTag("Camera Holder").GetComponent<vThirdPersonCamera>();
        rb = GetComponent<Rigidbody>();

        //MOVEMENT SPEED SET
        originSpeed = gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed;
        halfSpeed = originSpeed / 2f;

        //HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //ORIGINAL TIME RESET RATE
        originTimeReset = timeManager.GetComponent<TimeManager>().slowmoDuration;

        
    }


    void Update()
    {
        //GLOBAL ACTION COOLDOWN
        global -= Time.deltaTime;

        Checks();
        UICharge();
        UIDischarge();

        //SWORD FX
        if (attacking) swordTrail.SetActive(true);
        else swordTrail.SetActive(false);

        //STUN TEST
        if (Input.GetKeyDown(KeyCode.L)) Stun();
    }
    private void Checks()
    {
        sprinting = gameObject.GetComponent<vThirdPersonMotor>().isSprinting;
        jumping = !gameObject.GetComponent<vThirdPersonMotor>().isGrounded;
        speed = rb.velocity.magnitude;
        if (stamina.unit <= 0) gameObject.GetComponent<vThirdPersonMotor>().isSprinting = false;
    }
    private void UICharge()
    {
        if (currentCharge > lastCharge && !discharging)
        {
            lastCharge += Time.deltaTime * UIChargeMultiplier;
            swordFill.fillAmount = lastCharge / maxCharge;
            if (swordFill.fillAmount == 1)
            {
                currentCharge = maxCharge;
            }
        }

        if (currentCharge == maxCharge && !played)
        {
            fullCharge.Play();
            played = true;
        }
    }
    private void UIDischarge()
    {
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
    }
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
    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(aoePoint.position, aoePoint.localScale);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(heavyPoint.position, heavyPoint.localScale);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(detectPoint.position, detectPoint.localScale);
        }

        catch
        {
            print("Assign Gizmos!!!");
        }
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
    public void Dialog(AudioClip clip)
    {
        SwordSounds.PlayOneShot(clip);
    }
}

