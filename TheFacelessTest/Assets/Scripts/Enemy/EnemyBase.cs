using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    #region LIFE PARAMETERS
    public float maxHealth;
    protected float currentHealth;
    public Image healthBar;
    internal bool dying = false;
    public ParticleSystem electricStun;
    #endregion

    #region PLAYER SENSING PARAMETERS
    protected GameObject player;
    protected Vector3 lastKnownPlayerLocation;
    protected float distanceToPlayer;

    protected bool playerDetected;
    protected bool canHitPlayer;

    protected float timeSinceLastSawPlayer = Mathf.Infinity;

    public float suspicionTime;
    #endregion

    #region SENSING PARAMETERS
    public float senseFrequency;
    protected float senseTimer;
    protected bool pursuing;
    public float pursueDelay;
    protected float pursueDelayTimer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
