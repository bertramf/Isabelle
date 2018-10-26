using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitted : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Death_Falling") {
            Death_Falling();
        }
    }

    private void Death_Falling() {
        StartCoroutine(GameManager.Instance.Death());
    }

}
