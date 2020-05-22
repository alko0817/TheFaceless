using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class WildFire : MonoBehaviour
{
    public float speed;
    public ParticleSystem fire;
    Vector3 orPos;
    Vector3 vel;

    public float travelTime;
    float timer;
    bool shot = false;

    private void Start()
    {
        vel = new Vector3(speed, 0, 0);
        orPos = transform.localPosition;
    }
    private void Update()
    {

        timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R) && !shot)
        {
            timer = travelTime;
            shot = true;
            fire.Play();
        }

        if (timer > 0)
        {
            //transform.Translate(vel);

        }

        else
        {
            shot = false;
            transform.localPosition = orPos;
            fire.Stop();
        }
    }
}
