using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using UnityEngine;

public class WildFire : MonoBehaviour
{
    public float speed;
    public float travelTime;
    [Space]
    public ParticleSystem lingeringFlame;
    public ParticleSystem swordFire;
    public GameObject explosion;
    [Space]
    public LayerMask enemyLayer;
    public int damage;
    public float dmgRadius;
    [Range(3f, 30f)]
    public float maxDistance;

    playerController controller;
    Vector3 maxPos;
    Vector3 orPos;
    Vector3 vel;

    bool shot = false;
    bool startFlame = false;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        vel = new Vector3(speed, 0, 0);
        orPos = transform.localPosition;
        maxPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + maxDistance);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !shot)
        {
            shot = true;
            StartCoroutine(Flame());
            
        }
        if (startFlame)
        {
            Guide();
            ClampDistance();
        }
    }

    IEnumerator Flame()
    {
        yield return new WaitForSeconds(1.4f);
        Instantiate(explosion, controller.burstPoint.position, Quaternion.Euler(90, 0, 0));
        lingeringFlame.Play();
        swordFire.Play();
        startFlame = true;
        controller.foving.FovOut();
        yield return new WaitForSeconds(travelTime);
        controller.foving.FovIn();
        startFlame = false;
        shot = false;
        transform.localPosition = orPos;
        lingeringFlame.Stop();
        swordFire.Stop();
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

    void ClampDistance ()
    {
        float distance = Vector3.Distance(orPos, transform.localPosition);
        if (distance > maxDistance) transform.localPosition = maxPos;
        if (transform.localPosition.magnitude < orPos.magnitude) transform.localPosition = orPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dmgRadius);
    }
}
