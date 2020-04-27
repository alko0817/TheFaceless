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


    // Start is called before the first frame update
    void Start()
    {
        detector = GameObject.FindGameObjectWithTag("Detector");

        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), detector.GetComponent<Collider>(), true);
    }

    void Update()
    {
        Move();
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

        }
        ResetProjectile();
    }
    void ResetProjectile()
    {
        gameObject.SetActive(false);
    }
}
