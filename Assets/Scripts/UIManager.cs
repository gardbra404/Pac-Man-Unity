using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Text scoreText, levelText, lifeText, pauseText;

    void Awake()
    {
        instance = this;    
    }

    public void UpdateUI()
    {
        scoreText.text = ""+GameManager.instance.GetScore();
        levelText.text = ""+GameManager.instance.GetLevel();
        lifeText.text = ""+GameManager.instance.GetLives();
    }

    public void DisplayPauseText(bool display)
    {
        if (display)
        {
            pauseText.text = "PAUSED";
        } 
        else
        {
            pauseText.text = "";
        }
    }
}
