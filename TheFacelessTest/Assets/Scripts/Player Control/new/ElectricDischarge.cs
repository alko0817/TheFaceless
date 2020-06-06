﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricDischarge : DischargeAttack
{
    public float dischargeForce = 100000f;
    public LayerMask movables;
    [Space]
    public float cooldown = 1f;
    public string anim;
    public float delay = 1f;
    public int damage = 1;
    [Space]
    public AudioClip DischargeFirst;
    public AudioClip DischargeSecond;
    protected override void Update()
    {
        base.Update();
        if (!AllowAction() || !AllowDischarge() || skillIndex != 1) return;

        if (Input.GetButtonDown("discharge"))
        {
            controller.isDischarge = true;
            controller.canDischarge = false;
            StartCoroutine(Attack(cooldown, anim, delay, damage, controller.aoePoint, 0));
            StartCoroutine(Electric());
        }
    }

    protected IEnumerator Electric()
    {
        controller.health.Immortality(true);

        yield return new WaitForSeconds(.7f);
        BlockMovement();

        yield return new WaitForSeconds(.3f);

        controller.fullCharge.Stop();
        controller.discharging = true;
        controller.explosion.Play();
        controller.SwordSounds.PlayOneShot(DischargeFirst);

        if (controller.timeManager != null)
        {
            controller.timeManager.slowmoDuration = controller.dischargeSlowDuration;
            controller.timeManager.Slowmo();
        }
        controller.foving.FovOut();

        yield return new WaitForSeconds(.7f);
        controller.electricityCharge.Stop();
        StartCoroutine(controller.camShake.Shake(controller.shakeDuration, controller.shakeMagnitude));
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
        controller.foving.FovIn();
        controller.isDischarge = false;
        controller.SwordSounds.volume = temp;
        yield return new WaitForSeconds(1f);
        AllowMovement();

        controller.health.Immortality(false);

        controller.discharging = false;
    }
}
