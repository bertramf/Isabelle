using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParent : MonoBehaviour {

    //Kan een array worden als een parentable object een andere tag moet hebben
    public string parentObjectTag;
    

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == parentObjectTag) {
             transform.SetParent(other.gameObject.transform, true);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == parentObjectTag) {
            transform.SetParent(null);
        }
    }
}
