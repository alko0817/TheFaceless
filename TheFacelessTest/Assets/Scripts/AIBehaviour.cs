using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public float sightDistance;
    public float attackDistance;
    public float speed;


    private GameObject player;
    private Transform lastKnownPlayerLocation;
    private float distanceToPlayer;

    public float senseFrequency;
    private float senseTimer;

    private bool playerDetected;
    private bool moveToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
        moveToPlayer = false;
        senseTimer = 0.0f;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

        senseTimer += Time.deltaTime;

        if(senseTimer > senseFrequency)
        {
            senseTimer = 0.0f;
            Sense();
        }

        Decide();
        Act();
    }


    void Sense()
    {

        if (distanceToPlayer < sightDistance)
        {
            playerDetected = true;
            lastKnownPlayerLocation = player.transform;
            Debug.Log("Player sighted by " + gameObject.name);
        }
        else
        {
            playerDetected = false;
            Debug.Log(gameObject.name + " lost sight of Player");
        }

    }

    void Decide()
    {
        if (playerDetected && distanceToPlayer > attackDistance)
        {
            moveToPlayer = true;
        }
    }

    void Act()
    {
        if(moveToPlayer)
        {
            MoveTo(lastKnownPlayerLocation);
        }


    }

    void Attack()
    {

    }

    void Patrol()
    {

    }

    void MoveTo(Transform location)
    {

    }
}
