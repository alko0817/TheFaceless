using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class EnemyBase : MonoBehaviour
{
    protected enum STATE
    {
        IDLE,
        PATROL,
        SUSPICIOUS,
        PURSUE,
        IN_COMBAT,
        STUNNED,
        FLEE
    }

    [Header("- Base Properties -")]
    public LayerMask playerMask;
    [Space]
    [Header("Base Components")]
    #region COMPONENETS
    public ParticleSystem electricStun;
    protected SpawnEffect dissolving;
    internal NavMeshAgent navMeshAgent;
    internal Animator anim;
    protected AudioSource audioSource;
    protected audioManager audioManager;
    protected EnemyBlackboard blackboard;
    public Image healthBar;
    public ParticleSystem hit;
    #endregion

    [Header("Global Settings Checkbox")]
    #region BOOLEANS
    [Tooltip("If checked, enemy will use settings defined in the Enemy Blackboard")]
    public bool useGlobalSettings;
    [Space]
    protected bool playerDetected;
    internal bool dying;
    protected bool stunned;
    internal bool engaging;
    internal bool blocking;
    #endregion

    [Header("Base Variables")]
    #region VARIABLES
    public float maxHealth;
    public float sightDistance;
    public float initialSpeed;
    #endregion

    #region INTERENAL PARAMETERS
    protected STATE state_;
    internal float currentHealth;
    protected GameObject player;
    protected Vector3 lastKnownPlayerLocation;
    protected Vector3 initialPosition;
    protected float distanceToPlayer;
    protected float timeSinceLastSawPlayer;
    #endregion



    #region SOUND
    //[Header("Sound FX")]
    //[Space]
    //public AudioClip ReceiveDmgSound;
    //public AudioClip DealDmgSound;
    //[Space]
    //public AudioClip StunnedSound;
    //public AudioClip DeathSound;
    //[Space]
    //public AudioClip MoveSound;
    //public AudioClip StaticSound;
    //public AudioClip ChatterSound;
    #endregion

    protected virtual void Start()
    {
        playerDetected = false;
        dying = false;
        engaging = false;
        stunned = false;
        blocking = false;

        timeSinceLastSawPlayer = Mathf.Infinity;

        currentHealth = maxHealth;


        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = initialSpeed;

        player = GameObject.FindWithTag("Player");
        blackboard = GameObject.FindWithTag("Blackboard").GetComponent<EnemyBlackboard>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<audioManager>();

        audioSource = GetComponent<AudioSource>();
        dissolving = GetComponent<SpawnEffect>();
        anim = GetComponent<Animator>();

        state_ = STATE.IDLE;
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

        if (currentHealth <= 0)
        {
            Die();
        }

        if (!dying)
        {
            Sense();
            Decide();
            Act();
        }


        UpdateTimers();

    }

    
    protected virtual void UpdateTimers()
    {
        timeSinceLastSawPlayer += Time.deltaTime;
    }

    protected virtual void Sense()
    {
        if (distanceToPlayer < sightDistance && IsPlayerVisible())
        {
            lastKnownPlayerLocation = player.transform.position;

            if (!engaging)
                blackboard.AddEnemyInSight(this.gameObject);
            else
                timeSinceLastSawPlayer = 0.0f;

            playerDetected = true;

        }
        else
        {

            blackboard.RemoveEnemyInSight(this.gameObject);
            blackboard.RemovePursuingEnemy(this.gameObject);
            playerDetected = false;
        }
    }


    protected virtual void Decide()
    {

        if (stunned)
        {
            state_ = STATE.STUNNED;
        }

    }

    protected virtual void Act()
    {
        if (state_ == STATE.STUNNED)
        {
            navMeshAgent.speed = initialSpeed;

            StartCoroutine(Stun());
        }

    }


    public virtual void SetEngaging(bool value)
    {
        engaging = value;
    }

    public virtual void SetStunned(bool value)
    {
        stunned = value;
    }

    IEnumerator Stun()
    {
        Stop();
        electricStun.Play();
        yield return new WaitForSeconds(3);
        electricStun.Stop();
        stunned = false;
    }


    protected virtual bool IsPlayerVisible()
    {
        NavMeshHit hit;
        if (!navMeshAgent.Raycast(player.transform.position, out hit))
        {
            return true;
        }
        else
            return false;
    }

    protected virtual void MoveTo(Vector3 location, float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = location;     
        navMeshAgent.isStopped = false;
        //audioSource.PlayOneShot(MoveSound);
    }

    protected virtual void Stop()
    {
        navMeshAgent.speed = 0;
        navMeshAgent.isStopped = true;
        audioSource.Stop();
    }

    public virtual void TakeDamage(int damage)
    {
        if (dying) return;
        if (!blocking)
        {
            currentHealth -= damage;
            healthBar.fillAmount = currentHealth / maxHealth;
        }
        else
        {

            print("attack blocked");
        }
        hit.Emit(1);

        //HURT ANIMATIONS

    }

    protected virtual void Guard()
    {
        Vector3 vectorToInitialPos = initialPosition - transform.position;
        float dist = vectorToInitialPos.magnitude;

        if (Math.Abs(dist) > 2f)
            MoveTo(initialPosition, initialSpeed);
        else
            Stop();
    }

    protected virtual void Die()
    {
        dying = true;
       // audioSource.PlayOneShot(DeathSound);
        blackboard.RemoveEnemyInSight(this.gameObject);
        blackboard.RemovePursuingEnemy(this.gameObject);
        Stop();
        StartCoroutine("Dissolve");
        Destroy(gameObject, 4f);

    }

    IEnumerator Dissolve ()
    {
        yield return new WaitForSeconds(2f);
        dissolving.enabled = true;
    }


}
