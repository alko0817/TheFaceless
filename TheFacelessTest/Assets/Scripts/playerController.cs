using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    public GameObject player;
    public Animator anim;

    //ENEMY DETECT
    [Header("Player Attack Point/Radius & Enemy Layer")]
    public Transform detectPoint;
    public LayerMask enemyLayer;
    public float attackRadius = .5f;

    //COMBAT
    
    int combos = 0;
    float lastClick = 0f;
    [Header("Attack Delays")]
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float nextAttack = 2f;
    float nextCombo = 0f;
    public bool attacking = false;

    [Header("Attack Damage")]
    public int slashDamage = 20;
    public int slash2Damage = 25;
    public int stabDamage = 40;
    public int dischargeDamage = 40;

    //DODGE
    [Header("Dodge Cooldown")]
    public float dodgeCooldown = 1f;
    float dodgeCd = 0;

    //DISCHARGE
    [Header("Discharge Mechanic")]
    public float maxCharge = 100f;
    public float chargeRate = 10f;
    float currentCharge = 0f;
    float lastCharge = 0f;
    public Image swordFill;
    bool canDischarge = false;


    //TESTING VARS
    


    void Update()
    {
        //TESTING GROUNDS

       


        
        


        //TESTING GROUNDS
        #region ATTACKS

        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;

        if (lastClick <= 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 0))
            {
                lastClick = attackDelay1;
                combos = 1;
                Slash();
                
                nextCombo = nextAttack;

            }

            if (Input.GetButtonDown("Fire2"))
            {
                lastClick = attackDelay1;
                Stab();
            }

            if (Input.GetButtonDown("discharge") && canDischarge)
            {
                lastClick = attackDelay1;
                Discharge();
            }

        }

        if (lastClick <= 0 && nextCombo > 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 1))
            {
                lastClick = attackDelay2;
                combos = 0;
                Slash2();
            }
        }

        if (nextCombo <= 0) combos = 0;

        #endregion

        #region DODGE
        dodgeCd -= Time.deltaTime;

        if (dodgeCd <=0)
        {
            if (Input.GetButtonDown("Dodge"))
            {
                dodgeCd = dodgeCooldown;
                Dodge();
            }
        }
        #endregion

    }

    void Dodge()
    {
        anim.SetTrigger("dodging");
    }

    #region AttackAnimations

    void Slash2()
    {
        anim.SetTrigger("isSlash2");
        DPS(slash2Damage);
        Charge();
        
    }

    void Slash ()
    {
        anim.SetTrigger("isSlash");
        DPS(slashDamage);
        Charge();

    }
    void Stab()
    {
        anim.SetTrigger("isStab");
        DPS(stabDamage);
        Charge();
    }

    void Discharge ()
    {
        anim.SetTrigger("discharge");
        DPS(dischargeDamage);
        canDischarge = false;
        swordFill.fillAmount = 0;
        currentCharge = 0;
    }
    #endregion


    void Charge()
    {
        if (currentCharge < maxCharge) currentCharge += chargeRate;
        else if (currentCharge >= maxCharge) currentCharge = maxCharge;


        float charged = currentCharge / maxCharge;
        swordFill.fillAmount = charged;

        

        if (currentCharge >= maxCharge) canDischarge = true;

    }


    void DPS (int damageDone)
    {

        //ENEMY DETECT
        Collider[] hitEnemies = Physics.OverlapSphere(detectPoint.position, attackRadius, enemyLayer);

        //APPLY DPS
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<enemyController>().takeDamage(damageDone);
        }
    }




    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null) return;
        Gizmos.DrawWireSphere(detectPoint.position, attackRadius);
    }
}
