using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Derive from AI?
public class AIAnimator : MonoBehaviour
{
    //Reference to AI 
    internal Animator an;

    //might use the ones from AI
    float speed;
    bool movement;

    bool shoot;
    bool attack;
    bool block;

    public float animSmooth;

    private void Start()
    {
        an = GetComponent<Animator>();
    }

    private void Update()
    {
        an.SetFloat(Animate.speed, speed, animSmooth, Time.deltaTime);

        if (shoot) an.SetTrigger(Animate.shoot);
        if (block) an.SetTrigger(Animate.hit);
        if (attack) an.SetTrigger(Animate.attack);
    }

    public static partial class Animate 
    {
        public static int shoot = Animator.StringToHash("shoot");
        public static int block = Animator.StringToHash("block");
        public static int attack = Animator.StringToHash("attack");
        public static int hit = Animator.StringToHash("hit");
        public static int speed = Animator.StringToHash("speed");
    }
}
