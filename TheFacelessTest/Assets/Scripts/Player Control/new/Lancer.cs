using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancer : MonoBehaviour
{
    internal GameObject enemy;
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            enemy = other;
        }
    }
}

