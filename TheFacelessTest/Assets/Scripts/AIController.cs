using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    GameObject player;
    public float chaseDistance;
    public float attackDistance;
    public float speed;

    private float distanceToPlayer;

    private void Update()
    {
        player = GameObject.FindWithTag("Player");

        Vector3 vectorToPlayer = player.transform.position - transform.position;

        distanceToPlayer = vectorToPlayer.magnitude;

        if (distanceToPlayer < chaseDistance)
        {
            float startTime = Time.time; 

            Debug.Log(this.name + " sees " + player.name);
            //float distCovered = (Time.time - startTime) * speed;
            //float fractionOfJourney = distCovered / distanceToPlayer;

            //transform.position = Vector3.Lerp(this.transform.position, player.transform.position, fractionOfJourney);


            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();

            if (distanceToPlayer > attackDistance)
            {
                transform.position = transform.position + (direction * speed * Time.deltaTime);
            }
            
        }



    }



}
