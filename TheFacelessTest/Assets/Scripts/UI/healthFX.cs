﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class healthFX : MonoBehaviour
{
    PlayerHealth state;
    float health;
    float maxHealth;
    float healthClamp;
    float healthReverse;
    internal PostProcessVolume post;
    public float satIntensity;
    ColorGrading saturation;
    ChromaticAberration aberration;

    private void Start()
    {
        post = GameObject.FindGameObjectWithTag("Post").GetComponent<PostProcessVolume>();
        state = GetComponent<PlayerHealth>();
        post.profile.TryGetSettings(out saturation);
        post.profile.TryGetSettings(out aberration);
        maxHealth = state.maxHealth;
    }

    private void Update()
    {
        //SOME MATH
        health = state.currentHealth; //return current health
        healthClamp = health / maxHealth; //return current health but between 1-0
        healthReverse = 1 - healthClamp; // return current health but between 0-1

        //UPDATE VALUES
        aberration.intensity.value = healthReverse;
        saturation.saturation.value = healthReverse * -satIntensity;     
    }

}
