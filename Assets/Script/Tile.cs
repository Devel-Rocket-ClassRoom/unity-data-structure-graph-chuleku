using UnityEngine;

//0000 0001 0010 0011 0100 0101 0110 0111 1000 1001 1010 1011 1100 1101 1110 1111
public enum Sides
{
    None = -1,
    Bottom, //3
    Right, //2
    Left,  //1 
    Top,
}

public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;
    public int fowTileId;
    public bool isVisited = false;

    public bool CanMove => autoTileId !=(int)TileTypes.Empty;
    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for(int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] !=null)
            {
                //1000 0
                //0100 1
                //0010 2
                //0001 3
                autoTileId |= (1 << adjacents.Length - 1 - i);
            }
        }
    }
    public void UpdateFowTileId()
    {
        fowTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null && adjacents[i].isVisited==false)
            {
                fowTileId |= (1 << (adjacents.Length - 1 - i));
            }
        }
    }
    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null) continue;

            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;   
                UpdateAutoTileId();
                break;
            }
        }
    }
    public void ClearAdjacents()
    {
        for(int i = 0;i < adjacents.Length;i++)
        {
            if (adjacents[i] == null) continue;

            adjacents[i].RemoveAdjacents(this);
            adjacents[i] = null;
        }

        UpdateAutoTileId();
    }

}
