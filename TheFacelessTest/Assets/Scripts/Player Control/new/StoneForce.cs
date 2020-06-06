using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;

public class StoneForce : DischargeAttack
{
    [Header("Components")]
    public GameObject setStone;
    public GameObject[] stones;
    public Transform point;
    [Header("Stone Discharge Settings")]
    public float cooldown = 1f;
    public string dischargeAnimation;
    public float delay = 1f;
    public int initialDamage;
    [Space]
    public float duration = 20f;
    public int maxShots = 4;
    [Header("Shooting Settings")]
    public float fireRate;
    public float force;
    public int stoneDamage;

    float shotCd = 0f;
    float timer = 0f;
    int shots = 0;
    bool shootMode = false;

    protected override void Start()
    {
        base.Start();
        timer = duration;
        for (int i = 0; i < stones.Length; i++)
        {
            stones[i] = Instantiate(setStone);
            stones[i].SetActive(false);
        }
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
            AllowSkill();
        }
    }

    private void FixedUpdate()
    {
        if (shootMode)
        {
            shotCd -= Time.deltaTime;
            if (shotCd <= 0) Shoot();
        }
    }
    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            shotCd = fireRate;
            foreach (GameObject stone in stones)
            {
                if (!stone.activeInHierarchy)
                {
                    stone.SetActive(true);
                    Launch(stone);
                    shots++;
                    break;
                }
            }
        }
    }
    private void Launch (GameObject stone)
    {
        Rigidbody rb = stone.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
    private void Aimer()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            controller.cameraView.Aim();
        }

        else if (Input.GetButtonUp("Fire2"))
        {
            controller.cameraView.Unaim();
        }
    }
    private void AllowSkill()
    {
        timer -= Time.deltaTime;
        if (shots > maxShots || timer <= 0f)
        {
            controller.cameraView.Unaim();
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
        yield return new WaitForSeconds(1.6f);
        controller.timeManager.slowmoDuration = 2f;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(.1f);
        AllowMovement();
    }
}
