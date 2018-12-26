using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUp_Trigger : MonoBehaviour {

    private BlockUp_Base blockUpBase;

    private void Start() {
        blockUpBase = transform.parent.GetComponent<BlockUp_Base>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            blockUpBase.hitsTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            blockUpBase.hitsTrigger = false;
        }
    }
}
