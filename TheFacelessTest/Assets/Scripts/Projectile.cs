using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 direction;
    public Vector3 startingPosition;
    public float speed;
    public int damage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<playerController>().TakeDamage(damage);

        }
        ResetProjectile();

 
    }

    void ResetProjectile()
    {
        gameObject.SetActive(false);
    }
}
