using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerLock : MonoBehaviour
{
    playerController controller;
    public vThirdPersonCamera tpc;
    public Transform detectArea;
    public float detectRadius;
    Camera cam;

    bool targeting = false;
    Transform target;

    private void Start()
    {
        controller = GetComponent<playerController>();
        cam = Camera.main;
    }

    private void Update()
    {
        Collider[] enemies = Physics.OverlapSphere(detectArea.position, detectRadius, controller.enemyLayer);

        if (enemies == null) target = null;

        foreach(Collider enemy in enemies)
        {
            target = enemy.transform;
        }


        if (Input.GetKeyDown(KeyCode.Tab) && !targeting)
        {
            if (target != null)
            {
                targeting = true;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Tab) && targeting)
        {
            targeting = false;
        }
    }
}
