using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public float speed = 5f;

    Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    Vector3 nextPos, destination, direction;

    AudioSource audio;

    public LayerMask unwalkable;

    float audioTimer = 0;

    float playAudioTime = 4;

    bool audioCanStart;

    bool audioIsPlaying = false;

    float mouthAngleOffset = 0;

    public float mouthAngleOffsetAmount = 0.5f;

    bool mouthDown;

    public GameObject mouth;

    Vector3 initPosition;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        audio = this.gameObject.GetComponent<AudioSource>();
        audio.Stop();
        Reset();
    }

    public void Reset()
    {
        transform.position = initPosition;
        currentDirection = up;
        nextPos = Vector3.forward;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (!audioCanStart)
        {
            checkForAudioStart();
        }
        
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, (speed * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            nextPos = Vector3.forward;
            currentDirection = up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            nextPos = Vector3.back;
            currentDirection = down;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            nextPos = Vector3.left;
            currentDirection = left;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextPos = Vector3.right;
            currentDirection = right;
        }
        if (Vector3.Distance(destination, transform.position) < 0.00001f)
        {
            transform.localEulerAngles = currentDirection;
            if(Valid())
            {
                if (audioCanStart && !audioIsPlaying)
                {
                    audio.Play();
                    audioIsPlaying = true;
                }
                destination = transform.position + nextPos;
                direction = nextPos;    
            } 
            else
            {
                audio.Stop();
                audioIsPlaying = false;
            }
            moveMouth();
        }
    }

    bool Valid()
    {
        Ray myRay = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
        RaycastHit myHit;

        if(Physics.Raycast(myRay, out myHit, 1f, unwalkable))
        {
            if(myHit.collider.tag == "Wall")
            {
                return false;
            }
        }
        return true;
    }

    void checkForAudioStart()
    {
        audioTimer += Time.deltaTime;
        if (audioTimer >= playAudioTime)
        {
            audioCanStart = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ghost")
        {
            PathFinding pGhost = other.GetComponent<PathFinding>();
            if(pGhost.state == PathFinding.GhostStates.FRIGHTENED)
            {
                SoundEffects.instance.playGhostDied();
                pGhost.state = PathFinding.GhostStates.GOT_EATEN;
                GameManager.instance.AddScore(500);
                
            } else if (pGhost.state != PathFinding.GhostStates.GOT_EATEN && pGhost.state != PathFinding.GhostStates.FRIGHTENED)
            {
                SoundEffects.instance.playPacDeath();
                GameManager.instance.LoseLife();
            }
        }    
    }

    void moveMouth()
    {
        
        if (mouthDown)
        {
            mouthAngleOffset += mouthAngleOffsetAmount;
            mouth.transform.Rotate(new Vector3(0, 0, mouthAngleOffsetAmount));
            if (mouthAngleOffset >= 60)
            {
                mouthDown = false;
            }
        } else
        {
            mouthAngleOffset -= mouthAngleOffsetAmount;
            mouth.transform.Rotate(new Vector3(0, 0, -1*mouthAngleOffsetAmount));
            if (mouthAngleOffset <= 0)
            {
                mouthDown = true;
            }
        }
    }
}
