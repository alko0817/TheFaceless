using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Range(.01f, .9f)]
    public float slowmoIntensity = .05f;
    public float slowmoDuration = 2f;


    private void Update()
    {
        Time.timeScale += (1f / slowmoDuration) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }
    public void Slowmo ()
    {
        Time.timeScale = slowmoIntensity;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        //StartCoroutine("TimeReset");

    }

    //IEnumerator TimeReset()
    //{
    //    while (true)
    //    {
    //        if (Time.timeScale < 1) Time.timeScale += (1f / slowmoDuration) * Time.unscaledDeltaTime;

    //        else yield return null;
    //    }
    //}
}
