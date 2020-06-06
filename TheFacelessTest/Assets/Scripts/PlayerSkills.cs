using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    Animator anim;

    internal bool selected, eleOn, frostOn, fireOn, stoneOn = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !eleOn)
        {
            if (selected) selectSet("eleIn");
            else SetSkill("eleIn");

            eleOn = true;

        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !frostOn)
        {
            if (selected) selectSet("frostIn");
            else SetSkill("frostIn");

            frostOn = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !fireOn)
        {
            if (selected) selectSet("fireIn");
            else SetSkill("fireIn");

            fireOn = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && !stoneOn)
        {
            if (selected) selectSet("stoneIn");
            else SetSkill("stoneIn");

            stoneOn = true;
        }
    }



    void selectSet(string source)
    {
        if (eleOn)
        {
            anim.SetTrigger("eleOut");
            eleOn = false;
        }

        if (frostOn)
        {
            anim.SetTrigger("frostOut");
            frostOn = false;
        }

        if (fireOn)
        {
            anim.SetTrigger("fireOut");
            fireOn = false;
        }

        if (stoneOn)
        {
            anim.SetTrigger("stoneOut");
            stoneOn = false;
        }

        SetSkill(source);
    }

    void SetSkill(string source)
    {
        anim.SetTrigger(source);
        selected = true;
    }

    public int GetSkill()
    {
        if (eleOn)
        {
            return 1;
        }

        else if (frostOn)
        {
            return 2;
        }

        else if (fireOn)
        {
            return 3;
        }

        else if (stoneOn)
        {
            return 4;
        }

        return 0;
    }
}
