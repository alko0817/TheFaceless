using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endScene : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(scene());
    }


    IEnumerator scene()
    {
        yield return new WaitForSeconds(10f);
        anim.SetTrigger("nextScene");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);

    }
}
