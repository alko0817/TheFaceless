using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedHealth : MonoBehaviour
{
    public GameObject face;
    EnemyBase controller;
    Animator anim;
    float health;

    private void Start()
    {
        anim = face.GetComponent<Animator>();
        controller = GetComponent<EnemyBase>();
    }

    private void Update()
    {
        health = controller.currentHealth / controller.maxHealth; // return value 1 - 0

        anim.SetFloat("health", health);
    }
}
