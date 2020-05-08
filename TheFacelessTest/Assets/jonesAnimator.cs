using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jonesAnimator : AIAnimator
{
    StretchyJones control;

    protected override void Start()
    {
        base.Start();
        control = GetComponent<StretchyJones>();
    }

    protected override void Update()
    {
        base.Update();

    }
}
