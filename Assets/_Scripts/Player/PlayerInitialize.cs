using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialize : MonoBehaviour {

    private void Awake() {
        SetStartPosition();
    }

    public void SetStartPosition() {
        Vector3 startPosition = GameManager.Instance.CurrentCheckpoint;
        transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
    }

}
