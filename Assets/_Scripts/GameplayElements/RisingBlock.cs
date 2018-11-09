using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingBlock : MonoBehaviour {

    public enum BlockState {
        idle,
        movingUp,
        movingDown
    }
    private Animator anim;

    [Header("Gameplay Values")]
    public int maxSteps = 3;
    public float maxCoolDownTime = 3f;

    [Header("Debug Values")]
    public BlockState blockState;
    public int startY;
    public int currentStep;
    public float coolDownTimer;

    private void Start() {
        anim = transform.Find("RisingBlock_Visuals").GetComponent<Animator>();

        blockState = BlockState.idle;
        startY = Mathf.RoundToInt(transform.position.y);
        currentStep = 0;
        coolDownTimer = 0f;
    }

    private void FixedUpdate() {
        if(blockState == BlockState.idle) {
            coolDownTimer += Time.deltaTime;
        }

        if (coolDownTimer > maxCoolDownTime) {
            coolDownTimer = 0f;
            CalculateDown();
        }

        if (coolDownTimer > (maxCoolDownTime - 1f) && currentStep > 0) {
            anim.SetBool("goTrill", true);
        }
        else {
            anim.SetBool("goTrill", false);
        } 
    }

    public void CalculateUp() {
        if (currentStep < maxSteps && blockState == BlockState.idle) {
            currentStep += 1;
            StartCoroutine(MoveOneTileUp());
        }
    }

    private void CalculateDown() {
        if (currentStep > 0 && blockState == BlockState.idle) {
            currentStep -= 1;
            StartCoroutine(MoveOneTileDown());
        }
    }

    private IEnumerator MoveOneTileUp() {
        blockState = BlockState.movingUp;
        coolDownTimer = 0f;
        transform.position = new Vector3(transform.position.x, startY + currentStep, transform.position.z);
        yield return new WaitForSeconds(0.5f);
        blockState = BlockState.idle;
    }

    private IEnumerator MoveOneTileDown() {
        blockState = BlockState.movingUp;
        coolDownTimer = 0f;
        transform.position = new Vector3(transform.position.x, startY + currentStep, transform.position.z);
        yield return new WaitForSeconds(0.5f);
        blockState = BlockState.idle;
    }

}
