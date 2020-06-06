using UnityEngine;

public class ClickSounds : MonoBehaviour
{
    AudioSource source;
    public AudioClip clip;
    public AudioClip playClick;


    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void ClickSound ()
    {
        source.PlayOneShot(clip);
    }

    public void ClickPlay ()
    {
        source.PlayOneShot(playClick);
    }
}
