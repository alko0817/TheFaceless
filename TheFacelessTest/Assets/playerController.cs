using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public GameObject player;
    public Animator anim;

    //ENEMY DETECT
    public Transform detectPoint;
    public LayerMask enemyLayer;
    public float attackRadius = .5f;

    //COMBAT
    int combos = 0;
    float lastClick = 0f;
    public float attackDelay1 = 1.5f;
    public float attackDelay2 = 1.5f;
    public float nextAttack = 2f;
    float nextCombo = 0f;
    public bool attacking = false;

    //DODGE
    public float dodgeCooldown = 1f;
    float dodgeCd = 0;

  
    void Update()
    {


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
    void Slash2()
    {
        anim.SetTrigger("isSlash2");
        //anim.SetBool("attacking", true);
        
    }

    void Slash ()
    {
       //ANIMATION PLAY
        anim.SetTrigger("isSlash");


        //ENEMY DETECT
        Physics.OverlapSphere(detectPoint.position, attackRadius, enemyLayer);

        //DPS

    }

    void Stab()
    {
        anim.SetTrigger("isStab");
    }


    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null) return;
        Gizmos.DrawWireSphere(detectPoint.position, attackRadius);
    }
}
