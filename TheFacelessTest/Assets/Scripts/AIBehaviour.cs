using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public float sightDistance;
    public float attackDistance;
    public float speed;

    private float senseTimer;
    private float actTimer;

    GameObject player;

    private bool playerDetected;


    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Sense(float dt)
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = vectorToPlayer.magnitude;
        
        if(distanceToPlayer < sightDistance)
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }
    }

    void Decide()
    {

    }

    void Act(float dt)
    {

    }

    void Attack()
    {

    }

    void MoveTo(Transform location)
    {

    }
}
