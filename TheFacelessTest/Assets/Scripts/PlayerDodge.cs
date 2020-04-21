using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerDodge : MonoBehaviour
{
    playerController controller;

    float dodgeCooldown = 1f;
    float dodgeDashBoost = 4f;
    float axisThreshold = .1f;
    float dodgeCd = 0;

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

    IEnumerator Dodge (string side)
    {
        dodgeCd = dodgeCooldown;
        controller.anim.SetTrigger(side);
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;
        yield return new WaitForSeconds(.7f);
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;
    }
}
