using UnityEngine;
using UnityEngine.Analytics;

public class Graph
{
    public int row = 0;
    public int col = 0;

    public GraphNode[] nodes;

    public void Init(int[,] grid)
    {
        row = grid.GetLength(0);
        col = grid.GetLength(1);

        nodes = new GraphNode[grid.Length];
        for(int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new GraphNode();
            nodes[i].id = i;
        }

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                int index = i * col + j;
                nodes[index].weight = grid[i,j];

                if (grid[i,j] == -1)
                {
                    continue;
                }

                if (i - 1 >= 0 && grid[i-1,j]>=0)
                {
                    nodes[index].adjacents.Add(nodes[index - col]);
                }
                if (j + 1 < col && grid[i,j+1]>=0)
                {
                    nodes[index].adjacents.Add(nodes[index + 1]);
                }
                if (i + 1 < row && grid[i+1,j]>=0)
                {
                    nodes[index].adjacents.Add(nodes[index + col]);
                }
                if (j- 1 >= 0 && grid[i,j-1]>=0)
                {
                    nodes[index].adjacents.Add(nodes[index -1]);
                }


            }
        }
    }

    public void ResetNodePrevious()
    {
        foreach(var node in nodes)
        {
            node.previous = null;
        }
    }
}
