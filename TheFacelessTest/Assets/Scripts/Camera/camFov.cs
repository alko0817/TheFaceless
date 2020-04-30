using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camFov : MonoBehaviour
{
    public vThirdPersonCamera controller;
    public float zoomRate = 1f;

    bool unzooming = false;
    bool zooming = false;
    float originDistance;
    public float maxDistance; 

    private void Start()
    {
        //controller = GetComponentInChildren<vThirdPersonCamera>();
        originDistance = controller.defaultDistance;
    }

    private void Update()
    {
        if (zooming)
        {
            unzooming = false;
            controller.defaultDistance -= Time.deltaTime * zoomRate;
            if (controller.defaultDistance <= originDistance) zooming = false;
        }

        if (unzooming)
        {
            zooming = false;
            controller.defaultDistance += Time.deltaTime * zoomRate;
            if (controller.defaultDistance >= maxDistance - .1f) unzooming = false;
        }
        controller.defaultDistance = Mathf.Clamp(controller.defaultDistance, originDistance, maxDistance);
    }


    public void FovIn ()
    {
        zooming = true;      
    }

    public void FovOut ()
    {
        unzooming = true;
    }
}
