using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerBase playerBase;
    private ColorSwap colorSwap;

    public ColorSwapPreset defaultPreset;
    public ColorSwapPreset dashPreset;

    public Color dashColor;

    private void Start () {
        anim = transform.Find("PlayerVisuals").GetComponent<Animator>();
        spriteRenderer = transform.Find("PlayerVisuals").GetComponent<SpriteRenderer>();
        playerBase = GetComponent<PlayerBase>();
        colorSwap = GetComponentInChildren<ColorSwap>();
    }

    private void OnEnable() {
        PlayerHitted.onEventHittedBox += Event_PlayerHittedBox;
        PlayerBase.onEventChangeDirection += Event_FlipLocalX;
        PlayerBase.onEventDash += Event_ChangeColor;
        PlayerBase.onEventDashRecharged += Event_RedoColors;
    }

    private void OnDisable() {
        PlayerHitted.onEventHittedBox -= Event_PlayerHittedBox;
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

    private void Event_PlayerHittedBox() {
        anim.SetTrigger("isHitted");
    }

    private void Event_FlipLocalX(int direction) {
        Vector3 playerScale = new Vector3(direction, 1, 1);
        transform.localScale = playerScale;
    }

    private void Event_ChangeColor() {
        colorSwap.UpdateVisualData(dashPreset);
        //spriteRenderer.color = dashColor;
    }

    private void Event_RedoColors() {
        colorSwap.UpdateVisualData(defaultPreset);
        //spriteRenderer.color = Color.white;
    }

    private IEnumerator RedoNormalColors() {
        yield return new WaitForSeconds(0.2f);
    }

}
