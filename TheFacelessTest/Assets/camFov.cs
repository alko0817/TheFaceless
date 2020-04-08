using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camFov : MonoBehaviour
{
    public Camera cam;
    public float fov = 50f;
    public float zoomRate = 1f;
    bool zoomed = false;
    float originFov;

    private void Start()
    {
        originFov = cam.fieldOfView;
    }

    private void Update()
    {
        if (!zoomed)
        {
            cam.fieldOfView += Time.deltaTime * zoomRate;
        }

        if (zoomed)
        {
            cam.fieldOfView -= Time.deltaTime * zoomRate;
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, fov, originFov);
    }
    public void FovIn ()
    {
        zoomed = true;      
    }

    public void FovOut ()
    {
        zoomed = false;
    }
}
