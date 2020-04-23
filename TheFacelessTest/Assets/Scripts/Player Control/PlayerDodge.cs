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

        float inputZ = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        //CHECK FOR LAST TIME DODGED
        if (dodgeCd <= 0)
        {
            if (controller.stamina.bar.fillAmount >= controller.dodgeCost)
            {
                if (Input.GetButtonDown("Dodge") && inputX < -axisThreshold)
                {
                    StartCoroutine(Dodge("dodgingLeft"));
                }

                if (Input.GetButtonDown("Dodge") && inputX > axisThreshold)
                {
                    StartCoroutine(Dodge("dodgingRight"));
                }

                if (Input.GetButtonDown("Dodge") && inputZ < -axisThreshold)
                {
                    StartCoroutine(Dodge("dodgingBack"));
                }

                if (Input.GetButtonDown("Dodge") && inputZ > axisThreshold)
                {
                    StartCoroutine(Dodge("dodgingRoll"));
                }
            }
        }
    }

    IEnumerator Dodge (string side)
    {
        dodgeCd = dodgeCooldown;
        controller.stamina.bar.fillAmount -= controller.dodgeCost;
        controller.stamina.canRecharge = false;
        controller.anim.SetTrigger(side);
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;

        yield return new WaitForSeconds(.7f);

        controller.stamina.canRecharge = true;
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
    }
}
