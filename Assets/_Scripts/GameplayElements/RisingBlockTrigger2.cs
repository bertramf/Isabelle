using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingBlockTrigger2 : MonoBehaviour {

    public BoxCollider2D coll;

    private void Start() {
        coll = transform.parent.transform.parent.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            coll.enabled = false;
        }
    }

}
