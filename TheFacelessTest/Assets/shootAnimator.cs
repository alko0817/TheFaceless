using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootAnimator : AIAnimator
{
    private ShooterEnemy control;
    bool shoot;

    protected override void Start()
    {
        base.Start();
        control = GetComponent<ShooterEnemy>();
    }

    protected override void Update()
    {
        base.Update();

    }

    public void Shot()
    {
        an.SetTrigger(Animate.shoot);
    }
}
