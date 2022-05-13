using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//shhhhhhhhh, this class is super secret, it's my own little Easter Egg...because I could
public class FunTimes : MonoBehaviour
{
   
    bool metalIsPlaying = false;
    bool backgroundIsPlaying = true;
    AudioSource audio;
    public AudioClip mainAudio;
    public AudioClip metalAudio;
    float audioStartTime = 4;
    float audioTimer = 0;
    bool audioCanStart = false;
    // Start is called before the first frame update
    void Start()
    {
        audio = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioCanStart)
        {
            checkTimer();
        }
        else
        {
            if (!backgroundIsPlaying && Input.GetKeyDown(KeyCode.N))
            {
                audio.clip = mainAudio;
                metalIsPlaying = false;
                backgroundIsPlaying = true;
                audio.Play();
            }
            else if (!metalIsPlaying && Input.GetKeyDown(KeyCode.M))
            {
                audio.clip = metalAudio;
                metalIsPlaying = true;
                backgroundIsPlaying = false;
                audio.Play();
            }
        }
    }

    private void checkTimer()
    {
        audioTimer += Time.deltaTime;
        if (audioTimer >= audioStartTime)
        {
            audioCanStart = true;
            audio.Play();
        }
    }
}
