using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour{

    private bool isInTrigger;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }
        isInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }
        isInTrigger = false;
    }

    private void Update() {
        if (isInTrigger) {
            if (Input.GetButtonDown("Y")) {
                DestroyFeather();
            }
        }
    }

    private void DestroyFeather() {
        Destroy(this.gameObject);
    }

}
