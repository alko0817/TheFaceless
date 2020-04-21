using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fbManager : MonoBehaviour
{
    public triggerDetails[] triggers;

    private void Awake()
    {
        foreach (triggerDetails trigger in triggers)
        {
            trigger.animator = trigger.popUp.GetComponent<Animator>();
        }
    }
}
