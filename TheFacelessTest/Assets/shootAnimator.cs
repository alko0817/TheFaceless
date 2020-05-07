using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootAnimator : AIAnimator
{
    private ShooterEnemy control;

    protected override void Start()
    {
        base.Start();
        control = GetComponent<ShooterEnemy>();
    }

    protected override void Update()
    {
        base.Update();

        if (control.shootTimer > control.fireRate)
        {
            an.SetTrigger(Animate.shoot);
            Debug.Log("Im in here!");
        }

    }
}
