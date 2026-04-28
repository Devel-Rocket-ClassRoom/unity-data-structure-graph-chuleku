using System.ComponentModel;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefabs;
    private GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;
    public float lakePercent = 0.05f;
    public float treePercent = 0.08f;
    public float hillPercent = 0.1f;
    public float mountainPercent = 0.03f;
    public float townPercent = 0.2f;
    public float monsterPercent = 0.09f;

    public float distance = 100f;
    public Vector2 tileSize = new Vector2(16, 16);
    private int prevTileId = -1;
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;
    public Camera cam;
    private Map map;
    private Map tempMap;
    public PlayerMovement playerPrefab;
    private PlayerMovement player;
    private Vector3 tileposition
    {
        get
        {   
            Vector3 pos = transform.position;
            pos.x = -(mapWidth * tileSize.x / 2);
            pos.y = mapHeight * tileSize.y / 2;
            return pos;
        }
    }
    public Map Map => map;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResetStage();
            CreatePlayer();
        }
        if(tileObjs !=null)
        {
            int currentTileId = ScreenPosToTileId(Input.mousePosition);
            if (prevTileId != currentTileId)
            {
                tileObjs[currentTileId].GetComponent<SpriteRenderer>().color = Color.green;
                if(prevTileId>=0&&prevTileId<tileObjs.Length)
                {
                    tileObjs[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
                }
                prevTileId = currentTileId;
            }
        }
    }
    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent, treePercent, hillPercent, mountainPercent, townPercent, monsterPercent);
        CreateGrid();
    }
    private void CreatePlayer()
    {
        if(player !=null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);
        player.MoveTo(map.startTile.id);
    }
    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tileObj in tileObjs)
            {
                Destroy(tileObj.gameObject);
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];
        var position = tileposition;
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;
                var newGo = Instantiate(tilePrefabs, transform);
                newGo.transform.position = position;
                position.x += tileSize.x;
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            position.x = tileposition.x;
            position.y -= tileSize.y;
        }
    }
    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        if (tileGo == null) return;
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            if(!tile.isVisited)
            {
                ren.sprite = fowSprites[tile.fowTileId];
            }
            else
            {
                ren.sprite = islandSprites[tile.autoTileId];
            }
        }
        else
        {
            ren.sprite = null;
        }
    }
    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        return WorldPosToTileId(cam.ScreenToWorldPoint(screenPos));
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var first = tileposition;

        int x = Mathf.FloorToInt((worldPos.x-first.x)/tileSize.x+0.5f);
        int y = Mathf.FloorToInt((first.y- worldPos.y) /tileSize.y+0.5f);
        
        x = Mathf.Clamp(x,0,mapWidth-1);
        y = Mathf.Clamp(y,0,mapHeight-1);

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId/mapHeight,tileId%mapWidth);
    }

    public Vector3 GetTilePos(int y, int x) => tileposition + new Vector3(x*tileSize.x,-y*tileSize.y);

    public void TilesMap()
    {
        for (int i = 0; i < map.tiles.Length; i++)
        {
            DecorateTile(i);
        }
    }
}