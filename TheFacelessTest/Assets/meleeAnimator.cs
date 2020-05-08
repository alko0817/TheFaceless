using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class meleeAnimator : AIAnimator
{
    private MeleeEnemy control;

    protected override void Start()
    {
        base.Start();
        control = GetComponent<MeleeEnemy>();
    }

    protected override void Update()
    {
        base.Update();
        if (control.dodging) an.SetTrigger(Animate.dodge);
        if (control.blocking) an.SetTrigger(Animate.block);

    }
}
