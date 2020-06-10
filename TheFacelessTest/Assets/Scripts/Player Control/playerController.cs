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
    public ParticleSystem heavyTrail;
    [Header("- Player Attack Pointers")]
    public Transform detectPoint;
    public Transform heavyPoint;
    public Transform aoePoint;
    [Tooltip("By default, this needs to be 'Enemy'")]
    public LayerMask enemyLayer;
    [Tooltip("Radius at which damage is applied. Requires fine-tunning!")]
    public float attackRadius = .5f;
    public float heavyRadius = 2f;
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
    internal bool charged = false;

    //DISCHARGE
    [Header("- Discharge")]
    [Tooltip("How fast the sword actually charges")]
    public float chargeRate = 10f;
    [Tooltip("How fast the UI updates the charge")]
    public float chargeMultiplier = 2f;

    internal float lastCharge = 0f;
    internal Image swordFill;
    public AudioSource fullCharge;
    [Header("- Sword Effects when fully charged")]
    public ParticleSystem electricCharge;
    public ParticleSystem frostCharge;
    public ParticleSystem fireCharge;
    [Space]
    public cameraShake camShake;
    public GameObject burst;
    public Transform burstPoint;
    float swordCharge = 0f;
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
        SwordTrail();
        Cheats();
    }
    private void Cheats()
    {
        if (Input.GetKeyDown(KeyCode.L)) Stun();
        if (Input.GetKeyDown(KeyCode.P))
        {
            lastCharge = 1;
        }
    }
    private void SwordTrail()
    {
        if (attacking) swordTrail.SetActive(true);
        else swordTrail.SetActive(false);
    }
    private void Checks()
    {
        sprinting = gameObject.GetComponent<vThirdPersonMotor>().isSprinting;
        jumping = !gameObject.GetComponent<vThirdPersonMotor>().isGrounded;
        speed = rb.velocity.magnitude;
        if (stamina.unit <= 0) gameObject.GetComponent<vThirdPersonMotor>().isSprinting = false;
    }
    public void ChargeCheck()
    {
        swordCharge = Mathf.Clamp01(swordCharge);
        swordFill.fillAmount = swordCharge;
        if (!discharging && !charged)
        {
            if (swordCharge < lastCharge)
            {
                swordCharge += Time.deltaTime * chargeMultiplier;
            }
        }

        if (swordCharge >= 1 && !charged)
        {
            charged = true;
            canDischarge = true;
        }
    }
    /// <summary>
    /// Instantly discharges the UI sword.
    /// </summary>
    public void UIDischarge()
    {
        if (discharging)
        {
            if (swordCharge > 0)
            {
                charged = false;
                lastCharge = 0;
                swordCharge -= Time.deltaTime;
            }
        }
    }
    /// <summary>
    /// Gradually discharge the UI sword, following the given lasting duration.
    /// </summary>
    /// <param name="duration"></param>
    public void UIDischarge(float duration, float maxDuration)
    {
        if (discharging)
        {
            if (swordCharge > 0)
            {
                lastCharge = 0;
                swordCharge = (duration/maxDuration);
                charged = false;
            }
        }
    }
    public void Charge(float rate)
    {
        lastCharge += rate;
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

