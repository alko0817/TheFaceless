using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DischargeAttack : BaseAttack
{
    protected PlayerSkills skills;
    protected int skillIndex;

    protected override void Start()
    {
        base.Start();
        skills = GameObject.FindGameObjectWithTag("Skills").GetComponent<PlayerSkills>();
    }
    protected virtual void Update()
    {
        skillIndex = skills.GetSkill();
    }

    protected bool AllowDischarge()
    {
        if (controller.canDischarge && controller.GetComponent<vThirdPersonMotor>().isGrounded) return true;
        else return false;
    }

}

