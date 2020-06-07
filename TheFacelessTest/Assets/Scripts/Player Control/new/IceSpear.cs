using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

public class IceSpear : DischargeAttack
{
    [Header("Components")]
    public Transform point;
    public GameObject lancer;
    [Header("Effects")]
    public float shakeDuration;
    public float shakeMagnitude;
    public float slowDuration;
    [Header("Frost Discharge Settings")]
    public float cooldown = 1f;
    public string dischargeAnimation;
    public float delay = 1f;
    public int initialDamage;
    [Space]
    public float duration = 20f;
    public int maxShots = 4;
    [Header("Shooting Settings")]
    public bool toggleAim;
    public float fireRate;
    public int spearDamage;
    public float aimSensitivity;

    private float originXsense;
    private float originYsense;
    private Animator crossAnim;
    private ParticleSystem spear;
    private Lancer particle;
    float shotCd = 0f;
    float timer = 0f;
    int shots = 0;
    bool shootMode = false;

    protected override void Start()
    {
        base.Start();
        timer = duration;
        spear = lancer.GetComponent<ParticleSystem>();
        particle = lancer.GetComponent<Lancer>();
        originXsense = controller.camSettings.xMouseSensitivity;
        originYsense = controller.camSettings.yMouseSensitivity;
        crossAnim = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (AllowShootMode() && !controller.shooting)
            {
                StartCoroutine(Attack(cooldown, dischargeAnimation, delay, initialDamage, controller.aoePoint, 0));
                InitShootMode();
            }
        }

        if (shootMode)
        {
            Aimer();
            Shoot();
            Detect();
            AllowSkill();
        }
    }
    private void Detect()
    {
        if (particle.enemy != null)
        {
            ApplyDamage();
        }
    }
    private void ApplyDamage ()
    {
        particle.enemy.GetComponent<EnemyBase>().TakeDamage(spearDamage);
        particle.enemy = null;
    }
    private void AttachLancer ()
    {
        lancer.transform.position = point.position;
        lancer.transform.rotation = Camera.main.transform.rotation;
    }
    private void Shoot()
    {
        shotCd -= Time.deltaTime;
        if (shotCd <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                shotCd = fireRate;
                AttachLancer();
                spear.Emit(1);
                shots++;
            }
        }
    }
    private void Aimer()
    {
        if (toggleAim)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (!controller.aiming)
                {
                    controller.aiming = !controller.aiming;
                    controller.cameraView.Aim();
                    controller.camSettings.xMouseSensitivity = aimSensitivity;
                    controller.camSettings.yMouseSensitivity = aimSensitivity;
                    crossAnim.SetTrigger("aim");
                }
                else
                {
                    controller.aiming = !controller.aiming;
                    controller.cameraView.Unaim();
                    controller.camSettings.xMouseSensitivity = originXsense;
                    controller.camSettings.yMouseSensitivity = originYsense;
                    crossAnim.SetTrigger("unaim");
                }
            }

        }

        else
        {
            if (Input.GetButtonDown("Fire2"))
            {
                controller.aiming = true;
                controller.cameraView.Aim();
                controller.camSettings.xMouseSensitivity = aimSensitivity;
                controller.camSettings.yMouseSensitivity = aimSensitivity;
                crossAnim.SetTrigger("aim");
            }

            else if (Input.GetButtonUp("Fire2"))
            {
                controller.aiming = false;
                controller.cameraView.Unaim();
                controller.camSettings.xMouseSensitivity = originXsense;
                controller.camSettings.yMouseSensitivity = originYsense;
                crossAnim.SetTrigger("unaim");
            }
        }
    }
    private void AllowSkill()
    {
        timer -= Time.deltaTime;
        if (shots > maxShots || timer <= 0f)
        {
            if (controller.aiming)
            {
                controller.aiming = false;
                controller.cameraView.Unaim();
                controller.camSettings.xMouseSensitivity = originXsense;
                controller.camSettings.yMouseSensitivity = originYsense;
                crossAnim.SetTrigger("unaim");
            }

            controller.shooting = false;
            shootMode = false;
            shotCd = 0;
            timer = duration;
        }
    }
    private void InitShootMode()
    {
        controller.shooting = true;
        shootMode = true;
        shots = 0;
        RestrainMovement(true);
        StartCoroutine(PlayAnimation());
        //change locomotion
    }
    IEnumerator PlayAnimation()
    {
        BlockMovement();
        yield return new WaitForSeconds(.3f);
        StartCoroutine(controller.camShake.Shake(shakeDuration, shakeMagnitude));
        yield return new WaitForSeconds(1.3f);
        controller.timeManager.slowmoDuration = slowDuration;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(.1f);
        AllowMovement();
    }
}
