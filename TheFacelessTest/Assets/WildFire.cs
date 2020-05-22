using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class WildFire : MonoBehaviour
{
    public float speed;
    public float travelTime;
    [Space]
    public ParticleSystem fire;
    public GameObject explosion;
    [Space]
    public LayerMask enemyLayer;
    public int damage;
    public float dmgRadius;


    playerController controller;
    Vector3 orPos;
    Vector3 vel;

    float timer;
    bool shot = false;
    bool startFlame = true;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
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
            StartCoroutine(Flame());
            
        }
        if (startFlame)
        {
            Guide();
        }
    }

    IEnumerator Flame()
    {
        yield return new WaitForSeconds(1.5f);
        Instantiate(explosion, controller.burstPoint.position, Quaternion.Euler(90, 0, 0));
        fire.Play();
        startFlame = true;
        yield return new WaitForSeconds(travelTime);
        startFlame = false;
        shot = false;
        transform.localPosition = orPos;
        fire.Stop();
    }

    void Guide()
    {
        if (Input.GetKey(KeyCode.Mouse3))
        {
            transform.Translate(vel);
        }

        if (Input.GetKey(KeyCode.Mouse4))
        {
            transform.Translate(-vel);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, dmgRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            enemy.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dmgRadius);
    }
}
