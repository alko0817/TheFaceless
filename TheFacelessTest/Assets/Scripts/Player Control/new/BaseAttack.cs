using System.Collections;
using UnityEngine;
using Invector.vCharacterController;

public class BaseAttack : MonoBehaviour
{
    protected playerController controller;
    private float holder = 0;

    protected virtual void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
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

    protected IEnumerator Attack(float cooldown, string animation, float attackDelay, int damage, Transform aoe, float cost)
    {
        controller.global = cooldown;
        controller.anim.SetTrigger(animation);
        controller.attacking = true;
        yield return new WaitForSeconds(attackDelay);
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

    #region Checks'n'Passes
    protected void SetAttacking (bool value) { controller.attacking = value; }
    protected void SetBlocking (bool value) { controller.blocking = value; }
    protected void SetHolding (bool value) { controller.holding = value; }
    protected void SetDischarge (bool value) { controller.isDischarge = value; }
    protected bool CanLightAttack() { return controller.global <= 0 && !controller.holding && !controller.blocking && !controller.isDischarge; }
    protected bool CanHeavyAttack() { return controller.global <= 0 && controller.holding && !controller.attacking; }
    protected bool AllowAction() { return controller.global <= 0; }
    protected void BlockMovement() { controller.GetComponent<vThirdPersonMotor>().stopMove = true; }
    protected void AllowMovement() { controller.GetComponent<vThirdPersonMotor>().stopMove = false; }
    public bool GetAttack() { return controller.attacking; }
    #endregion

}
