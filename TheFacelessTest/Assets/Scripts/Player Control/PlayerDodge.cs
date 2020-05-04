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
        if (dodgeCd <= 0)
        {
            if (controller.stamina.bar.fillAmount >= controller.dodgeCost)
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
        controller.health.Immortality(true);
        dodgeCd = dodgeCooldown;
        controller.stamina.bar.fillAmount -= controller.dodgeCost;
        controller.stamina.drainingDodge = true;
        controller.stamina.canRecharge = false;
        controller.SwordSounds.PlayOneShot(controller.DodgeSound);
        controller.anim.SetTrigger(side);
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;

        yield return new WaitForSeconds(.7f);
        controller.health.Immortality(false);
        controller.stamina.drainingDodge = false;
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
    }
}
