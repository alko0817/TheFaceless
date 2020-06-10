using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpear : DischargeAttack
{
    [Header("Components")]
    public Transform point;
    public GameObject lancer;
    [Header("Effects")]
    public float shakeDuration;
    public float shakeMagnitude;
    public float slowDuration;
    public ParticleSystem[] frostHands;
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
    public float animRate;
    public int spearDamage;
    public float aimSensitivity;

    private float originXsense;
    private float originYsense;
    private Animator crossAnim;
    private ParticleSystem spear;
    private Lancer particle;
    float shotCd = 0f;
    float animCd = 0f;
    float timer = 0f;
    int shots = 0;
    bool shootMode = false;
    int sequence = 0;

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
        if (AllowDischarge() && AllowAction() && skillIndex == 2)
        {
            if (Input.GetButtonDown("discharge"))
            {
                if (AllowShootMode())
                {
                    controller.frostCharge.Stop();
                    StartCoroutine(Attack(cooldown, dischargeAnimation, delay, initialDamage, controller.aoePoint, 0));
                    InitShootMode();
                }
            }
        }

        if (shootMode)
        {
            controller.UIDischarge(timer, duration);
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
        if (!controller.aiming) return;

        shotCd -= Time.deltaTime;
        animCd -= Time.deltaTime;
        if (shotCd <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                AnimControl();
                StartCoroutine(ShotDelay());
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
                    //controller.anim.SetTrigger("aim");
                    //controller.anim.SetBool("shootMode", true);
                }
                else
                {
                    //controller.anim.SetBool("shootMode", false);
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
                controller.anim.SetBool("shootMode", false);
                controller.aiming = false;
                controller.cameraView.Unaim();
                controller.camSettings.xMouseSensitivity = originXsense;
                controller.camSettings.yMouseSensitivity = originYsense;
                crossAnim.SetTrigger("unaim");
            }

            controller.shooting = false;
            controller.discharging = false;
            shootMode = false;
            foreach (ParticleSystem hand in frostHands) hand.Stop();
            shotCd = 0;
            timer = duration;
        }
    }
    private void InitShootMode()
    {
        controller.canDischarge = false;
        controller.discharging = true;
        controller.shooting = true;
        shootMode = true;
        shots = 0;
        RestrainMovement(true);
        StartCoroutine(PlayAnimation());
        foreach (ParticleSystem hand in frostHands) hand.Play();
        //change locomotion
    }
    private void AnimControl()
    {
        if (sequence > 1 || animCd <= 0) sequence = 0;
        switch (sequence)
        {
            case 0:
                controller.anim.SetTrigger("shoot");
                break;
            case 1:
                controller.anim.SetTrigger("shoot2");
                break;
        }
        animCd = animRate;
        sequence++;
    }
    IEnumerator ShotDelay()
    {
        shotCd = fireRate;
        yield return new WaitForSeconds(.25f);
        AttachLancer();
        spear.Emit(1);
        shots++;
    }
    IEnumerator PlayAnimation()
    {
        BlockMovement();
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(controller.camShake.Shake(shakeDuration, shakeMagnitude));
        controller.timeManager.slowmoDuration = slowDuration;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(.3f);
        AllowMovement();
    }
}
