using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    private Transform playerTransform;
    private PlayerBase playerBase;

    private float xMovement;
    private float yMovement;

    [Header("Movement Values")]
    public float xOffset = 0.2f;
    public float xSmoothing = 7f;
    public float ySmoothing = 7f;

    private void Start () {
        playerTransform = GameObject.Find("Player").transform;
        playerBase = playerTransform.GetComponent<PlayerBase>();  
    }
	
	private void FixedUpdate () {
        PositionCalculations();
        Movement();
    }

    private void PositionCalculations() {
        float offsetWalk = playerBase.lookDirection * xOffset;

        float xA = transform.position.x;
        float yA = transform.position.y;
        float xB = playerTransform.position.x + offsetWalk;
        float yB = playerTransform.position.y;

        xMovement = Mathf.Lerp(xA, xB, xSmoothing * Time.deltaTime);
        yMovement = Mathf.Lerp(yA, yB, ySmoothing * Time.deltaTime);
    }

    private void Movement() {
        transform.position = new Vector3(xMovement, yMovement, transform.position.z);
    }
}
