using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerDodge : MonoBehaviour
{
    playerController controller;

    private float dodgeCooldown = 1f;
    private float dodgeDashBoost = 4f;
    private float axisThreshold = .1f;
    private float dodgeCd = 0;
    float inputX;
    float inputZ;

    private void Start()
    {
        controller = GetComponent<playerController>();

        dodgeCooldown = controller.dodgeCooldown;
        dodgeDashBoost = controller.dodgeDashBoost;
        axisThreshold = controller.axisThreshold;
        dodgeCd = controller.dodgeCd;
    }

    private void Update()
    {
        dodgeCd -= Time.deltaTime;

        inputZ = Input.GetAxis("Vertical");
        inputX = Input.GetAxis("Horizontal");

        //CHECK FOR LAST TIME DODGED
        if (dodgeCd <= 0 && !controller.attacking && !controller.stunned && !controller.health.dead)
        {
            if (controller.stamina.unit >= controller.dodgeCost)
            {
                if (!controller.sprinting)
                {
                    if (Input.GetButtonDown("Dodge") && inputX < -axisThreshold)
                    {
                        StartCoroutine(Dodge("dodgingLeft"));
                    }

                    else if (Input.GetButtonDown("Dodge") && inputX > axisThreshold)
                    {
                        StartCoroutine(Dodge("dodgingRight"));
                    }

                    else if (Input.GetButtonDown("Dodge") && inputZ < -axisThreshold)
                    {
                        StartCoroutine(Dodge("dodgingBack"));
                    }

                    else if (Input.GetButtonDown("Dodge") && inputZ > axisThreshold)
                    {
                        StartCoroutine(Dodge("dodgingRoll"));
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Dodge"))
                    {
                        StartCoroutine(Dodge("dodgingRoll"));
                    }
                }
            }
        }
    }

    IEnumerator Dodge (string side)
    {
        controller.dodging = true;
        controller.health.Immortality(true);
        dodgeCd = dodgeCooldown;

        controller.stamina.Drain(controller.dodgeCost);

        controller.SwordSounds.PlayOneShot(controller.DodgeSound);

        controller.anim.SetTrigger(side);

        //gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;

        yield return new WaitForSeconds(.7f);

        controller.dodging = false;
        controller.health.Immortality(false);
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
    }
}
