using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour{

    public enum FlowerState {
        closed,
        open
    }
    public FlowerState flowerState;

    private Animator anim_c;

    public ParticleSystem ps_open;
    public ParticleSystem ps_closing;
    public bool openingFlower;
    public bool randomXflip;

    private void Start() {
        anim_c = GetComponent<Animator>();
        SetRandomValues();
        if(flowerState == FlowerState.closed) {
            anim_c.SetTrigger("startClosed");
        }
    }

    private void SetRandomValues() {
        //If randomXflip == true, set random xScale : 50% chance for x=1 or x=-1
        if (randomXflip) {
            float xScale = 1;
            int randomXScale = Random.Range(0, 2);
            if (randomXScale == 1) {
                xScale = 1;
            }
            else {
                xScale = -1;
            }
            transform.localScale = new Vector3(xScale, 1, 0);
        }

        //Set random height
        int yOffset = Random.Range(0, 3);
        float yOff = yOffset * -0.03f;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + yOff, transform.localPosition.z);

        //Set random animatorSpeed
        float randomAnimatorSpeed = Random.Range(0.85f, 1.15f);
        anim_c.speed = randomAnimatorSpeed;

        //Set random idleTime
        float randomOffset = Random.Range(0f, 0.5f);
        Invoke("EnableAnimator", randomOffset);
    }

    private void EnableAnimator() {
        anim_c.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if (flowerState == FlowerState.closed && openingFlower) {
                flowerState = FlowerState.open;
                anim_c.SetTrigger("interact");
                ps_open.Play();
            }
            else if (flowerState == FlowerState.open && !openingFlower) {
                flowerState = FlowerState.closed;
                anim_c.SetTrigger("interact");
                ps_closing.Play();
            }
        }
    }

}
