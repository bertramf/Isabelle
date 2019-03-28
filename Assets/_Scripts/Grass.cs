using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour{

    private Animator anim_c;

    public AnimationState idleAnim;
    public AnimationClip clip1;
    public AnimationClip clip2;

    private void Start() {
        anim_c = GetComponent<Animator>();

        //Set random xScale
        float xScale = 1;
        int randomXScale = Random.Range(0, 2);
        if (randomXScale == 1) {
            xScale = 1;
        }
        else {
            xScale = -1;
        }
        transform.localScale = new Vector3(xScale, 1, 0);

        //Set random idleTime
        float randomStartTime = Random.Range(0f, 0.5f);

        ////set random idleanimation
        //int randomidle = random.randomrange(0, 2);
        //if (randomidle == 1) {
        //    idleanim.addclip(clip1, "beuuh");
        //}
        //else {
        //    idleanim.addclip(clip1, "mwaah");
        //}

        Invoke("StartIdleAnimation", randomStartTime);
    }

    private void StartIdleAnimation() {
        anim_c.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            anim_c.SetTrigger("swoosh");
        }
    }
}
