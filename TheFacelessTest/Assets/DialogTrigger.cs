using System.Collections;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    GameObject player;
    AudioSource sound;
    public bool answer = false;
    public AudioClip nicola;
    [Space]
    public Animator face;
    public GameObject greta;
    [Header("Subs")]
    public GameObject text;
    public float duration;
    Animator anim;

    bool triggered = false;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = text.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            if (!triggered)
            {
                triggered = true;
                Face();
                StartCoroutine(Subs());
                StartCoroutine(Dialog());
            }
        }
    }

    void Face()
    {
        greta.SetActive(true);
        face.SetTrigger("talk");
    }

    IEnumerator Dialog()
    {
        sound.Play();
        while (sound.isPlaying)
        {
            yield return null;
        }
        greta.SetActive(false);
        if (answer)
        {

            yield return new WaitForSeconds(1f);
            player.GetComponent<playerController>().Dialog(nicola);
        }

    }

    IEnumerator Subs ()
    {
        text.SetActive(true);
        anim.SetTrigger("fadeIn");
        yield return new WaitForSeconds(duration);

        anim.SetTrigger("fadeOut");
    }

}
