using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour {

    private Vector3 spawnPosition;

    private void Start() {
        spawnPosition = transform.parent.position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            GameManager.Instance.CurrentCheckpoint = spawnPosition;
        }
    }

}
