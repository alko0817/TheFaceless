using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    vThirdPersonCamera controller;
    Camera cam;
    public float zoomRate = 1f;
    public float maxDistance;
    [Space]
    public float aimRate = 1f;
    public float maxAim;

    bool unzooming = false;
    bool zooming = false;
    bool aim = false;
    bool unaim = false;

    float originDistance;
    float originView;

    private void Start()
    {
        cam = Camera.main;
        controller = GetComponent<vThirdPersonCamera>();
        originDistance = controller.defaultDistance;
        originView = cam.fieldOfView;
    }

    private void Update()
    {
        if (zooming)
        {
            controller.defaultDistance -= Time.deltaTime * zoomRate;
            if (controller.defaultDistance <= originDistance) zooming = false;
        }

        else if (unzooming)
        {
            controller.defaultDistance += Time.deltaTime * zoomRate;
            if (controller.defaultDistance >= maxDistance) unzooming = false;
        }
        else if (aim)
        {
            cam.fieldOfView -= Time.deltaTime * aimRate;
            if (cam.fieldOfView <= maxAim) aim = false;
        }
        else if (unaim)
        {
            cam.fieldOfView += Time.deltaTime * aimRate;
            if (cam.fieldOfView >= originView) unaim = false;
        }
        controller.defaultDistance = Mathf.Clamp(controller.defaultDistance, originDistance, maxDistance);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, maxAim, originView);
    }


    public void ZoomIn ()
    {
        zooming = true;
        unzooming = false;
        aim = false;
        unaim = false;
    }

    public void ZoomOut ()
    {
        zooming = false;
        unzooming = true;
        aim = false;
        unaim = false;
    }

    public void Aim()
    {
        zooming = false;
        unzooming = false;
        aim = true;
        unaim = false;
    }

    public void Unaim()
    {
        zooming = false;
        unzooming = false;
        aim = false;
        unaim = true;
    }
}
