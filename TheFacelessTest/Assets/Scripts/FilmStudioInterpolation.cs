using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilmStudioInterpolation : MonoBehaviour
{
    public Vector3 startingLocation;
    public Vector3 endingLocation;

    public Quaternion startingRotation;
    public Quaternion endingRotation;

    public float startingFov;
    public float endingFov;

    public float travelTime;
    public bool looping;
    Vector3 pos;
    Quaternion rot;

    private float timeAccumulated = 0f;
    private float looper = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        rot = transform.rotation;

        timeAccumulated += Time.deltaTime;

        looper = timeAccumulated / travelTime;
        if (looper >= 1 && looping)
        {
            timeAccumulated -= travelTime;
            looper -= 1;
        }

        pos.x = Mathf.Lerp(startingLocation.x, endingLocation.x, looper);
        pos.y = Mathf.Lerp(startingLocation.y, endingLocation.y, looper);
        pos.z = Mathf.Lerp(startingLocation.z, endingLocation.z, looper);

        rot = Quaternion.Lerp(startingRotation, endingRotation, looper);

        gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(startingFov, endingFov, looper);

        transform.position = pos;
        transform.rotation = rot;
        //this script kinda grew lmao
    }
}
