using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject wall;
    public GameObject pellet;
    public GameObject powerPellet;
    public GameObject bottomLeft, topRight;
    public GameObject tile;


    public TextAsset text;

    //TEMP
    public GameObject start, goal;

    public Node[,] grid;

    public List<Node> clydePath;
    public List<Node> blinkyPath;
    public List<Node> inkyPath;
    public List<Node> pinkyPath;

    public LayerMask unwalkable;

    int xStart, zStart;

    int xEnd, zEnd;

    int vCells, hCells;

    int cellWidth = 1;

    int cellHeight = 1;

    private void Awake()
    {
        BuildWorld();
        CreateGrid();
        //remove the artificially created pellet which prevents the game from immediately entering the win condition
        //since no pellets had been created yet
        //GameManager.instance.RunAfterStart();
    }

    void CreateGrid() {
        xStart = (int)bottomLeft.transform.position.x;
        zStart = (int)bottomLeft.transform.position.z;

        xEnd = (int)topRight.transform.position.x;
        zEnd = (int)topRight.transform.position.z;

        vCells = ((xEnd - xStart) / cellWidth);
        hCells = ((zEnd - zStart) / cellHeight);

        grid = new Node[hCells+1, vCells+1];

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        for (int i = 0; i <= hCells; i++)
        {
            for (int j = 0; j <= vCells; j++)
            {
                bool walkable = !(Physics.CheckSphere(new Vector3(xStart + i, 0, zStart + j), 0.4f, unwalkable));
                grid[i, j] = new Node(i, j, 0, walkable);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.red;
                if(clydePath!=null)
                {
                    if(clydePath.Contains(node))
                    {
                        Gizmos.color = Color.yellow;
                    }
                }
                if (inkyPath != null)
                {
                    if (inkyPath.Contains(node))
                    {
                        Gizmos.color = Color.cyan;
                    }
                }
                if (blinkyPath != null)
                {
                    if (blinkyPath.Contains(node))
                    {
                        Gizmos.color = Color.red;
                    }
                }
                if (pinkyPath != null)
                {
                    if (pinkyPath.Contains(node))
                    {
                        Gizmos.color = Color.magenta;
                    }
                }

                Gizmos.DrawWireCube(new Vector3(xStart + node.posX, 0.5f, zStart + node.posZ), new Vector3(0.8f, 0.8f, 0.8f));
            }
        }
    }

    public Node NodeRequest (Vector3 pos)
    {
        int gridX = (int)Vector3.Distance(new Vector3(pos.x, 0, 0), new Vector3(xStart, 0, 0));
        int gridZ = (int)Vector3.Distance(new Vector3(0, 0, pos.z), new Vector3(0, 0, zStart));
        return grid[gridX, gridZ];
    }

    public List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighborNodes = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0 ||
                    x == 1 && z == 1 ||
                    x == 1 && z == -1 ||
                    x == -1 && z == -1 ||
                    x == -1 && z == 1)
                {
                    continue;
                }

                int checkPosX = node.posX + x;
                int checkPosZ = node.posZ + z;

                if (checkPosX >= 0 && checkPosX <=(hCells) && checkPosZ >= 0 && checkPosZ < (vCells))
                {
                    neighborNodes.Add(grid[checkPosX, checkPosZ]);
                }
            }
        }
        return neighborNodes;
    }

    public Vector3 NextPathPoint(Node node)
    {
        int gridX = (int)(xStart + node.posX);
        int gridZ = (int)(zStart + node.posZ);

        return new Vector3(gridX, 0, gridZ);
    }

    public bool CheckInsideGrid(Vector3 requestedPosition)
    {
        int gridx = (int)(requestedPosition.x - xStart);
        int gridz = (int)(requestedPosition.z - zStart);

        if (gridx > hCells || gridx < 0 || gridz > vCells || gridz < 0 ||!NodeRequest(requestedPosition).walkable)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public Vector3 GetNearestNonWall(Vector3 target)
    {
        float min = 1000;
        int minIndexI = 0;
        int minIndexJ = 0;

        for (int i = 0; i < hCells; i++)
        {
            for (int j = 0; j < vCells; j++)
            {
                if(grid[i,j].walkable)
                {
                    Vector3 nextPoint = NextPathPoint(grid[i, j]);
                    float distance = Vector3.Distance(nextPoint, target);
                    if(distance < min)
                    {
                        min = distance;
                        minIndexI = i;
                        minIndexJ = j;
                    }
                }
            }
        }
        return NextPathPoint(grid[minIndexI, minIndexJ]);
    }

    private void BuildWorld()
    {
        //Note, when spawning the tiles, spawn them at y = 0.00001
        string[] fileLines = text.ToString().Split('\n');
        Vector3 currentMazePoint = new Vector3(this.bottomLeft.transform.position.x, this.bottomLeft.transform.position.y, this.topRight.transform.position.z);
        for (int i = 0; i < fileLines.Length; i++)
        {
            for (int j = 0; j < fileLines[i].Length; j++)
            {
                Vector3 tileVector = new Vector3(currentMazePoint.x, 0.0001f, currentMazePoint.z);
                if (fileLines[i][j] == '1')
                {
                    Instantiate(wall, currentMazePoint, Quaternion.identity);
                }
                else if (fileLines[i][j] == '0' && !Physics.CheckSphere(currentMazePoint, 0.4f))
                {
                    Instantiate(pellet, currentMazePoint, Quaternion.identity);
                    Instantiate(tile, tileVector, Quaternion.Euler(90, 0, 0));
                }
                else if (fileLines[i][j] == '0') 
                {
                    Instantiate(tile, tileVector, Quaternion.Euler(90, 0, 0));
                }
                else if (fileLines[i][j] == '2')
                {
                    Instantiate(powerPellet, currentMazePoint, Quaternion.identity);
                    Instantiate(tile, tileVector, Quaternion.Euler(90, 0, 0));
                } 
                currentMazePoint = new Vector3(currentMazePoint.x + 1, currentMazePoint.y, currentMazePoint.z);
            }
            currentMazePoint = new Vector3(this.bottomLeft.transform.position.x, currentMazePoint.y, currentMazePoint.z - 1);
        }
    }


}
