using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycasts : MonoBehaviour {

    public delegate void EventGrounded();
    public static event EventGrounded onEventGrounded;
    
    private PlayerBase PlayerBase;
    private PlayerValues PlayerValues;
    private Vector2 topLeft_grounded;
    private Vector2 bottomRight_grounded;
    private Vector2 topLeft_befWall;
    private Vector2 bottomRight_befWall;
    private float releasePlatformTime;

    [Header("Values")]
    public bool grounded;
    public bool coyoteGrounded;
    public bool beforeWall;
    public bool hitsCamBorder; //Moet accesable zijn
    public float yBorder; //Moet accesable zijn

    [Header("Values grounded raycasts")]
    public LayerMask groundLayer;
    public float horLength_grounded = 0.14f;
    public float verLength_grounded = 0.1f;
    public float verOffset_grounded = -0.5f;

    [Header("Values beforeWall")]
    public float horLength_befWall = 0.2f;
    public float horOffset_befWall = -0.05f;
    public float verLength_befWall = 0.45f;
    public float verOffset_befWall = -0.25f;

    [Header("Values beforeWall")]
    public LayerMask camBorderLayer;
    public float verLength_camBorder = 4f;
    public float verOffset_camBorder = -0.5f;

    private void Start() {
        PlayerBase = GetComponent<PlayerBase>();
        PlayerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");

        grounded = true;
        releasePlatformTime = PlayerValues.coyoteTime; 
    }

    public void Raycasting() {
        //Grounded
        topLeft_grounded = new Vector2(transform.position.x - (horLength_grounded / 2), transform.position.y + (verLength_grounded / 2) + verOffset_grounded);
        bottomRight_grounded = new Vector2(transform.position.x + (horLength_grounded / 2), transform.position.y - (verLength_grounded / 2) + verOffset_grounded);
        bool oldGrounded = grounded;
        grounded = Physics2D.OverlapArea(topLeft_grounded, bottomRight_grounded, groundLayer);
        if (oldGrounded != grounded && grounded) {
            if(onEventGrounded != null) {
                onEventGrounded();
            }
        }

        //BeforeWall
        topLeft_befWall = new Vector2(transform.position.x - (horOffset_befWall * PlayerBase.lookDirection), transform.position.y + (verLength_befWall / 2) + verOffset_befWall);
        bottomRight_befWall = new Vector2( transform.position.x + ((horLength_befWall + horOffset_befWall) * PlayerBase.lookDirection), transform.position.y - (verLength_befWall / 2) + verOffset_befWall);
        beforeWall = Physics2D.OverlapArea(topLeft_befWall, bottomRight_befWall, groundLayer);

        //Camerabox
        RaycastHit2D hitCamInfo = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + verOffset_camBorder), Vector2.down, verLength_camBorder, camBorderLayer);
        if (hitCamInfo) {
            hitsCamBorder = true;
            Collider2D camBorderCollider = hitCamInfo.collider;
            yBorder = camBorderCollider.bounds.max.y;
        }
        else {
            hitsCamBorder = false;
            yBorder = 0f;
        }
    }

    public void CoyoteTime() {
        if (grounded) {
            coyoteGrounded = true;
            releasePlatformTime = PlayerValues.coyoteTime;
        }
        else if (!grounded && PlayerBase.upVelocity < 0.1f && releasePlatformTime > 0) {
            releasePlatformTime -= Time.deltaTime;
            coyoteGrounded = true;
        }
        else {
            releasePlatformTime = -1f;
            coyoteGrounded = false;
        }
    }

    private void OnDrawGizmosSelected() {
        GroundedGizmos();
        BeforeWallGizmos();
    }

    private void OnDrawGizmos() {
        CamBorderGizmos();
    }

    private void GroundedGizmos() {
        //Green
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Vector3 gizCenter_grounded = new Vector3(transform.position.x, transform.position.y + verOffset_grounded, 0);
        Vector3 gizSize_grounded = new Vector3(horLength_grounded, verLength_grounded, 0);
        Gizmos.DrawCube(gizCenter_grounded, gizSize_grounded);
    }

    private void BeforeWallGizmos() {
        float PlayerDirection = 1f;
        if (Application.isPlaying) {
            PlayerDirection = PlayerBase.lookDirection;
        }

        //Red
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 gizCenter_befWall = new Vector3(transform.position.x + (((horLength_befWall / 2) + horOffset_befWall) * PlayerDirection), transform.position.y + verOffset_befWall, 0);
        Vector3 gizSize_befWall = new Vector3(horLength_befWall, verLength_befWall, 0);
        Gizmos.DrawCube(gizCenter_befWall, gizSize_befWall);
    }

    private void CamBorderGizmos() {
        //Purple
        Gizmos.color = new Color(1, 0, 1, 1f);
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + verOffset_camBorder, transform.position.z), new Vector3(transform.position.x, transform.position.y + verOffset_camBorder - verLength_camBorder, transform.position.z));
    }

}
