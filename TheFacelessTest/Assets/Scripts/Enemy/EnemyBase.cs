using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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

    protected STATE state_;
    public LayerMask playerMask;
    protected audioManager audioManager;
    protected EnemyBlackboard blackboard;

    public Image healthBar;

    #region COMPONENETS
    public ParticleSystem electricStun;
    protected SpawnEffect dissolving;
    protected NavMeshAgent navMeshAgent;
    protected Animator anim;
    protected AudioSource audioSource;
    #endregion

    #region TIMERS
    protected float timeSinceLastSawPlayer;
    //protected float senseTimer;
    #endregion

    #region BOOLEANS
    protected bool playerDetected;
    protected bool dying;
    protected bool stunned;
    protected bool engaging;
    protected bool blocking;
    #endregion

    #region LIFE PARAMETERS
    public float maxHealth;
    protected float currentHealth;
    #endregion

    #region PLAYER SENSING PARAMETERS
    protected GameObject player;
    protected Vector3 lastKnownPlayerLocation;
    protected float distanceToPlayer;
    #endregion

    #region SENSING PARAMETERS
    public float sightDistance;
    public float senseFrequency;
    #endregion

    public float initialSpeed;

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
            if (!engaging)
                blackboard.AddEnemyInSight(this.gameObject);

            lastKnownPlayerLocation = player.transform.position;

            playerDetected = true;
            timeSinceLastSawPlayer = 0.0f;

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
        //HURT ANIMATIONS

    }



    protected virtual void Die()
    {
        dying = true;
       // audioSource.PlayOneShot(DeathSound);
        blackboard.RemoveEnemyInSight(this.gameObject);
        blackboard.RemovePursuingEnemy(this.gameObject);
        Stop();
        dissolving.enabled = true;
        Destroy(gameObject, 2f);

    }
}
