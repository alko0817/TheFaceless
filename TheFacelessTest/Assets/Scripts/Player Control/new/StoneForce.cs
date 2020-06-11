using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using UnityEngine;

public class StoneForce : DischargeAttack
{
    [Header("Initial Settings")]
    public float cooldown;
    public string dischargeAnim;
    public float delay;
    public int initDamage;
    public float duration;
    public float healthRegen = 1f;
    [Header("Stone Force Settings")]
    public float comboDuration;
    [Space]
    public float cd1 = 1f;
    public float cd2 = 1f;
    public float cd3 = 1f;
    public float slamCd = 1f;
    [Space]
    public string anim1;
    public string anim2;
    public string anim3;
    public string slamAnim;
    [Space]
    public float delay1 = 1f;
    public float delay2 = 1f;
    public float delay3 = 1f;
    public float slamDelay = 1f;
    [Space]
    public int dmg1 = 20;
    public int dmg2 = 20;
    public int dmg3 = 20;
    public int slamDmg = 20;
    [Range(.05f, .9f)]
    public float cost;

    private float comboTimer = 0f;
    private int combo = 0;
    private float timer;
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.R) && AllowAction() && AllowBerserkMode())
        {
            InitBerserkMode();
        }

        if (controller.berserk)
        {
            Berserk();
            CheckTimer();
        }
    }

    private void InitBerserkMode()
    {
        controller.berserk = true;
        timer = duration;
        StartCoroutine(Attack(cooldown, dischargeAnim, delay, initDamage, controller.aoePoint, 0));
        //health regen
        //initialize timer
        //initial anim & vfx
        //block basic attacks

    }
    private void CheckTimer()
    {
        timer -= Time.deltaTime;
        if (timer <=0)
        {
            controller.berserk = false;
            //prollly some more logic
        }
    }

    private void Berserk()
    {
        comboTimer -= Time.deltaTime;
        ComboControl();
        controller.health.PassiveRegen(healthRegen);
        if (AllowAction() && combo == 0)
        {
            stoneAttack(cd1, anim1, delay1, dmg1, cost);
        }

        if (AllowCombo())
        {
            if (combo == 1) stoneAttack(cd2, anim2, delay2, dmg2, cost);
            else if (combo == 2) stoneAttack(cd3, anim3, delay3, dmg3, cost);
        }

        if (comboTimer <= 0) combo = 0;

        if (AllowAction() && Input.GetButtonDown("Fire2"))
        {
            stoneHeavy(slamCd, slamAnim, slamDelay, slamDmg, 0);
        }

    }
    
    private void stoneAttack (float cooldown, string anim, float delay, int dmg, float cost)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            combo++;
            StartCoroutine(BlockMovement(delay));
            comboTimer = comboDuration;
            StartCoroutine(Attack(cooldown, anim, delay, dmg, controller.heavyPoint, cost));
        }
    }
    private void stoneHeavy(float cooldown, string anim, float delay, int dmg, float cost)
    {
        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(BlockMovement(delay));
            StartCoroutine(Attack(cooldown, anim, delay, dmg, controller.aoePoint, cost));
        }
    }
    private void ComboControl() { if (combo > 2) combo = 0;  }
    private bool AllowCombo() { return AllowAction() && comboTimer > 0; }

}
