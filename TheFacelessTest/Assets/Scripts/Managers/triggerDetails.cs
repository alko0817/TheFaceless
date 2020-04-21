using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class triggerDetails
{
    [Tooltip("The name of the flashback. It is not important to the code")]
    public string name;

    [Tooltip("This index should be a unique number and has to be assigned on the trigger as well." +
        " You can simply make the local index of the trigger the same as this one")]
    public int index;

    [Tooltip("Assign here the panel as an object (drag and drop)")]
    public GameObject popUp;

    [Tooltip("For how long will the flashback stay")]
    public float duration;

    [Tooltip("The sound/music needs to be copy-pasted from the Audio Manager")]
    public string sound;

    //ADD SOME VFX 

    [HideInInspector]
    public Animator animator;

}
