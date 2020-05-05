using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 direction;
    public Vector3 startingPosition;
    GameObject detector;
    public float speed;
    public int damage;
    internal bool hit;


    // Start is called before the first frame update
    void Start()
    {
        //detector = GameObject.FindGameObjectWithTag("Detector");
        hit = false;
        //Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), detector.GetComponent<Collider>(), true);
    }

    void FixedUpdate()
    {
        Move();
        StartCoroutine(NotHitting());

    }

    IEnumerator NotHitting ()
    {
        yield return new WaitForSeconds(2f);
        if (!hit) ResetProjectile();
    }
    void Move()
    {
        transform.position = transform.position + (direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<playerController>().TakeDamage(damage);
            hit = true;
        }
        ResetProjectile();
    }
    void ResetProjectile()
    {
        gameObject.SetActive(false);
    }
}
