using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int pelletAmount;

    private static int score;

    private static int currentLevel = 1;
    private static int lives = 4;

    public List<GameObject> ghostList = new List<GameObject>();
    public GameObject pacman;

    //frightened timer
    float fTimer = 5f;
    float currFTimer = 0;

    //chase timer
    float cTimer = 20f;
    float currCTimer = 0;

    //scatter timer
    float sTimer = 7f;
    float currSTimer = 0;

    bool chase;
    bool scatter;
    public bool frightened;


    void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        if (score > ((currentLevel-1) * 3000))
        {
            lives++;
        }
        scatter = true;
        ghostList.AddRange(GameObject.FindGameObjectsWithTag("Ghost"));
        pacman = GameObject.FindGameObjectWithTag("Player");

        UIManager.instance.UpdateUI();
    }

    void Update()
    {
        Timing();   
    }

    public void AddPellet()
    {
        pelletAmount++;
    }

    public void ReducePellet(int amount)
    {
        pelletAmount--;
        score += amount;
        UIManager.instance.UpdateUI();
        if (pelletAmount == 0)
        {
            currentLevel++;
            WinCondition();
        }
        for (int i = 0; i < ghostList.Count; i++)
        {
            PathFinding pGhost = ghostList[i].GetComponent<PathFinding>();
            if (score>= pGhost.pointsToCollect && !pGhost.released)
            {
                pGhost.state = PathFinding.GhostStates.CHASE;
                pGhost.released = true;
            }
        }
    }

    void Timing()
    {
        UpdateStates();
        if (chase)
        {
            currCTimer = currCTimer + Time.deltaTime;
            if(currCTimer > cTimer)
            {
                currCTimer = 0;
                chase = false;
                scatter = true;
            }
        }
        if (scatter)
        {
            currSTimer = currSTimer + Time.deltaTime;
            if (currSTimer > sTimer)
            {
                currSTimer = 0;
                chase = true;
                scatter = false;
            }
        }
        if (frightened)
        {
            if (currCTimer != 0 || currSTimer != 0)
            {
                scatter = false;
                chase = false;
                currCTimer = 0;
                currSTimer = 0;
            }
            currFTimer = currFTimer + Time.deltaTime;
            if (currFTimer > fTimer)
            {
                currFTimer = 0;
                chase = true;
                scatter = false;
                frightened = false;
            }
        }
    }
    void UpdateStates()
    {
        for (int i = 0; i < ghostList.Count; i++)
        {
            PathFinding pGhost = ghostList[i].GetComponent<PathFinding>();
            if(pGhost.state == PathFinding.GhostStates.CHASE && scatter)
            {
                pGhost.state = PathFinding.GhostStates.SCATTER;
            } else if (pGhost.state == PathFinding.GhostStates.SCATTER && chase)
            {
                pGhost.state = PathFinding.GhostStates.CHASE;
            } else if (pGhost.state != PathFinding.GhostStates.HOME && frightened && pGhost.state != PathFinding.GhostStates.GOT_EATEN)
            {
                pGhost.state = PathFinding.GhostStates.FRIGHTENED;
            }else if (pGhost.state == PathFinding.GhostStates.FRIGHTENED)
            {
                pGhost.state = PathFinding.GhostStates.CHASE;
            }
        }
    }

    void WinCondition()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseLife()
    {
        lives--;
        if (lives <=0)
        {
            ScoreHolder.level = currentLevel;
            ScoreHolder.score = score;

            SceneManager.LoadScene("GameOver");
            // game over
            return;
        }
        foreach(GameObject ghost in ghostList) {
            ghost.GetComponent<PathFinding>().Reset();
        }
        pacman.GetComponent<PacMan>().Reset();
        UIManager.instance.UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        UIManager.instance.UpdateUI();
    }
    public int GetScore()
    {
        return score;
    }
    public int GetLevel()
    {
        return currentLevel;
    }
    public int GetLives()
    {
        return lives;
    }

    public void RunAfterStart()
    {
        pelletAmount--;
    }
}
