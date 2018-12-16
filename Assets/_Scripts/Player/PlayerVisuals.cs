using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private PlayerBase PlayerBase;
    private ColorSwap colorSwap;

    public ColorSwapPreset defaultPreset;
    public ColorSwapPreset dashPreset;
    public ColorSwapPreset deadPreset;

    private void Start () {
        anim = GetComponent<Animator>();
        PlayerBase = GetComponentInParent<PlayerBase>();
        colorSwap = GetComponent<ColorSwap>();
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

        if (PlayerBase.isAlive) {
            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isDashing", PlayerBase.isDashing);
        }
        else {
            anim.SetBool("isWalking", false);
            anim.SetBool("isDashing", false);
        }
        anim.SetFloat("yVelocity", PlayerBase.upVelocity);

    }

    private void Event_PlayerHittedFalling() {
        colorSwap.UpdateVisualData(deadPreset);
    }

    private void Event_PlayerHittedBox() {
        colorSwap.UpdateVisualData(deadPreset);
        StartCoroutine(HittedShakeLogic());
        anim.SetTrigger("isHitted");
    }

    private IEnumerator HittedShakeLogic() {
        Screenshake.instance.StartShakeVertical(1, 0.05f, 0.15f);
        yield return new WaitForSeconds(0.25f);
        Screenshake.instance.StartShakeVertical(2, 0.75f, 0.1f);
    }

    private void Event_FlipLocalX(int direction) {
        Vector3 PlayerScale = new Vector3(direction, 1, 1);
        transform.localScale = PlayerScale;
    }

    private void Event_ChangeColor() {
        colorSwap.UpdateVisualData(dashPreset);
    }

    private void Event_RedoColors() {
        colorSwap.UpdateVisualData(defaultPreset);
    }

}
