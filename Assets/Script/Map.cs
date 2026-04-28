using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Linq;
using Mono.Cecil;

public enum TileTypes
{
    Empty = -1,
    // 0~14
    grass = 15,
    Tree = 16,
    Hills = 17,
    Mountains = 18,
    Towns = 19,
    Castle = 20,
    Monster = 21,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;
    public Tile[] tiles;
    public Tile[] CoastTiles => tiles.Where(t=>t.autoTileId>=0&&t.autoTileId<(int)TileTypes.grass).ToArray();
    public Tile[] LandTiles => tiles.Where(t => t.autoTileId == (int)TileTypes.grass).ToArray();

    public Tile startTile;
    public Tile castleTile;

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
            
        }
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                var adjacents = tiles[index].adjacents;

                if ((r-1)>=0)
                {
                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }
                if ((c + 1) < cols)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }
                if ((c - 1) >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];

                }
                if ((r + 1) < rows)
                {
                    adjacents[(int)Sides.Bottom] =  tiles[index + cols];
                }
            }
        }
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
        }
        
    }

    public void ShuffleTiles(Tile[] tiles)
    {
        for(int i = tiles.Length-1 ; i>0 ; i--)
        {
            int rand = Random.Range(0, i+1);
            (tiles[rand], tiles[i]) = (tiles[i],tiles[rand]);
        }
    }

    public void DecorateTiles(Tile[] tiles,float percent, TileTypes tileType)
    {
        ShuffleTiles(tiles);

        int total = Mathf.FloorToInt(tiles.Length * percent);

        for (int i = 0; i < total; i++)
        {
            if(tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacents();
            }
            tiles[i].autoTileId = (int)tileType;
        }
    }
    public bool CreateIsland(float erodePercent,int erodIterations)
    {
        for(int i = 0;i<erodIterations;i++)
        {
            DecorateTiles(CoastTiles,erodePercent,TileTypes.Empty);
        }

        return true;
    }
    public bool CreateIsland(float erodePercent,int erodIterations,float lakePercent,float treePercent,float hillPercent,float mountainPercent,float townPercent,float monsterPercent)
    {
        for(int i = 0;i<erodIterations;i++)
        {
            DecorateTiles(CoastTiles,erodePercent,TileTypes.Empty);
        }
        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);
        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);
        var towns = tiles.Where(t => t.autoTileId == (int)TileTypes.Towns).ToArray();
        castleTile = towns[0];
        startTile = towns[Random.Range(1,towns.Length)];

        return true;
    }
}
