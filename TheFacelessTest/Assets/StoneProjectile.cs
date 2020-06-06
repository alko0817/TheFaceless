using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    Transform point;
    bool shot = false;
    private void Start()
    {
        point = GameObject.FindGameObjectWithTag("Stone Point").GetComponent<Transform>();
    }
    private void Update()
    {
        if (isActiveAndEnabled && !shot)
        {
            transform.position = point.position;
            transform.rotation = point.rotation;
            shot = true;
            StartCoroutine(RIP());
        }
    }

    IEnumerator RIP ()
    {
        yield return new WaitForSeconds(1.5f);
        shot = false;
        gameObject.SetActive(false);
    }
}
