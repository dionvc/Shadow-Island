
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Pathing : MonoBehaviour
{
    #region Singleton
    private static Pathing instance;
    public static Pathing Instance
    {
        get
        {
            if (instance == null) Debug.LogError("No Instance of Pathing in the Scene!");
            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Just one Instance of Definitions allowed!");
        }
    }
    #endregion

    int[][] distToObstacle;

    [SerializeField] Grid tileGrid;
    /// <summary>
    /// Agent size is the width of the collider rounded up of the agent, or in the case of a group the largest collider's width
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="pathTimeout"></param>
    /// <param name="agentSize"></param>
    /// <returns></returns>
    public PathNode GetPath(Vector3 start, Vector3 target, int pathTimeout, int agentSize)
    {
        Vector3Int startCoords = tileGrid.WorldToCell(start);
        Vector3Int endCoords = tileGrid.WorldToCell(target);
        float heuristic = CalculateHeuristic(startCoords, endCoords);

        PathNode startNode = new PathNode(null, 0, 0, heuristic, startCoords);
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();
        PathNode endNode = null;
        openList.Add(startNode);
        while (openList.Count > 0 && pathTimeout > 0)
        {
            pathTimeout--;
            PathNode curNode = null;
            int removeIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (curNode == null || openList[i].cost < curNode.cost)
                {
                    removeIndex = i;
                    curNode = openList[i];
                }
            }
            openList.RemoveAt(removeIndex);
            closedList.Add(curNode);
            if (pathTimeout == 0 || (curNode.coords.x == endCoords.x && curNode.coords.y == endCoords.y))
            {
                endNode = curNode;
                break;
            }
            for (int i = curNode.coords[0] - 1; i <= curNode.coords[0] + 1; i++)
            {
                for (int j = curNode.coords[1] - 1; j <= curNode.coords[1] + 1; j++)
                {
                    bool ignore = false;
                    if (ignore == false) //else check that coords are not already in closedList
                    {
                        for (int k = 0; k < closedList.Count; k++)
                        {
                            if (closedList[k].coords[0] == i && closedList[k].coords[1] == j)
                            {
                                ignore = true;
                                break;
                            }
                        }
                    }
                    if (ignore == false) //else check that coords are not already in openList
                    {
                        for (int k = 0; k < openList.Count; k++)
                        {
                            if (openList[k].coords[0] == i && openList[k].coords[1] == j)
                            {
                                ignore = true;
                                break;
                            }
                        }
                    }
                    if (!ignore) //if neither of the preceding conditions set ignore to false, add tile to open list on path search
                    {
                        bool walkable = IsPathable(i, j, agentSize, curNode.coords.x, curNode.coords.y);
                        if (walkable == true)
                        {
                            Vector3Int coords = new Vector3Int(i ,j ,0);
                            PathNode newNode = new PathNode(curNode, 1, curNode.begin + 1, CalculateHeuristic(coords, endCoords), coords);
                            openList.Add(newNode);
                        }
                    }
                }
            }
        }
        return endNode;
    }

    private float CalculateHeuristic(Vector3 start, Vector3 target)
    {
        return (float)(start.x - target.x) * (start.x - target.x) + (start.y - target.y) * (start.y - target.y);
    }


    public void InitializePathingGrid(int sizeX, int sizeY)
    {
        int maxPathSize = Mathf.Min(sizeX, sizeY);
        distToObstacle = new int[sizeX][];
        for(int i = 0; i < sizeX; i++)
        {
            distToObstacle[i] = new int[sizeY];
            for (int j = 0; j < sizeY; j++)
            {
                distToObstacle[i][j] = maxPathSize;
            }
        }
    }
    /// <summary>
    /// Sets a square to show as unavailable for pathing
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetUnpathableLocation(int x, int y)
    {
        distToObstacle[x][y] = -1;
    }

    /// <summary>
    /// Calculates the pathablity of tiles based on static objects (essentially just cliffs and water for now)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CalculatePathablity(int sizeX, int sizeY)
    {
        //Can skip the very edges farthest edges of the map, we know they will be unpathable as they are water

        //First time, start from the bottom left
        for (int i = 1; i < sizeX - 1; i++)
        {
            for (int j = 1; j < sizeY - 1; j++)
            {
                if (distToObstacle[i][j] != -1)
                {
                    int[] values = new int[8];
                    values[0] = distToObstacle[i - 1][j - 1];
                    values[1] = distToObstacle[i - 1][j];
                    values[2] = distToObstacle[i - 1][j + 1];
                    values[3] = distToObstacle[i][j - 1];
                    values[4] = distToObstacle[i][j + 1];
                    values[5] = distToObstacle[i + 1][j - 1];
                    values[6] = distToObstacle[i + 1][j];
                    values[7] = distToObstacle[i + 1][j + 1];
                    distToObstacle[i][j] = Mathf.Min(values) + 2;
                }
            }
        }
        //Second time, start from the top right
        for(int i = sizeX - 2; i > 0; i--)
        {
            for(int j = sizeY - 2; j > 0; j--)
            {
                if (distToObstacle[i][j] != -1)
                {
                    int[] values = new int[8];
                    values[0] = distToObstacle[i - 1][j - 1];
                    values[1] = distToObstacle[i - 1][j];
                    values[2] = distToObstacle[i - 1][j + 1];
                    values[3] = distToObstacle[i][j - 1];
                    values[4] = distToObstacle[i][j + 1];
                    values[5] = distToObstacle[i + 1][j - 1];
                    values[6] = distToObstacle[i + 1][j];
                    values[7] = distToObstacle[i + 1][j + 1];
                    distToObstacle[i][j] = Mathf.Min(values) + 2;
                }
            }
        }
    }

    /// <summary>
    /// Agent size is the box colliders size rounded up
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="agentSize"></param>
    /// <returns></returns>
    public bool IsPathable(int x, int y, int agentSize, int prevX, int prevY)
    {
        //Detect if the new node is on a diagonal
        if(x - prevX != 0 && y - prevY != 0)
        {
            //Don't allow diagonal movement if moving diagonal would cause collision with corner of an obstacle
            if(x == prevX + 1 && y == prevY + 1 && (distToObstacle[prevX + 1][prevY] <= agentSize || distToObstacle[prevX][prevY+1] <= agentSize))
            {
                return false;
            }
            else if(x == prevX + 1 && y == prevY - 1 && (distToObstacle[prevX + 1][prevY] < agentSize || distToObstacle[prevX][prevY -1] < agentSize))
            {
                return false;
            }
            else if(x == prevX - 1 && y == prevY + 1 && (distToObstacle[prevX - 1][prevY] < agentSize || distToObstacle[prevX][prevY + 1] < agentSize))
            {
                return false;
            }
            else if(x == prevX - 1 && y == prevY - 1 && (distToObstacle[prevX - 1][prevY] < agentSize || distToObstacle[prevX][prevY - 1] < agentSize))
            {
                return false;
            }
        }
        return distToObstacle[x][y] >= agentSize;
    }
    /*
    private void OnDrawGizmos()
    {
        for(int i = 0; i < 96; i++)
        {
            for(int j = 0; j < 96; j++)
            {
                Vector3 position = new Vector3(i + 0.5f, j + 0.5f);
                Gizmos.DrawWireCube(position, new Vector3(0.8f, 0.8f, 0.2f));
                Handles.Label(position, distToObstacle[i][j].ToString());
            }
        }
    }
    */
}
