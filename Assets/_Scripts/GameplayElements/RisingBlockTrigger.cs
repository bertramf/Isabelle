using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingBlockTrigger : MonoBehaviour {
    
    private RisingBlock risingBlockScr2;

    private void Start() {
        risingBlockScr2 = transform.parent.transform.parent.GetComponent<RisingBlock>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            risingBlockScr2.hitsTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            risingBlockScr2.hitsTrigger = false;
        }
    }

}
