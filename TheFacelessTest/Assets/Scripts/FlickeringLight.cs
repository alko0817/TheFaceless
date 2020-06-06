using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light flick;
    public float minWaitTime;
    public float maxWaitTime;
    public bool useFlickering;
    [Space]
    public MeshRenderer mesh;
    public Material dark;
    public Material bright;

    private float timer;

    private void Update()
    {
        if (!useFlickering) return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = Random.Range(minWaitTime, maxWaitTime);
            flick.enabled = !flick.enabled;
            if (flick.enabled) mesh.material = bright;
            else mesh.material = dark;

        }
    }
}
