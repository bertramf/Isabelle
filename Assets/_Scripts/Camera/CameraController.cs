using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    private Transform PlayerTransform;
    private PlayerBase PlayerBase;
    private PlayerRaycasts PlayerRaycasts;
    private Camera camera;
    private float xMovement;
    private float yMovement;
    private float yCamHalfSize;

    [Header("Movement Values")]
    public float xOffset = 0.2f;
    public float xSmoothing = 7f;
    public float ySmoothing = 7f;

    private void Start () {
        PlayerTransform = GameObject.Find("Player").transform;
        PlayerBase = PlayerTransform.GetComponent<PlayerBase>();
        PlayerRaycasts = PlayerTransform.GetComponent<PlayerRaycasts>();
        camera = transform.Find("Main Camera").GetComponent<Camera>();

        yCamHalfSize = camera.orthographicSize;
    }
	
	private void FixedUpdate () {
        PositionCalculations();
        Movement();
    }

    private void PositionCalculations() {
        float offsetWalk = PlayerBase.lookDirection * xOffset;
        float yCamBorder = yCamHalfSize + PlayerRaycasts.yBorder;

        float xA = transform.position.x;
        float xB = PlayerTransform.position.x + offsetWalk;
        xMovement = Mathf.Lerp(xA, xB, xSmoothing * Time.deltaTime);

        float yA = transform.position.y;
        float yB = PlayerTransform.position.y;

        if (PlayerRaycasts.hitsCamBorder) {
            if(yB > yCamBorder) {
                yMovement = Mathf.Lerp(yA, yB, ySmoothing * Time.deltaTime);
            }
            else {
                yMovement = Mathf.Lerp(yA, yCamBorder, ySmoothing * Time.deltaTime);
            }
        }
        else {
            yMovement = Mathf.Lerp(yA, yB, ySmoothing * Time.deltaTime);
        }
    }

    private void Movement() {
        transform.position = new Vector3(xMovement, yMovement, transform.position.z);
    }
}
