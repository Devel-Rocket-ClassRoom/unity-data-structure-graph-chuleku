using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public int id;
    public int weight = 1;
    public List<GraphNode> adjacents = new List<GraphNode>();
    public GraphNode previous = null;
    public bool CanVisit => adjacents.Count > 0 && weight > 0;

}
