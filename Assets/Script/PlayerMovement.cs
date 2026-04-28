using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Stage stage;
    private int currentTileId;
    private int playerx = 3;
    private int playery = 3;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        var findGo = GameObject.FindWithTag("Map");
        stage =findGo.GetComponent<Stage>();

    }

    private void Update()
    {
        var direction = Sides.None;
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
          
            direction = Sides.Top;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
          
            direction = Sides.Bottom;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Sides.Right;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Sides.Left;
        }
        
        if(direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if(targetTile != null&&targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }
    }


    public void MoveTo(int tileId)
    {
        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        UpdateFow();
    }
    public void MoveTo(int x, int y)
    {
    }

    public void UpdateFow()
    {
        var x = currentTileId % stage.mapWidth;
        var y = currentTileId / stage.mapHeight;


        for (int j = y - playery; j <= y + playery; j++)
        {
            for (int i = x - playerx; i <= x + playerx; i++)
            {
                int tileId = j * stage.mapWidth + i;
                if (stage.GetTilePos(tileId) == null) continue;
                stage.Map.tiles[tileId].isVisited = true;

            }
        }
        foreach(var tile in stage.Map.tiles)
        {
            tile.UpdateFowTileId();
        }
        stage.TilesMap();
    }
}
