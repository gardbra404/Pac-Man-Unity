using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public enum Ghosts
    {
        BLINKY,
        CLYDE,
        INKY,
        PINKY
    }
    public Ghosts ghost;

    public Transform BlinkyLocation;

    List<Node> path = new List<Node>();

    int D = 10;
    Node lastVisitedNode;
    public Grid grid;

    public Transform currentTarget;
    public Transform pacManTarget;
    public List<Transform> homeTarget = new List<Transform>();
    public Transform frightenedTarget;
    public List <Transform> scatterTarget = new List<Transform>();
    public Transform leavingHomeTarget;

    public Material eye, iris;

    float speed = 3f;
    Vector3 nextPos, destination;

    Vector3 initPosition;
    GhostStates initState;

    Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currenDirection = Vector3.zero;
    public enum GhostStates
    {
        HOME,
        LEAVING_HOME,
        CHASE,
        SCATTER,
        FRIGHTENED,
        GOT_EATEN
    }
    public GhostStates state;

    int activeAppearance;
    public Material[] appearance;

    //Move timer
    float timer = 3f;
    float currTime = 0;

    public int pointsToCollect;
    public bool released;

    void Start()
    {
        initPosition = transform.position;
        initState = state;
        destination = transform.position;
    }

    void Update()
    {
        CheckState();
        //Timing();
    }

    void FindThePath()
    {
        
        Node startNode = grid.NodeRequest(transform.position);
        Node goalNode = grid.NodeRequest(currentTarget.position);

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (currentNode == goalNode)
            {
                PathTracer(startNode, goalNode);
                return;
            }
            foreach(Node neighborNode in grid.GetNeighborNodes(currentNode))
            {
                if(!neighborNode.walkable || closedList.Contains(neighborNode) || neighborNode == lastVisitedNode)
                {
                    continue;
                }
                int calcMoveCost = currentNode.gCost + GetDistance(currentNode, neighborNode);
                if(calcMoveCost < neighborNode.gCost || !openList.Contains(neighborNode))
                {
                    neighborNode.gCost = calcMoveCost;
                    neighborNode.hCost = GetDistance(neighborNode, goalNode);

                    neighborNode.parentNode = currentNode;
                    if(!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }

            }
            lastVisitedNode = null;
        }
    }

    void PathTracer(Node startNode, Node goalNode)
    {
        lastVisitedNode = startNode;
        path.Clear();
        Node currentNode = goalNode;
        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();
        if (ghost == Ghosts.BLINKY)
        {
            grid.blinkyPath = path;
        }
        if (ghost == Ghosts.PINKY)
        {
            grid.pinkyPath = path;
        }
        if (ghost == Ghosts.INKY)
        {
            grid.inkyPath = path;
        }
        if (ghost == Ghosts.CLYDE)
        {
            grid.clydePath = path;
        }
        //grid.path = path;
    }

    int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.posX - b.posX);
        int distZ = Mathf.Abs(a.posZ - b.posZ);

        return D * (distX + distZ);
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, destination) < 0.0001f)
        {
            FindThePath();
            if (path.Count > 0)
            {
                nextPos = grid.NextPathPoint(path[0]);
                destination = nextPos;

                SetDirection();
                transform.localEulerAngles = currenDirection;
            }
        }
    }


    void SetDirection()
    {
        int dirX = (int)(nextPos.x - transform.position.x);
        int dirZ = (int)(nextPos.z - transform.position.z);

        if(dirX == 0 && dirZ > 0)
        {
            currenDirection = up;
        } else if (dirX > 0 && dirZ == 0)
        {
            currenDirection = right;
        } else if (dirX < 0 && dirZ == 0)
        {
            currenDirection = left;
        } else if (dirX == 0 && dirZ < 0)
        {
            currenDirection = down;
        }
    }

    void CheckState()
    {
        switch (state) {
            case GhostStates.HOME:
                speed = 1.5f;
                homeLoop();
                Move();
                break;
            case GhostStates.LEAVING_HOME:
                break;
            case GhostStates.CHASE:
                speed = 3f;
                activeAppearance = 0;
                setAppearance();
                
                if (ghost == Ghosts.CLYDE)
                {
                    if (Vector3.Distance(transform.position, pacManTarget.position) <= 8)
                    {
                        scatterLoop();
                    } 
                    else
                    {
                        currentTarget = pacManTarget;
                    }
                }
                if (ghost == Ghosts.BLINKY)
                {
                    currentTarget = pacManTarget;
                }
                if (ghost == Ghosts.PINKY)
                {
                    PinkyBehavior();
                }
                if (ghost == Ghosts.INKY)
                {
                    InkyBehavior();
                }

                Move();
                break;
            case GhostStates.SCATTER:
                speed = 3f;
                scatterLoop();
                Move();
                break;
            case GhostStates.FRIGHTENED:
                activeAppearance = 1;
                setAppearance();
                speed = 1.5f;
                scatterLoop();
                Move();
                break;
            case GhostStates.GOT_EATEN:
                speed = 10f;
                currentTarget = homeTarget[0];
                if (Vector3.Distance(transform.position, homeTarget[0].position) < 0.0001f)
                {
                    state = GhostStates.HOME;
                }
                Move();
                activeAppearance = 2;
                setAppearance();
                break;
            default:
                break;
        }
    }

    void setAppearance()
    {
        for (int i = 0; i < appearance.Length; i++)
        {
            if (i == activeAppearance)
            {
                this.gameObject.GetComponentInChildren<MeshRenderer>().materials = new Material[3] { eye, iris, appearance[i] };
                break;
            }
        }
    }

    void scatterLoop()
    {
        if (!scatterTarget.Contains(currentTarget))
        {
            currentTarget = scatterTarget[0];
        }

        for (int i = 0; i < scatterTarget.Count; i++)
        {
            if (Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f && currentTarget == scatterTarget[i])
            {
                i++;
                if (i >= scatterTarget.Count)
                {
                    i = 0;
                }
                currentTarget = scatterTarget[i];
                continue;
            }
        }
    }
    void homeLoop()
    {
        if (!homeTarget.Contains(currentTarget))
        {
            currentTarget = homeTarget[0];
        }

        //allow for the ghost to just loop through all of the available targets
        for (int i = 0; i < homeTarget.Count; i++)
        {
            //check how close it is to the target, but also check if 
            if (currentTarget == homeTarget[i] && Vector3.Distance(transform.position, homeTarget[i].position) < 0.0001f)
            {
                i++;
                if (i >= homeTarget.Count)
                {
                    i = 0;
                }
                currentTarget = homeTarget[i];
                continue;
            }
        }

        if(released)
        {
            currTime = currTime + Time.deltaTime;
            if (currTime >= timer)
            {
                currTime = 0;
                state = GhostStates.CHASE;
            }
        }
    }

    void PinkyBehavior()
    {
        Transform aheadTarget = new GameObject().transform;
        int lookAhead = 4;
        //set the target
        for (int i = lookAhead; i > 0; i--)
        {
            if(!grid.CheckInsideGrid(aheadTarget.position))
            {
                lookAhead--;
                aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
            } else
            {
                break;
            }
        }
        aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
        Debug.DrawLine(transform.position, aheadTarget.position);
        currentTarget = aheadTarget;
        Destroy(aheadTarget.gameObject);
    }

    void InkyBehavior()
    {
        Transform blinkyToPacman = new GameObject().transform;
        Transform target = new GameObject().transform;
        Transform goal = new GameObject().transform;

        blinkyToPacman.position = new Vector3(pacManTarget.position.x - BlinkyLocation.position.x, 0, pacManTarget.position.z - BlinkyLocation.position.z);
        target.position = new Vector3(pacManTarget.position.x + blinkyToPacman.position.x, 0, pacManTarget.position.z + blinkyToPacman.position.z);

        goal.position = grid.GetNearestNonWall(target.position);
        currentTarget = goal;

        Debug.DrawLine(transform.position, currentTarget.position);

        Destroy(target.gameObject);
        Destroy(blinkyToPacman.gameObject);
        Destroy(goal.gameObject);
    }

    public void Reset()
    {
        transform.position = initPosition;
        state = initState;

        destination = transform.position;
        currenDirection = up;
    }
}
