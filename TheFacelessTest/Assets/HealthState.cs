using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthState : MonoBehaviour
{
    AIBehaviour controller;
    public Image healthFace;
    private Color color;
    private float health;
    private float reverse;

    private void Start()
    {
        controller = GetComponent<AIBehaviour>();
        color.r = 0;
        color.g = 1f;
        color.b = 0f;
        color.a = 1f;
        healthFace.color = color;
    }

    private void Update()
    {
        //MATH
        health = controller.currentHealth / controller.maxHealth; // return a value 1 - 0 
        reverse = 1 - health; // return a value 0 - 1

        //COLOR APPLICATION
        color.r = reverse;
        color.g = health;

        //APPLY TO FACE
        healthFace.color = color;
    }
}
