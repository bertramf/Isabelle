using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Transform playerVfx;
    private PlayerBase playerBase;

    public ParticleSystem[] ps_land;
    private Vector3[] ps_startPos_land = new Vector3[4];
    private float[] ps_xPosition_land = new float[4];

    public ParticleSystem[] ps_jump;
    private Vector3[] ps_startPos_jump = new Vector3[2];
    private float[] ps_xPosition_jump = new float[2];

    private int i;

    private void Start () {
        anim = transform.Find("PlayerVisuals").GetComponent<Animator>();
        spriteRenderer = transform.Find("PlayerVisuals").GetComponent<SpriteRenderer>();
        playerVfx = transform.Find("PlayerVfx");
        playerBase = GetComponent<PlayerBase>();

        for(int i = 0; i < ps_land.Length; i++) {
            ps_startPos_land[i] = ps_land[i].transform.localPosition;
        }
        for (int i = 0; i < ps_jump.Length; i++) {
            ps_startPos_jump[i] = ps_jump[i].transform.localPosition;
        }
    }

    private void OnEnable() {
        PlayerBase.onEventChangeDirection += Event_FlipLocalX;
        PlayerBase.onEventDash += Event_ChangeColor;
        PlayerBase.onEventDashRecharged += Event_RedoColors;
        PlayerBase.onEventDash += Event_DashParticles;
        PlayerBase.onEventJump += Event_JumpParticles;
        PlayerRaycasts.onEventGrounded += Event_LandParticles;
    }

    private void OnDisable() {
        PlayerBase.onEventChangeDirection -= Event_FlipLocalX;
        PlayerBase.onEventDash -= Event_ChangeColor;
        PlayerBase.onEventDashRecharged -= Event_RedoColors;
        PlayerBase.onEventDash -= Event_DashParticles;
        PlayerBase.onEventJump -= Event_JumpParticles;
        PlayerRaycasts.onEventGrounded -= Event_LandParticles;
    }

    private void LateUpdate() {
        AssignAnimations();
    }

    private void AssignAnimations() {
        bool isWalking;
        if (playerBase.movementSpeed > 0) {
            isWalking = true;
        }
        else {
            isWalking = false;
        }

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isDashing", playerBase.isDashing);
        anim.SetFloat("yVelocity", playerBase.upVelocity);
    }

    private void Event_FlipLocalX(int direction) {
        Vector3 playerScale = new Vector3(direction, 1, 1);
        transform.localScale = playerScale;
    }

    private void Event_ChangeColor() {
        spriteRenderer.color = new Color(0.25f, 0.25f, 0.25f);
    }

    private void Event_RedoColors() {
        spriteRenderer.color = Color.white;
    }

    private void Event_DashParticles() {

    }

    private void Event_JumpParticles() {
        i++;
        print(i);
        Vector3 vfxScale = new Vector3(playerBase.lookDirection, 1, 1);
        float xOffsetWalking = 0.12f;

        for (int i = 0; i < ps_jump.Length; i++) {
            if (playerBase.lookDirection == 1) {
                if (playerBase.movementSpeed > 0) {
                    ps_xPosition_jump[i] = ps_startPos_jump[i].x + xOffsetWalking;
                }
                else {
                    ps_xPosition_jump[i] = ps_startPos_jump[i].x;
                }
            }
            else {
                if (playerBase.movementSpeed > 0) {
                    ps_xPosition_jump[i] = ps_startPos_jump[i].x * -1 + xOffsetWalking;
                }
                else {
                    ps_xPosition_jump[i] = ps_startPos_jump[i].x * -1;
                }
            }
        }

        for (int i = 0; i < ps_jump.Length; i++) {
            ps_jump[i].transform.localScale = vfxScale;
            ps_jump[i].transform.localPosition = new Vector3(ps_xPosition_jump[i], ps_startPos_jump[i].y, ps_startPos_jump[i].z);
            ps_jump[i].transform.parent = null;
            ps_jump[i].Play();
        }

        StartCoroutine(Parent_JumpParticles());
    }

    private IEnumerator Parent_JumpParticles() {
        yield return new WaitForSeconds(0.45f);

        for (int i = 0; i < ps_jump.Length; i++) {
            ps_jump[i].transform.SetParent(playerVfx);
            ps_jump[i].transform.localPosition = ps_startPos_jump[i];
        }

        //Switch inhoud particles
    }

    private void Event_LandParticles() {
        Vector3 vfxScale = new Vector3(playerBase.lookDirection, 1, 1);
        float xOffsetWalking = 0.12f;

        for (int i = 0; i < ps_land.Length; i++) {
            if (playerBase.lookDirection == 1) {
                if (playerBase.movementSpeed > 0) {
                    ps_xPosition_land[i] = ps_startPos_land[i].x + xOffsetWalking;
                }
                else {
                    ps_xPosition_land[i] = ps_startPos_land[i].x;
                }
            }
            else {
                if (playerBase.movementSpeed > 0) {
                    ps_xPosition_land[i] = ps_startPos_land[i].x * -1 + xOffsetWalking;
                }
                else {
                    ps_xPosition_land[i] = ps_startPos_land[i].x * -1;
                }
            }  
        }

        for (int i = 0; i < ps_land.Length; i++) {
            //Set xDirection
            ps_land[i].transform.localScale = vfxScale;
            //Set position
            ps_land[i].transform.localPosition = new Vector3(ps_xPosition_land[i], ps_startPos_land[i].y, ps_startPos_land[i].z);
            //Unparent
            ps_land[i].transform.parent = null;
            //Play Particle
            ps_land[i].Play();
        }

        StartCoroutine(Parent_LandParticles());
    }

    private IEnumerator Parent_LandParticles() {
        yield return new WaitForSeconds(0.35f);
        
        for (int i = 0; i < ps_land.Length; i++) {
            //Unparent
            ps_land[i].transform.SetParent(playerVfx);
            //Set to localStartPos
            ps_land[i].transform.localPosition = ps_startPos_land[i];
        }

        //Switch inhoud particles
    }



}
