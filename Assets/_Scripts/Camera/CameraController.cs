using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    private Transform PlayerTransform;
    private PlayerBase PlayerBase;

    private float xMovement;
    private float yMovement;

    [Header("Movement Values")]
    public float xOffset = 0.2f;
    public float xSmoothing = 7f;
    public float ySmoothing = 7f;

    private void Start () {
        PlayerTransform = GameObject.Find("Player").transform;
        PlayerBase = PlayerTransform.GetComponent<PlayerBase>();  
    }
	
	private void FixedUpdate () {
        PositionCalculations();
        Movement();
    }

    private void PositionCalculations() {
        float offsetWalk = PlayerBase.lookDirection * xOffset;

        float xA = transform.position.x;
        float yA = transform.position.y;
        float xB = PlayerTransform.position.x + offsetWalk;
        float yB = PlayerTransform.position.y;

        xMovement = Mathf.Lerp(xA, xB, xSmoothing * Time.deltaTime);
        yMovement = Mathf.Lerp(yA, yB, ySmoothing * Time.deltaTime);
    }

    private void Movement() {
        transform.position = new Vector3(xMovement, yMovement, transform.position.z);
    }
}
