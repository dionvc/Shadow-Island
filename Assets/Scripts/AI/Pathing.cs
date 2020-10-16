using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathing : MonoBehaviour
{
    [SerializeField] Grid tileGrid;
    [SerializeField] Tilemap[] unwalkableMaps;
    public PathNode GetPath(Vector3 start, Vector3 target, int pathTimeout)
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
                    if (i < 0 || j < 0) //Check if coords are outside worldSize
                    {
                        ignore = true;
                    }
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
                        bool walkable = true;
                        for(int k = 0; k < unwalkableMaps.Length; k++)
                        {
                            if(unwalkableMaps[k].HasTile(new Vector3Int(i,j, 0))) {
                                walkable = false;
                            }
                        }
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
}
