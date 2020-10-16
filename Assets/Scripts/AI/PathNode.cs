using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public PathNode parentNode;
    public float cost;
    public float heuristic;
    public float begin;
    public float pathCost;
    public Vector3Int coords;
    public PathNode(PathNode parentNode, float frictionCost, float begin, float heuristic, Vector3Int coords)
    {
        this.parentNode = parentNode;
        this.coords = coords;
        this.cost = heuristic + begin + frictionCost;
        this.heuristic = heuristic;
        this.begin = begin;
        this.pathCost = frictionCost;
    }
}
