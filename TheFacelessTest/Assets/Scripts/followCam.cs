using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour
{
    public Transform cam;
    Transform holder;
    public float smooth;

    private void Start()
    {
        holder = GetComponent<Transform>();
        
    }

    private void Update()
    {
        holder.position = Vector3.Lerp(holder.position, cam.position, smooth * Time.deltaTime);
    }
}
