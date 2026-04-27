using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();
    public void Init(Graph graph)
    {
        this.graph = graph;
    }
    public void DFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();

        var stack = new Stack<GraphNode>();
        
        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if(!adjacent.CanVisit||visited.Contains(adjacent)) continue;

                visited.Add(adjacent);
                stack.Push(adjacent);
            }
        }
    }
    public void BFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);

            foreach(var adjacent in current.adjacents)
            {
                if(!adjacent.CanVisit ||visited.Contains(adjacent)) continue;

                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }
    }
    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        DFSRecursive(node,visited);
    }
    protected void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);
        foreach(var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent)) continue;

            DFSRecursive(adjacent, visited);
        }
    }
    public bool PathFindingBFS(GraphNode startnode,GraphNode endnode)
    {
        path.Clear();
        graph.ResetNodePrevious();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(startnode);
        visited.Add(startnode);
        bool succese = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == endnode)
            {
                succese = true;
                break;
            }
            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent)) continue;

                visited.Add(adjacent);
                adjacent.previous = current;
                queue.Enqueue(adjacent);
            }
        }

        if(!succese)
        {
            return false;
        }
        GraphNode temp = endnode;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;
    }
    public bool Dijkstra(GraphNode startnode, GraphNode endnode)
    {
        path.Clear();
        graph.ResetNodePrevious();
        var visited = new HashSet<GraphNode>();
        Dictionary<GraphNode, int> dist = new Dictionary<GraphNode, int>();
        var pq = new PriorityQueue<GraphNode,int>();
        dist[startnode] = 0;
        pq.Enqueue(startnode,0);

        bool succese = false;
        while (pq.Count > 0)
        {
            var current = pq.Dequeue();
            if(visited.Contains(current)) continue;
            visited.Add(current);
            if (current == endnode)
            {
                succese = true;
                break;
            }
            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit) continue;
                int distances = dist.ContainsKey(adjacent) ? dist[adjacent] : int.MaxValue;
                int newdist = dist[current] + adjacent.weight;
                if (distances > newdist)
                {
                    dist[adjacent] = newdist;
                    adjacent.previous = current;
                    pq.Enqueue(adjacent, newdist);
                }
            }
        }
        if (!succese)
        {
            return false;
        }
        GraphNode temp = endnode;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;

    }
    public bool Dijkstras(GraphNode startnode, GraphNode endnode)
    {
        path.Clear();
        graph.ResetNodePrevious();
        var visited = new HashSet<GraphNode>();
        Dictionary<GraphNode, int> dist = new Dictionary<GraphNode, int>();
        var distance = new int[graph.nodes.Length];
        var pq = new PriorityQueue<GraphNode,int>();
        for(int i = 0;  i < distance.Length; i++)
        {
            distance[i] = int.MaxValue;
        }
        distance[startnode.id] = 0;
        pq.Enqueue(startnode, distance[startnode.id]);

        bool succese = false;
        while (pq.Count > 0)
        {
            var current = pq.Dequeue();
            if (current == endnode)
            {
                succese = true;
                break;
            }
            visited.Add(current);
            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit|| visited.Contains(current)) continue;
                var newDistance = distance[current.id] + adjacent.weight;
                if (newDistance < distance[adjacent.id])
                {
                    distance[adjacent.id] = newDistance;
                    adjacent.previous = current;
                    pq.Enqueue(adjacent, newDistance);
                }
            }
        }
        if (!succese)
        {
            return false;
        }
        GraphNode temp = endnode;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;

    }
    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.col;
        int ay = a.id / graph.col;

        int bx = b.id % graph.col;
        int by = b.id / graph.col;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
    public bool AStar(GraphNode startnode, GraphNode endnode)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        Dictionary<GraphNode, int> dist = new Dictionary<GraphNode, int>();
        var pq = new PriorityQueue<GraphNode, int>();
        dist[startnode] = 0;
        pq.Enqueue(startnode, 0+Heuristic(startnode,endnode));
   
        bool succese = false;
        while (pq.Count > 0)
        {
            var current = pq.Dequeue();
            if (visited.Contains(current)) continue;
            visited.Add(current);
            if (current == endnode)
            {
                succese = true;
                break;
            }
            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit) continue;
                int distances = dist.ContainsKey(adjacent) ? dist[adjacent] : int.MaxValue;
                int newdist = dist[current] + adjacent.weight;
                if (distances > newdist)
                {
                    dist[adjacent] = newdist;
                    adjacent.previous = current;
                    pq.Enqueue(adjacent, newdist+Heuristic(adjacent,endnode));
                }
            }
        }
        if (!succese)
        {
            return false;
        }
        GraphNode temp = endnode;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;

    }
}
