using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerBase playerBase;

    public Color dashColor;

    private void Start () {
        anim = transform.Find("PlayerVisuals").GetComponent<Animator>();
        spriteRenderer = transform.Find("PlayerVisuals").GetComponent<SpriteRenderer>();
        playerBase = GetComponent<PlayerBase>();
    }

    private void OnEnable() {
        PlayerBase.onEventChangeDirection += Event_FlipLocalX;
        PlayerBase.onEventDash += Event_ChangeColor;
        PlayerBase.onEventDashRecharged += Event_RedoColors;
    }

    private void OnDisable() {
        PlayerBase.onEventChangeDirection -= Event_FlipLocalX;
        PlayerBase.onEventDash -= Event_ChangeColor;
        PlayerBase.onEventDashRecharged -= Event_RedoColors;
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
        spriteRenderer.color = dashColor;
    }

    private void Event_RedoColors() {
        spriteRenderer.color = Color.white;
    }

}
