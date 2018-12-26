using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUp_Base : MonoBehaviour {

    //public enum BlockState { idle, prepare, moveUp, trill, moveDown }
    //public BlockState blockState;

    private IEnumerator currentCoroutine;

    //Debug Values
    public bool playerOnBlock;
    public bool hitsTrigger;
    public int currentStep;
    public float startY;
    public float coolDownTimer;

    //Gameplay Values
    public int maxSteps = 3;
    public float warmUpTime = 0.4f;
    public float moveUpTime = 0.1f;
    public float trillTime = 0.6f;
    public float beforeMoveDownTime = 3f;

    private void Start() {
        currentStep = 0;
        startY = transform.position.y;
        coolDownTimer = 0f;

        SwitchCoroutine(IdleState());
    }

    private void OnEnable() {
        PlayerBase.onEventJump += Event_StartMovementLogic;
    }

    private void OnDisable() {
        PlayerBase.onEventJump -= Event_StartMovementLogic;
    }

    private void Event_StartMovementLogic() {
        if (playerOnBlock) {
            if(currentStep < maxSteps) {
                //Prepare Transition
                SwitchCoroutine(PrepareFunction());
            }
        }
    }

    private void SwitchCoroutine(IEnumerator nextCoroutine) {
        print("enter: " + nextCoroutine.ToString());
        if(currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = nextCoroutine;
        StartCoroutine(nextCoroutine);
    }


    //
    //STATES
    //

    private IEnumerator IdleState() {
        if (currentStep > 0) {
            float t = 0f;
            while(t < 1f) {
                t += Time.deltaTime / beforeMoveDownTime;
                if (!playerOnBlock) {
                    t = 0f;
                }
                yield return null;
            }
            //Trill Transition
            SwitchCoroutine(TrillFunction());
        }
        else {
            while (true) {
                yield return null;
            }
        }
    }

    private IEnumerator PrepareFunction() {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / warmUpTime;
            //MoveUp Transition
            if (hitsTrigger) {
                SwitchCoroutine(MoveUpFunction());
                yield break;
            }
            yield return null;
        }
        //Idle Transition
        SwitchCoroutine(IdleState());
    }

    private IEnumerator MoveUpFunction() {
        yield return new WaitForSeconds(moveUpTime);
        //MoveUp Calculation
        currentStep += 1;
        transform.position = new Vector3(transform.position.x, startY + (currentStep * 1.5f), transform.position.z);
        //Idle Transition
        SwitchCoroutine(IdleState());
    }

    private IEnumerator TrillFunction() {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / trillTime;
            //Idle Transition
            if (hitsTrigger) {
                SwitchCoroutine(IdleState());
            }
            yield return null;
        }
        //MoveDown Transition
        SwitchCoroutine(MoveDownFunction());

    }

    private IEnumerator MoveDownFunction() {
        yield return new WaitForSeconds(moveUpTime);
        //MoveDown Calculation
        currentStep -= 1;
        transform.position = new Vector3(transform.position.x, startY + (currentStep * 1.5f), transform.position.z);
        //Idle Transition
        SwitchCoroutine(IdleState());
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            playerOnBlock = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            playerOnBlock = false;
        }
    }

}
