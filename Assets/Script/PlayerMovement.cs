using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Stage stage;
    private Tile tile;
    public List<Tile> path = new List<Tile>();
    private int currentTileId;
    private int targetTileId;
    private int playerx = 3;
    private int playery = 3;
    private Vector3 startpos;
    private Vector3 endpos;
    private float moveTime;
    private float moveSpeed = 30f;
    private bool isMoving = false;
    private Coroutine coMove = null;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
        isMoving = false;
        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    private void Update()
    {
        var direction = Sides.None;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            direction = Sides.Top;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            direction = Sides.Bottom;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Sides.Right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Sides.Left;
        }

        if (direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            int targetTileId = stage.ScreenPosToTileId(Input.mousePosition);
            Tile startTile = stage.Map.tiles[currentTileId];
            Tile endTile = stage.Map.tiles[targetTileId];
            if (AStar(startTile, endTile))
            {
                StartCoroutine(CoMoving());
            }
        }
    }

    /*    public void MoveTo(int tileId)
        {
            currentTileId = tileId;
            transform.position = stage.GetTilePos(currentTileId);
            moveTime = 0;
            UpdateFow();
        }*/
    /*    public void MoveTo(int tileId)
        {

            currentTileId = tileId;
            transform.position = stage.GetTilePos(currentTileId);
            stage.OnTileVisited(tileId);

        }*/
    public void Warp(int tileId)
    {
        if (isMoving)
            return;
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        isMoving = false;
        targetTileId = -1;

        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        stage.OnTileVisited(currentTileId);
    }
    public void MoveTo(int tileId)
    {
        if (isMoving)
            return;

        targetTileId = tileId;
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        coMove = StartCoroutine(CoMove());
    }
    public void MoveTo(int x, int y)
    {

    }

/*    public void UpdateFow()
    {
        var x = currentTileId % stage.mapWidth;
        var y = currentTileId / stage.mapHeight;


        for (int j = y - playery; j <= y + playery; j++)
        {
            for (int i = x - playerx; i <= x + playerx; i++)
            {
                if (i < 0 || i >= stage.mapWidth || j < 0 || j >= stage.mapHeight) continue;
                int tileId = j * stage.mapWidth + i;
                stage.Map.tiles[tileId].isVisited = true;

            }
        }
        foreach (var tile in stage.Map.tiles)
        {
            tile.UpdateFowTileId();
        }
        stage.TilesMap();
    }*/
    private int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % stage.mapWidth;
        int ay = a.id / stage.mapWidth;

        int bx = b.id % stage.mapWidth;
        int by = b.id / stage.mapWidth;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
    public bool AStar(Tile startTile, Tile endTile)
    {
        foreach(var t in stage.Map.tiles)
        {
            t.previous = null;
        }
        path.Clear();
        var visited = new HashSet<Tile>();
        Dictionary<Tile, int> dist = new Dictionary<Tile, int>();
        var pq = new PriorityQueue<Tile, int>();
        dist[startTile] = 0;
        pq.Enqueue(startTile, 0 + Heuristic(startTile, endTile));

        bool succese = false;
        while (pq.Count > 0)
        {
            var current = pq.Dequeue();
            if (visited.Contains(current)) continue;
            visited.Add(current);
            if (current == endTile)
            {
                succese = true;
                break;
            }
            foreach (var adjacent in current.adjacents)
            {
                if (adjacent == null || !adjacent.CanMove||visited.Contains(adjacent)) continue;
                int distances = dist.ContainsKey(adjacent) ? dist[adjacent] : int.MaxValue;
                int newdist = dist[current] + adjacent.weight;
                if (distances > newdist)
                {   

                    dist[adjacent] = newdist;
                    adjacent.previous = current;
                    pq.Enqueue(adjacent, newdist + Heuristic(adjacent, endTile));
                }
            }
        }
        if (!succese)
        {
            return false;
        }
        Tile temp = endTile;

        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;
    }
/*    private IEnumerator MoveRoutine()
    {
        isMoving = true;
        animator.speed = 1;
        foreach (var current in path)
        {
            startpos = transform.position;
            endpos = stage.GetTilePos(current.id);
            moveTime = 0f;
            while (Vector3.Distance(transform.position, endpos) > 0.01f)
            {
                moveTime += Time.deltaTime / moveSpeed;
                transform.position = Vector3.Lerp(startpos, endpos, moveTime);

                yield return null;
            }
            transform.position = endpos;
            currentTileId = current.id;
            UpdateFow();
        }
        animator.speed = 0;

        isMoving = false;
    }*/

    private IEnumerator CoMove()
    {
        isMoving = true;
        animator.speed = 1;
        var startPos = transform.position;
        var endPos = stage.GetTilePos(targetTileId);
        var duration = Vector3.Distance(startPos, endPos) / moveSpeed;
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
        currentTileId = targetTileId;
        targetTileId = -1;
        animator.speed = 0;
        transform.position = endPos;
        stage.OnTileVisited(currentTileId);
        isMoving = false;
        coMove = null;
    }
    private IEnumerator CoMoving()
    {
        isMoving = true;
        foreach(var t in path)
        {
            targetTileId = t.id;

            yield return StartCoroutine(CoMove());
        }
        isMoving = false;
    }
}
