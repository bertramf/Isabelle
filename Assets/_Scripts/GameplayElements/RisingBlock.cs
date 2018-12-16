using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingBlock : MonoBehaviour {

    private BoxCollider2D triggerCollider;
    private Rigidbody2D rb;

    [Header("Gameplay Values")]
    public int maxSteps = 3;
    public float maxCoolDownTime = 3f;
    public float beforeTriggerTime = 0.4f;

    [Header("Debug Values")]
    public float y;
    public bool PlayerOnBlock;
    public bool hitsTrigger;
    public int currentStep;
    public float startY;
    public float coolDownTimer;

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        triggerCollider = transform.Find("RisingBlock_Visuals").transform.Find("PlayerTrigger_Death").GetComponent<BoxCollider2D>();

        triggerCollider.enabled = false;
        currentStep = 0;
        startY = transform.position.y;
        coolDownTimer = 0f;
    }

    private void OnEnable() {
        PlayerBase.onEventJump += Event_StartMovementLogic;
    }

    private void OnDisable() {
        PlayerBase.onEventJump -= Event_StartMovementLogic;
    }

    private void FixedUpdate() {
        coolDownTimer += Time.deltaTime;

        if (coolDownTimer > maxCoolDownTime) {
            CalculateDown();
        }

        Movement();
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            PlayerOnBlock = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            PlayerOnBlock = false;
        }
    }

    private void Event_StartMovementLogic() {
        if (PlayerOnBlock) {
            StartCoroutine(MoveUpCheck());
        }
    }

    private IEnumerator MoveUpCheck() {
        float t = 0f;
        while (t < 1) {
            t += Time.deltaTime / beforeTriggerTime;
            if (hitsTrigger) {
                hitsTrigger = false;
                CalculateUp();
                t = 1f;
            }
            yield return null;
        }
    }

    private void CalculateUp() {
        if (currentStep < maxSteps) {
            currentStep += 1;
            StartCoroutine(MoveTile());
        }
    }

    private void CalculateDown() {
        if(currentStep > 0) {
            currentStep -= 1;
            StartCoroutine(MoveTile());
        }
    }

    private IEnumerator MoveTile() {
        triggerCollider.enabled = true;
        coolDownTimer = 0f;

        float t = 0f;
        while(t < 1) {
            t += Time.deltaTime / 0.2f;
            y = Mathf.Lerp(startY, startY + currentStep, t);
           
            yield return null;
        }

        triggerCollider.enabled = false;
    }

    private void Movement() {
        rb.velocity = new Vector3(0, y, 0);
    }

}
