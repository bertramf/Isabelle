using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitted : MonoBehaviour {

    public delegate void EventHittedBox();
    public delegate void EventHittedFalling();
    public static event EventHittedBox onEventHittedBox;
    public static event EventHittedFalling onEventHittedFalling;

    private PlayerBase playerBase;

    private void Start() {
        playerBase = GetComponent<PlayerBase>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Death_Falling") {
            Death_Falling();
        }
        if (other.gameObject.tag == "Death_Box") {
            Death_Box();
        }
    }

    private void Death_Falling() {
        playerBase.isAlive = false;
        playerBase.StopDash();
        //Event Setter
        if (onEventHittedFalling != null) {
            onEventHittedFalling();
        }
        GameManager.Instance.TriggerDeath(0f);
    }

    private void Death_Box() {
        playerBase.isAlive = false;
        playerBase.FreezePlayer(10f);
        playerBase.StopDash();
        //Event Setter
        if (onEventHittedBox != null) {
            onEventHittedBox();
        }
        GameManager.Instance.TriggerDeath(0.5f);
    }

}
