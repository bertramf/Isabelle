using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPlatform : MonoBehaviour {

    public enum PlatformState {
        left,
        right
    }
    private SpriteRenderer rendMain;
    private SpriteRenderer rendStipplesRight;
    private SpriteRenderer rendStipplesLeft;

    public Sprite mainLeft;
    public Sprite mainRight;

    [Header("Debug Values")]
    public PlatformState plaftormState;
    public Vector3 startPos;
    public int currentPlayerDirection;

	private void Start () {
        rendMain = transform.Find("DashPlatform_Visuals").GetComponent<SpriteRenderer>();
        rendStipplesRight = transform.Find("DashPlatform_Visuals").transform.Find("Stipples_toRight").GetComponent<SpriteRenderer>();
        rendStipplesLeft = transform.Find("DashPlatform_Visuals").transform.Find("Stipples_toLeft").GetComponent<SpriteRenderer>();

        plaftormState = PlatformState.left;
        startPos = transform.position;
        currentPlayerDirection = 1;
    }

    private void OnEnable() {
        PlayerBase.onEventChangeDirection += Event_ChangeDirection;
        PlayerBase.onEventDash += Event_MovePlatform;
    }

    private void OnDisable() {
        PlayerBase.onEventChangeDirection -= Event_ChangeDirection;
        PlayerBase.onEventDash -= Event_MovePlatform;
    }

    private void Event_ChangeDirection(int direction) {
        currentPlayerDirection = direction;
    }

    private void Event_MovePlatform() {
        if(plaftormState == PlatformState.left) {
            if(currentPlayerDirection == 1) {
                MoveToRight();
            }
        }
        else if (plaftormState == PlatformState.right) {
            if (currentPlayerDirection == -1) {
                MoveToLeft();
            }
        }
    }

    private void MoveToRight() {
        float xOffset = 1f;
        Vector3 newPos = new Vector3(startPos.x + xOffset, startPos.y, startPos.z);
        transform.position = newPos;
        rendMain.sprite = mainRight;
        rendStipplesRight.enabled = false;
        rendStipplesLeft.enabled = true;
        plaftormState = PlatformState.right;
    }

    private void MoveToLeft() {
        float xOffset = 0f;
        Vector3 newPos = new Vector3(startPos.x + xOffset, startPos.y, startPos.z);
        transform.position = newPos;
        rendMain.sprite = mainLeft;
        rendStipplesRight.enabled = true;
        rendStipplesLeft.enabled = false;
        plaftormState = PlatformState.left;
    }
}
