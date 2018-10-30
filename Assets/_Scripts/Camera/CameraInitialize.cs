using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInitialize : MonoBehaviour {

    private CameraController cameraController;

	private void Awake () {
        cameraController = GetComponent<CameraController>();
        SetStartPosition();
    }

    public void SetStartPosition() {
        Vector3 startPosition = GameManager.Instance.CurrentCheckpoint;
        transform.position = new Vector3(startPosition.x + cameraController.xOffset, startPosition.y, transform.position.z);
    }

}
