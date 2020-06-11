using UnityEngine;

public class LightHeavyAttack : BaseAttack
{
    protected int combo = 0;
    protected bool check;
    protected float nextCombo = 0;
    [Header("Combo Cooldown")]
    public float holdComboFor = 1f;

    #region GroupVariables

    [Header("Attack land delay")]
    public float delay1 = 1f;
    public float delay2 = 1f;
    public float delay3 = 1f;
    public float delay4 = 1f;
    public float delay5 = 1f;
    [Space]
    public float heavyDelay = 1f;
    public float heavyDelay2 = 1f;
    [Header("Attack Cooldown")]
    public float cd1 = 1f;
    public float cd2 = 1f;
    public float cd3 = 1f;
    public float cd4 = 1f;
    public float cd5 = 1f;
    [Space]
    public float heavyCooldown = 1f;
    public float heavyCooldown2 = 1f;
    [Header("Damage")]
    public int dmg1 = 1;
    public int dmg2 = 1;
    public int dmg3 = 1;
    public int dmg4 = 1;
    public int dmg5 = 1;
    [Space]
    public int heavyDamage = 1;
    public int heavyDamage2 = 1;
    [Header("Animation sequence triggers")]
    public string anim1;
    public string anim2;
    public string anim3;
    public string anim4;
    public string anim5;
    [Space]
    public string heavyAnim;
    public string heavyAnim2;

    [Header("Stamina cost")]
    [Range(.01f, .5f)]
    public float cost = .2f;
    [Range(.02f, .6f)]
    public float heavyCost = 1f;

    [Header("Attack Sounds")]
    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip sound3;
    public AudioClip sound4;
    public AudioClip sound5;
    [Space]
    public AudioClip heavySound;
    public AudioClip heavySound2;
    #endregion

    private void Update()
    {
        nextCombo -= Time.deltaTime;
        ComboControl();
        if (CanLightAttack() && combo == 0)
        {
            LightAttack(cd1, anim1, delay1, dmg1, sound1);
        }

        if (canCombo())
        {
            if (combo == 1)
                LightAttack(cd2, anim2, delay2, dmg2, sound2);
            else if (combo == 2)
                LightAttack(cd3, anim3, delay3, dmg3, sound3);
            else if (combo == 3)
                LightAttack(cd4, anim4, delay4, dmg4, sound4);
            else if (combo == 4)
                LightAttack(cd5, anim5, delay5, dmg5, sound5);
        }

        if (nextCombo <= 0)
        {
            combo = 0;
            SetAttacking(false);
        }

        CheckHold();

        if (Input.GetButton("Fire1"))
        {
            if (CanHeavyAttack())
            {
                if (combo <= 2)
                {
                    HeavyAttack(heavyCooldown, heavyAnim, heavyDelay, heavyDamage, heavySound);
                }
                else
                {
                    HeavyAttack(heavyCooldown2, heavyAnim2, heavyDelay2, heavyDamage2, heavySound2);
                }
            }
        }
    }

    protected void LightAttack(float cooldown, string animation, float delay, int damage, AudioClip sound)
    {
        if (Input.GetButtonUp("Fire1"))
        {
            combo++;
            nextCombo = holdComboFor;
            StartCoroutine(Attack(cooldown, animation, delay, damage, controller.detectPoint, cost, sound));
        }
    }
    protected void HeavyAttack(float cooldown, string animation, float delay, int damage, AudioClip sound)
    {
        combo++;
        nextCombo = holdComboFor;
        StartCoroutine(Attack(cooldown, animation, delay, damage, controller.heavyPoint, heavyCost, sound));
        StartCoroutine(BlockMovement(cooldown));
    }
    protected bool canCombo()
    {
        if (controller.global <= 0 && nextCombo > 0) return true;
        else return false;
    }
    protected void ComboControl()
    {
        if (combo > 4) combo = 0;
    }
}
