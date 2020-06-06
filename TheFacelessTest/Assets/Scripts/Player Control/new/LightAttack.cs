using UnityEngine;

public class LightAttack : BaseAttack
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
    public float heavyDelay = 1f;
    [Header("Attack Cooldown")]
    public float cd1 = 1f;
    public float cd2 = 1f;
    public float cd3 = 1f;
    public float cd4 = 1f;
    public float cd5 = 1f;
    public float heavyCooldown = 1f;
    [Header("Damage")]
    public int dmg1 = 1;
    public int dmg2 = 1;
    public int dmg3 = 1;
    public int dmg4 = 1;
    public int dmg5 = 1;
    public int heavyDamage = 1;
    [Header("Animation sequence triggers")]
    public string anim1;
    public string anim2;
    public string anim3;
    public string anim4;
    public string anim5;
    public string heavyAnim;

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
    public AudioClip heavySound;
    #endregion

    private void Update()
    {
        nextCombo -= Time.deltaTime;

        if (CanLightAttack() && combo == 0)
        {
            Attack(1, cd1, anim1, delay1, dmg1, sound1);
        }

        if (canCombo())
        {
            if (combo == 1)
                Attack(2, cd2, anim2, delay2, dmg2, sound2);
            else if (combo == 2)
                Attack(3, cd3, anim3, delay3, dmg3, sound3);
            else if (combo == 3)
                Attack(4, cd4, anim4, delay4, dmg4, sound4);
            else if (combo == 4)
                Attack(0, cd5, anim5, delay5, dmg5, sound5);
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
                StartCoroutine(Attack(heavyCooldown, heavyAnim, heavyDelay,
                heavyDamage, controller.heavyPoint, heavyCost, heavySound));
            }
        }
    }

    protected void Attack(int comboCounter, float cooldown, string animation, float delay, int damage, AudioClip sound)
    {
        if (Input.GetButtonUp("Fire1"))
        {
            combo = comboCounter;
            StartCoroutine(Attack(cooldown, animation, delay, damage, 
                controller.detectPoint, cost, sound));
            nextCombo = holdComboFor;
        }
    }

    protected bool canCombo()
    {
        if (controller.global <= 0 && nextCombo > 0) return true;
        else return false;
    }
}
