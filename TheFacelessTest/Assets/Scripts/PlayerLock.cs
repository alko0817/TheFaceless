using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Video;

public class PlayerLock : MonoBehaviour
{
   
    playerController controller;
    public vThirdPersonCamera tpc;
    public Transform detectArea;
    public float detectRadius;
    Camera cam;

    bool targeting = false;
    Transform target;
    GameObject enemy;

    private void Start()
    {
        controller = GetComponent<playerController>();
        cam = Camera.main;
    }

    private void Update()
    {
        Detect();
        Track();
        Target();
    }

    void Detect ()
    {
        Collider[] enemies = Physics.OverlapSphere(detectArea.position, detectRadius, controller.enemyLayer);

        if (enemies == null) target = null;

        foreach (Collider enemy in enemies)
        {
            target = enemy.transform;
        }
    }

    void Track ()
    {
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

    void Target()
    {
        if (targeting)
        {
            transform.LookAt(target);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
            if (!target.gameObject.activeInHierarchy) targeting = false;
        }
    }
}
