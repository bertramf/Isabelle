using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUp_Base : MonoBehaviour {

    //public enum BlockState { idle, prepare, moveUp, trill, moveDown }
    //public BlockState blockState;

    private IEnumerator currentCoroutine;
    private IEnumerator delayRoutine;
    private Animator anim;
    private ColorSwap colorSwap;

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

    public ColorSwapPreset inActivePreset;
    public ColorSwapPreset activePreset;

    private void Start() {
        anim = GetComponentInChildren<Animator>();
        colorSwap = GetComponentInChildren<ColorSwap>();
        delayRoutine = DelayInActivePreset();

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
        anim.SetTrigger("idle");
        if (currentStep > 0) {
            float t = 0f;
            while(t < 1f) {
                t += Time.deltaTime / beforeMoveDownTime;
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
        anim.SetTrigger("prepare");
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / warmUpTime;
            if (hitsTrigger) {
                hitsTrigger = false;
                //MoveUp Transition
                SwitchCoroutine(MoveUpFunction());
            }
            yield return null;
        }
        //Idle Transition
        SwitchCoroutine(IdleState());
    }

    private IEnumerator MoveUpFunction() {
        anim.SetTrigger("moveUp");
        yield return new WaitForSeconds(moveUpTime);
        //MoveUp Calculation
        currentStep += 1;
        transform.position = new Vector3(transform.position.x, startY + (currentStep * 1.5f), transform.position.z);
        //Idle Transition
        SwitchCoroutine(IdleState());
    }

    private IEnumerator TrillFunction() {
        anim.SetTrigger("trill");
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
        //MoveDown Calculation
        currentStep -= 1;
        transform.position = new Vector3(transform.position.x, startY + (currentStep * 1.5f), transform.position.z);
        anim.SetTrigger("moveDown");
        yield return new WaitForSeconds(moveUpTime);
        //Idle Transition
        SwitchCoroutine(IdleState());
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            //StopCoroutine(delayRoutine);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            playerOnBlock = true;
            colorSwap.UpdateVisualData(activePreset);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            playerOnBlock = false;
            StartCoroutine(DelayInActivePreset());
        }
    }

    private IEnumerator DelayInActivePreset() {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / 1f;
            if (playerOnBlock) {
                yield break;
            }
            yield return null;
        }
        colorSwap.UpdateVisualData(inActivePreset);
    }

}
