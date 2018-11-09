using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingBlockTrigger : MonoBehaviour {

    private RisingBlock risingBlockScr;

    private void Start() {
        risingBlockScr = transform.parent.transform.parent.GetComponent<RisingBlock>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            risingBlockScr.CalculateUp();
        }
    }

}
