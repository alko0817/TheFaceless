﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricDischarge : DischargeAttack
{
    public float dischargeForce = 100000f;
    public LayerMask movables;
    [Header("Effects")]
    public float shakeDuration;
    public float shakeMagnitude;
    public float slowDuration;
    public ParticleSystem explosion;
    [Space]
    public float cooldown = 1f;
    public string anim;
    public float delay = 1f;
    public int damage = 1;
    [Space]
    public AudioClip DischargeFirst;
    public AudioClip DischargeSecond;

    private bool check = false;
    protected override void Update()
    {
        base.Update();
        if (AllowAction() && AllowDischarge() && skillIndex == 1)
        {
            if (Input.GetButtonDown("discharge"))
            {
                StartCoroutine(Attack(cooldown, anim, delay, damage, controller.aoePoint, 0));
                StartCoroutine(Electric());
            }
        }
        if (check) controller.UIDischarge();
    }

    protected IEnumerator Electric()
    {
        check = true;
        controller.isDischarge = true;
        controller.canDischarge = false;
        controller.health.Immortality(true);
        yield return new WaitForSeconds(.7f);
        BlockMovement();

        yield return new WaitForSeconds(.3f);

        controller.fullCharge.Stop();
        controller.discharging = true;
        explosion.Play();
        controller.SwordSounds.PlayOneShot(DischargeFirst);

        if (controller.timeManager != null)
        {
            controller.timeManager.slowmoDuration = slowDuration;
            controller.timeManager.Slowmo();
        }
        controller.cameraView.ZoomOut();
        yield return new WaitForSeconds(.7f);
        controller.electricCharge.Stop();
        StartCoroutine(controller.camShake.Shake(shakeDuration, shakeMagnitude));
        Instantiate(controller.burst, controller.burstPoint.position, Quaternion.Euler(90, 0, 0));

        Collider[] props = Physics.OverlapSphere(controller.aoePoint.position, controller.aoeRadius, movables);
        foreach (Collider prop in props)
        {
            Rigidbody rb = prop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(dischargeForce, transform.position, controller.aoeRadius, 1f);
            }
        }
        float temp = controller.SwordSounds.volume;
        controller.SwordSounds.volume += .4f;
        controller.SwordSounds.PlayOneShot(DischargeSecond);

        yield return new WaitForSeconds(.8f);
        controller.cameraView.ZoomIn();
        controller.isDischarge = false;
        controller.SwordSounds.volume = temp;
        yield return new WaitForSeconds(1f);
        AllowMovement();
        controller.health.Immortality(false);
        controller.discharging = false;
        check = false;
    }
}
