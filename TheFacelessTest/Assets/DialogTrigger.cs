using System.Collections;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    GameObject player;
    AudioSource sound;
    public AudioClip entity;
    public AudioClip nicola;

    public Animator face;
    public GameObject greta;

    bool triggered = false;
    bool answer = false;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            if (!triggered)
            {
                triggered = true;
                Face();
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
        answer = true;
        greta.SetActive(false);
    }
}
