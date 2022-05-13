using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ReadScore : MonoBehaviour
{
    public Text highScore, level;
    // Start is called before the first frame update
    void Start()
    {
        highScore.text = "High Score: " + ScoreHolder.score;
        level.text = "Level: " + ScoreHolder.level;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }    
    }
}
