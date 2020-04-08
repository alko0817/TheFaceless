using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camPan : MonoBehaviour
{
    public Camera cam;
    public float fov = 50f;
    public float panRate = 1f;
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
            cam.fieldOfView += Time.deltaTime * panRate;
        }

        if (zoomed)
        {
            cam.fieldOfView -= Time.deltaTime * panRate;
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, fov, originFov);
    }
    public void Pan ()
    {
        zoomed = true;      
    }

    public void Unpan ()
    {
        zoomed = false;
    }
}
