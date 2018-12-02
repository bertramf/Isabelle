using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitted : MonoBehaviour {

    public CapsuleCollider2D coll;
    public Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Death_Falling") {
            Death_Falling();
        }
        if (other.gameObject.tag == "Death_Box") {
            Death_Box();
        }
    }

    private void Death_Falling() {
        GameManager.Instance.TriggerDeath();
    }

    private void Death_Box() {
        GameManager.Instance.TriggerDeath();
    }

}
