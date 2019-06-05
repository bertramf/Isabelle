using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroup : MonoBehaviour{

    private Component[] birds;

    private void Start(){
        birds = GetComponentsInChildren<Bird>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            float xPosPlayer = other.gameObject.transform.position.x;
            foreach(Bird bird in birds) {
                bird.FlyFromGroupTrigger(xPosPlayer);
            }
        }
    }

}
