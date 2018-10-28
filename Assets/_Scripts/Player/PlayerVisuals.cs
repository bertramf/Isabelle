using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerBase playerBase;

    public ParticleSystem ps_landLeft;
    public ParticleSystem ps_landRight;
    
    private void Start () {
        anim = transform.Find("PlayerVisuals").GetComponent<Animator>();
        spriteRenderer = transform.Find("PlayerVisuals").GetComponent<SpriteRenderer>();
        playerBase = GetComponent<PlayerBase>();
    }

    private void Update() {
        DashState();
        AssignAnimations();
    }

    private void DashState() {
        if (!playerBase.canDash) {
            spriteRenderer.color = new Color(0.25f, 0.25f, 0.25f);
        }
        else {
            spriteRenderer.color = Color.white;
        }
    }

    public void LandParticles() {
        Vector3 vfxScale = new Vector3(playerBase.lookDirection, 1, 1);
        ps_landLeft.transform.localScale = vfxScale;
        ps_landRight.transform.localScale = vfxScale;
        ps_landLeft.Play();
        ps_landRight.Play();
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
        anim.SetFloat("yVelocity", playerBase.upVelocity);
    }
	
}
