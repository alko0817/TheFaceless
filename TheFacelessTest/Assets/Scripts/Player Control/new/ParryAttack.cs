using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryAttack : BaseAttack
{
    public ParticleSystem parryDeflect;
    [Header("Block")]
    public float blockCooldown = 1.1f;
    public string parryAnim;
    [Range(.01f, .5f)]
    public float parryCost;
    float cd;
    bool canReact = true;
    [Header("Reposte")]
    public bool allowSlowmotion = false;
    public float cooldown;
    public float delay;
    public int damage;
    public string reposteAnim;
    [Range(.01f, .5f)]
    public float reposteCost;
    public AudioClip swordSound;
    public AudioClip reposteSound;


    private void Update()
    {
        cd -= Time.deltaTime;
        if (controller.stamina.canBlock)
        {
            if (cd <= 0 && !controller.holding && canReact && !controller.discharging && !controller.shooting && !controller.berserk)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    StartCoroutine("Blocking");
                    cd = blockCooldown;
                }
            }

            if (controller.blocking)
            {
                Collider[] enemies = Physics.OverlapBox(controller.heavyPoint.position,
                    controller.heavyPoint.localScale / 2, Quaternion.identity, controller.enemyLayer);

                foreach (Collider enemy in enemies)
                {
                    if (enemy.GetComponent<MeleeEnemy>() == null) continue;

                    bool attacked = enemy.GetComponent<MeleeEnemy>().attackThrown;
                    if (attacked && canReact)
                    {
                        canReact = false;
                        StartCoroutine(Parrying(.2f, .3f, allowSlowmotion));
                        StartCoroutine(Attack(cooldown, reposteAnim, delay, damage, controller.heavyPoint, reposteCost, swordSound));
                        StartCoroutine(AttackSound(.1f, reposteSound));
                        break;
                    }

                }
            }
            else canReact = true;
        }
    }

    IEnumerator Blocking()
    {
        controller.anim.SetTrigger(parryAnim);
        BlockMovement();
        controller.health.Immortality(true);
        controller.blocking = true;
        controller.stamina.Drain(parryCost);

        yield return new WaitForSeconds(.1f);
        controller.blocking = false;

        yield return new WaitForSeconds(.5f);
        AllowMovement();

        yield return new WaitForSeconds(.4f);
        controller.health.Immortality(false);
    }

    IEnumerator Parrying(float delay, float duration, bool slow)
    {
        yield return new WaitForSeconds(delay);
        parryDeflect.Play();
        if (slow)
        {
            controller.timeManager.slowmoDuration = duration;
            controller.timeManager.Slowmo();
        }

        BlockMovement();
        yield return new WaitForSeconds(.7f);
        AllowMovement();
    }

    IEnumerator AttackSound(float connectDelay, AudioClip sound)
    {
        yield return new WaitForSeconds(connectDelay);
        //sounds.Play(sound, sounds.PlayerEffects);
        controller.SwordSounds.PlayOneShot(sound);
    }
}
