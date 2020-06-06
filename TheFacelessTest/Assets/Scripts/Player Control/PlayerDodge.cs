using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerDodge : MonoBehaviour
{
    playerController controller;

    public float dodgeCooldown = 1f;
    public float dodgeDashBoost = 4f;
    [Range(.01f, .5f)] 
    public float axisThreshold = .1f;
    [Range(.01f, .5f)]
    public float dodgeCost = .5f;
    public AudioClip DodgeSound;
    float dodgeCd = 0;
    float inputX;
    float inputZ;

    bool isRoll = false;

    private void Start()
    {
        controller = GetComponent<playerController>();
    }

    private void Update()
    {
        dodgeCd -= Time.deltaTime;

        inputZ = Input.GetAxis("Vertical");
        inputX = Input.GetAxis("Horizontal");

        //CHECK FOR LAST TIME DODGED
        if (dodgeCd <= 0 && !controller.attacking && !controller.stunned && !controller.health.dead)
        {
            if (controller.stamina.unit >= dodgeCost)
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
                        isRoll = true;
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Dodge"))
                    {
                        StartCoroutine(Dodge("dodgingRoll"));
                        isRoll = true;
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

        controller.stamina.Drain(dodgeCost);
        controller.SwordSounds.PlayOneShot(DodgeSound);
        controller.anim.SetTrigger(side);

        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed += dodgeDashBoost;
        yield return new WaitForSeconds(.3f);
        gameObject.GetComponent<vThirdPersonMotor>().strafeSpeed.walkSpeed = controller.originSpeed;

        yield return new WaitForSeconds(.5f);
        controller.dodging = false;
        controller.health.Immortality(false);
    }
}
