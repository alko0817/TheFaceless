using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceState : MonoBehaviour
{
    public Sprite aggro, death;
    public Image bg;
    public Image state;
    public Animator anim;

    AIBehaviour controller;

    private void Start()
    {
        controller = GetComponent<AIBehaviour>();
        state.sprite = aggro;
        state.enabled = false;

    }

}
