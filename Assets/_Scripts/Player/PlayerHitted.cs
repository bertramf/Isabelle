using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitted : MonoBehaviour {

    public delegate void EventHittedBox();
    public static event EventHittedBox onEventHittedBox;


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Death_Falling") {
            Death_Falling();
        }
        if (other.gameObject.tag == "Death_Box") {
            Death_Box();
        }
    }

    private void Death_Falling() {
        GameManager.Instance.TriggerDeath(0f);
    }

    private void Death_Box() {
        onEventHittedBox();
        PlayerFreeze.instance.FreezePlayer(1f);
        GameManager.Instance.TriggerDeath(0.5f);
    }

}
