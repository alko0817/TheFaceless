using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DischargeAttack : BaseAttack
{
    protected PlayerSkills skills;
    protected int skillIndex;
    protected int selected = -1;

    protected override void Start()
    {
        base.Start();
        skills = GameObject.FindGameObjectWithTag("Skills").GetComponent<PlayerSkills>();
    }
    protected virtual void Update()
    {
        skillIndex = skills.GetSkill();
        controller.ChargeCheck();
        if (controller.charged) ElementSwap(skillIndex);
        if (controller.discharging)
        {
            skills.BlockSkills(true);
            selected = 0;
        }
        else skills.BlockSkills(false);
    }
    private void ElementSwap (int skill)
    {
        if (skill == selected) return;
        selected = skill;
        Switcher();
    }
    private void Switcher()
    {
        switch (skillIndex)
        {
            case 1:
                controller.electricCharge.Play();
                controller.fireCharge.Stop();
                controller.frostCharge.Stop();
                break;
            case 2:
                controller.electricCharge.Stop();
                controller.fireCharge.Stop();
                controller.frostCharge.Play();
                break;
            case 3:
                controller.electricCharge.Stop();
                controller.fireCharge.Play();
                controller.frostCharge.Stop();
                break;
            case 4:
                //do nothing 
                break;
            default:
                Debug.LogWarning("Something is fucked... OR player hasn't selected a skill yet!");
                break;
        }
    }
    protected bool AllowDischarge() { return controller.canDischarge && controller.GetComponent<vThirdPersonMotor>().isGrounded; }

}

