using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private PlayerBase playerBase;
    
    private void Start () {
        anim = transform.Find("PlayerVisuals").GetComponent<Animator>();
        playerBase = GetComponent<PlayerBase>();
    }

    private void Update() {
        AssignAnimations();
    }

    private void AssignAnimations() {
        bool isWalking;
        if(playerBase.movementSpeed > 0) {
            isWalking = true;
        }
        else {
            isWalking = false;
        }

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isDashing", playerBase.isDashing);
        anim.SetInteger("jumpState", playerBase.jumpState);
    }
	
}
