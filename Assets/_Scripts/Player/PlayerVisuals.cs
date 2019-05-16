using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private PlayerBase PlayerBase;
    private ColorSwap ColorSwap;

    public ColorSwapPreset weakPreset;
    public ColorSwapPreset poweredPreset;
    public ColorSwapPreset deadPreset;

    private void Start () {
        anim = GetComponent<Animator>();
        PlayerBase = GetComponentInParent<PlayerBase>();
        ColorSwap = GetComponent<ColorSwap>();
    }

    private void OnEnable() {
        PlayerHitted.onEventHittedFalling += Event_PlayerHittedFalling;
        PlayerHitted.onEventHittedBox += Event_PlayerHittedBox;
        PlayerBase.onEventChangeDirection += Event_FlipLocalX;
        PlayerBase.onEventDash += Event_ChangeColor;
        PlayerBase.onEventDashRecharged += Event_RedoColors;
    }

    private void OnDisable() {
        PlayerHitted.onEventHittedFalling -= Event_PlayerHittedFalling;
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
        if (PlayerBase.movementSpeed > 0) {
            isWalking = true;
        }
        else {
            isWalking = false;
        }

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isDashing", PlayerBase.isDashing);
        anim.SetFloat("yVelocity", PlayerBase.upVelocity);

    }

    private void Event_PlayerHittedFalling() {
        ColorSwap.UpdateVisualData(deadPreset);
    }

    private void Event_PlayerHittedBox() {
        ColorSwap.UpdateVisualData(deadPreset);
        Screenshake.instance.StartShakeVertical(2, 0.5f, 0.06f);
        anim.SetTrigger("isHitted");
    }

    private void Event_FlipLocalX(int direction) {
        Vector3 PlayerScale = new Vector3(direction, 1, 1);
        transform.localScale = PlayerScale;
    }

    private void Event_ChangeColor() {
        ColorSwap.UpdateVisualData(weakPreset);
    }

    private void Event_RedoColors() {
        ColorSwap.UpdateVisualData(poweredPreset);
    }

}
