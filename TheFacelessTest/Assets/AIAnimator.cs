using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    //Reference to AI 
    protected Animator an;
    protected EnemyBase controller;

    private float speed = 1f;
    private bool dead = false;

    public float animSmooth;


    protected virtual void Start()
    {
        controller = GetComponent<EnemyBase>();
        an = GetComponent<Animator>();
    }
    protected virtual void Update()
    {
        
        //DEATH
        if (controller.dying && !dead)
        {
            dead = true;
            an.SetTrigger(Animate.die);
        }

        if (dead) return;

        //LOCOMOTION
        speed = controller.navMeshAgent.speed;

        an.SetFloat(Animate.speed, speed, animSmooth, Time.deltaTime);

        //ACTIONS
    }

    public static partial class Animate 
    {
        public static int shoot = Animator.StringToHash("shoot");
        public static int block = Animator.StringToHash("block");
        public static int attack = Animator.StringToHash("attack");
        public static int hit = Animator.StringToHash("hit");
        public static int speed = Animator.StringToHash("speed");
        public static int die = Animator.StringToHash("die");
        public static int dodge = Animator.StringToHash("dodge");
    }
}
