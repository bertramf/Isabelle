using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPlatform2 : MonoBehaviour {

    public enum PlatformState {
        left,
        middle,
        right
    }

    private SpriteRenderer rendStipplesRight;
    private SpriteRenderer rendStipplesLeft;

    [Header("Debug Values")]
    public PlatformState plaftormState;
    public int currentPlayerDirection;
    
    private void Start () {
        rendStipplesRight = transform.Find("DashPlatform_Visuals").transform.Find("Stipples_toRight").GetComponent<SpriteRenderer>();
        rendStipplesLeft = transform.Find("DashPlatform_Visuals").transform.Find("Stipples_toLeft").GetComponent<SpriteRenderer>();

        plaftormState = PlatformState.middle;
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
        if (plaftormState == PlatformState.middle) {
            if (currentPlayerDirection == 1) {
                MoveToRight();
                plaftormState = PlatformState.right;
                rendStipplesRight.enabled = false;
                rendStipplesLeft.enabled = true;
            }
            else if(currentPlayerDirection == -1) {
                MoveToLeft();
                plaftormState = PlatformState.left;
                rendStipplesRight.enabled = true;
                rendStipplesLeft.enabled = false;
            }
        }
        else if(plaftormState == PlatformState.left) {
            if (currentPlayerDirection == 1) {
                MoveToRight();
                plaftormState = PlatformState.middle;
                rendStipplesRight.enabled = true;
                rendStipplesLeft.enabled = true;
            }
        }
        else if (plaftormState == PlatformState.right) {
            if (currentPlayerDirection == -1) {
                MoveToLeft();
                plaftormState = PlatformState.middle;
                rendStipplesRight.enabled = true;
                rendStipplesLeft.enabled = true;
            }
        }
    }

    private void MoveToRight() {
        float xOffset = 1f;
        transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z);
    }

    private void MoveToLeft() {
        float xOffset = -1f;
        transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z);
    }
}
