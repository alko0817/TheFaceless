using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class swordGlow : MonoBehaviour
{
    playerController controller;
    public Image glow;
    Animator anim;
    bool charged = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
    }

    private void Update()
    {
        if (controller.swordFill.fillAmount == 1 && !charged)
        {
            anim.SetTrigger("charged");
            charged = true;
        }

        if (controller.discharging && charged)
        {
            anim.SetTrigger("discharging");
            charged = false;
        }


    }
}
