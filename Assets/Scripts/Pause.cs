using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !paused)
        {
            Time.timeScale = 0;
            paused = true;
            UIManager.instance.DisplayPauseText(paused);
        } 
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            paused = false;
            UIManager.instance.DisplayPauseText(paused);
        }
    }
}
