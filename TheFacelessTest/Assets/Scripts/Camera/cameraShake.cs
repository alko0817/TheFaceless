using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    public IEnumerator Shake (float duration, float magnitude)
    {

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = transform.localPosition.x * Random.Range(-1f, 1f) * magnitude;
            float y = transform.localPosition.y * Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, transform.localPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
