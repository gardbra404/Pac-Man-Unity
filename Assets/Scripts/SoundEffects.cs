using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public static SoundEffects instance;

    public AudioClip ghostDied;

    public AudioClip pacDied;

    private AudioSource audio;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        audio = this.gameObject.GetComponent<AudioSource>();
    }

    public void playPacDeath()
    {
        audio.clip = pacDied;
        audio.Play();
    }

    public void playGhostDied()
    {
        audio.clip = ghostDied;
        audio.Play();
    }
}
