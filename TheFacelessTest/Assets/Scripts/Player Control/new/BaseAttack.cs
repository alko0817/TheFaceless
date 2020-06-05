using System.Collections;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    protected playerController controller;
    private float holder = 0;

    protected virtual void Start()
    {
        controller = GetComponent<playerController>();
    }
    protected virtual void Update()
    {
        controller.global -= Time.deltaTime;
    }

    protected void CheckHold ()
    {
        if (Input.GetButton("Fire1"))
        {
            holder += Time.deltaTime;
            if (holder >= .35f) SetHolding(true);
        }
        else
        {
            SetHolding(false);
            holder = 0;
        }
    }

    protected IEnumerator Attack(float cooldown, string animation, float attackDelay, int damage, Transform aoe, float cost, AudioClip sound)
    {
        controller.global = cooldown;
        controller.anim.SetTrigger(animation);
        controller.attacking = true;
        yield return new WaitForSeconds(attackDelay);
        if (sound != null) controller.SwordSounds.PlayOneShot(sound);
        controller.stamina.Drain(cost);

        //APPLY DPS
        Collider[] hitEnemies = Physics.OverlapBox(aoe.position, aoe.localScale / 2, Quaternion.identity, controller.enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyBase>().TakeDamage(damage);
            if (controller.isDischarge)
            {
                if (!enemy.GetComponent<EnemyBase>().GetStunned())
                    enemy.GetComponent<EnemyBase>().SetStunned(true);
            }
            else controller.Charge();
        }
        controller.attacking = false;
    }

    #region Checks
    protected void SetAttacking (bool value) { controller.attacking = value; }
    protected void SetBlocking (bool value) { controller.blocking = value; }
    protected void SetHolding (bool value) { controller.holding = value; }
    protected void SetDischarge (bool value) { controller.isDischarge = value; }
    protected bool canLightAttack()
    {
        if (controller.global <= 0 && !controller.holding && !controller.blocking && !controller.isDischarge) return true;
        else return false;
    }

    protected bool canHeavyAttack()
    {
        if (controller.global <= 0 && controller.holding && !controller.attacking) return true;
        else return false;
    }

    protected bool AllowAction()
    {
        if (controller.global <= 0) return true;
        else return false;
    }
    #endregion

}
