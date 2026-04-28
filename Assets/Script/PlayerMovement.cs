using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Stage stage;
    private int currentTileId;
   
    private void Awake()
    {
        animator = GetComponent<Animator>();

        var findGo = GameObject.FindWithTag("Map");
        stage =findGo.GetComponent<Stage>();
    }

    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
    }

    public void MoveTo(int tileId)
    {
        currentTileId = tileId;

        transform.position = stage.GetTilePos(currentTileId);
    }
    public void MoveTo(int x, int y)
    {

    }



}
