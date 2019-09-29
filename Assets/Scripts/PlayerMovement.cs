using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    Vector2 playerSize;
    Boundary playerBounds;
    PlayerBehaviour player;
    bool invertControls;
        
    private void Awake() {
        ScalePlayerForScreen();
    }

    void Start()
    {
        playerSize = GetComponent<SpriteRenderer>().bounds.extents;
        playerBounds = Boundary.ScreenBoundary(playerSize);
        player = GetComponent<PlayerBehaviour>();
    }

    
    void Update()
    {
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            Vector2 pos = touch.position;
            pos = Camera.main.ScreenToWorldPoint(pos);
            if(touch.phase == TouchPhase.Began){
                lastPos = pos;
                player.isAttacking = true;
            }
            else{
                MovePlayer(pos);
                if(touch.phase == TouchPhase.Ended ||
                    touch.phase == TouchPhase.Canceled){
                    player.isAttacking = false;
                }
            }
        }
    }

    Vector2 lastPos;
    public void MovePlayer(Vector2  pos){
        Vector3 deltaPos = pos - lastPos;

        Vector2 position = transform.position + (deltaPos * (invertControls ? -1 : 1));

        Vector2 clampedNewPos = new Vector2 (Mathf.Clamp(position.x, 
                                            playerBounds.leftBound, playerBounds.rightBound),
                                            Mathf.Clamp(position.y, 
                                            0, 0));

        transform.position = clampedNewPos;

        lastPos = pos;
    }


    void Restart(){
        // InitializePlayer();
    }


    private void ScalePlayerForScreen(){
        float playerWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        float recommendedPlayerWidth = Boundary.visibleWorldWidth/10.0f;

        float resultingScale = recommendedPlayerWidth/playerWidth;

        transform.localScale = Vector3.one * resultingScale;

        GameManager.Scale = resultingScale;
    }

    public void InitializePlayer(bool justStarted){
        transform.position = Vector2.zero;

        if(justStarted) //**hack!!! to fix minor bug */
        {
            player.isAttacking = true;
        }

        invertControls = PlayerPrefs.GetInt(GameManager.InvertControlsKey) == 1;
    }
}
