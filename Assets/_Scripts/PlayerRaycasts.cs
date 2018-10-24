using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycasts : MonoBehaviour {

    private PlayerBase playerBase;
    private PlayerValues playerValues;

    private Vector2 topLeft_grounded;
    private Vector2 bottomRight_grounded;
    private Vector2 topLeft_befWall;
    private Vector2 bottomRight_befWall;
    private float releasePlatformTime;

    public bool grounded;
    public bool coyoteGrounded;
    public bool beforeWall;

    [Header("Values grounded")]
    public LayerMask groundLayer;
    public float horLength_grounded = 0.25f;
    public float verLength_grounded = 0.1f;
    public float verOffset_grounded = -0.5f;

    [Header("Values beforeWall")]
    public float horLength_befWall = 0.25f;
    public float horOffset_befWall = -0.1f;
    public float verLength_befWall = 0.1f;
    public float verOffset_befWall = -0.15f;

    private void Start() {
        playerBase = GetComponent<PlayerBase>();
        playerValues = GetComponent<PlayerValues>();

        releasePlatformTime = playerValues.coyoteTime;
    }

    public void Raycasting() {
        //Grounded
        topLeft_grounded = new Vector2(transform.position.x - (horLength_grounded / 2), transform.position.y + (verLength_grounded / 2) + verOffset_grounded);
        bottomRight_grounded = new Vector2(transform.position.x + (horLength_grounded / 2), transform.position.y - (verLength_grounded / 2) + verOffset_grounded);
        grounded = Physics2D.OverlapArea(topLeft_grounded, bottomRight_grounded, groundLayer);

        //BeforeWall
        topLeft_befWall = new Vector2(transform.position.x - (horOffset_befWall * playerBase.lookDirection), transform.position.y + (verLength_befWall / 2) + verOffset_befWall);
        bottomRight_befWall = new Vector2( transform.position.x + ((horLength_befWall + horOffset_befWall) * playerBase.lookDirection), transform.position.y - (verLength_befWall / 2) + verOffset_befWall);
        beforeWall = Physics2D.OverlapArea(topLeft_befWall, bottomRight_befWall, groundLayer);
    }

    public void CoyoteTime() {
        if (grounded) {
            coyoteGrounded = true;
            releasePlatformTime = playerValues.coyoteTime;
        }
        else if (!grounded && playerBase.upVelocity < 0.1f && releasePlatformTime > 0) {
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

    private void GroundedGizmos() {
        //Green
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Vector3 gizCenter_grounded = new Vector3(transform.position.x, transform.position.y + verOffset_grounded, 0);
        Vector3 gizSize_grounded = new Vector3(horLength_grounded, verLength_grounded, 0);
        Gizmos.DrawCube(gizCenter_grounded, gizSize_grounded);
    }

    private void BeforeWallGizmos() {
        float playerDirection = 1f;
        if (Application.isPlaying) {
            playerDirection = playerBase.lookDirection;
        }

        //Red
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 gizCenter_befWall = new Vector3(transform.position.x + (((horLength_befWall / 2) + horOffset_befWall) * playerDirection), transform.position.y + verOffset_befWall, 0);
        Vector3 gizSize_befWall = new Vector3(horLength_befWall, verLength_befWall, 0);
        Gizmos.DrawCube(gizCenter_befWall, gizSize_befWall);
    }

}
