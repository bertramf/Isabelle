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
        if (playerBase.isAlive) {
            if (other.gameObject.tag == "Death_Falling") {
                Death_Falling();
            }
            if (other.gameObject.tag == "Death_Box") {
                Death_Box();
            }
        }
    }

    private void Death_Falling() {
        playerBase.KillPlayer();
        //Event Setter
        if (onEventHittedFalling != null) {
            onEventHittedFalling();
        }
        GameManager.Instance.CheckForSceneChange(false, 0f);
    }

    private void Death_Box() {
        playerBase.KillPlayer();
        playerBase.FreezePlayer(10f);
        //Event Setter
        if (onEventHittedBox != null) {
            onEventHittedBox();
        }
        GameManager.Instance.CheckForSceneChange(false, 0.75f);
    }

}
