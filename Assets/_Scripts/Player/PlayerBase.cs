﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Designed for all player core mechanics & their behaviour
public class PlayerBase : MonoBehaviour {

    public delegate void EventJump();
    public delegate void EventDash();
    public delegate void EventDashRecharged();
    public delegate void EventChangeDirection(int direction);
    public static event EventJump onEventJump;
    public static event EventDash onEventDash;
    public static event EventDashRecharged onEventDashRecharged;
    public static event EventChangeDirection onEventChangeDirection;

    private Rigidbody2D rb;
    private PlayerValues playerValues;
    private PlayerRaycasts playerRaycasts;

    [Header("Horizontal Movement Values")]
    public int lookDirection; //Moet public zijn voor camera & raycasting
    public float inputHorizontal;
    public float movementSpeed; //Moet public zijn voor playervisuals

    [Header("Jump Values")]
    public float upVelocity; //Moet public zijn voor playervisuals

    [Header("Dash Values")]
    public bool secondDash = false;
    public bool dashCooldownOver = false;
    public bool isDashing;
    public float dashSpeed;
    public float inputRt;

    private IEnumerator dashCoroutine;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerValues = GetComponent<PlayerValues>();
        playerRaycasts = GetComponent<PlayerRaycasts>();

        lookDirection = 1;
        dashCoroutine = DashLoop();
    }

    private void Update () {
        playerRaycasts.Raycasting();
        playerRaycasts.CoyoteTime();

        HorizontalBehaviour();
        DashBehaviour();
        JumpBehaviour();

        SetGravity();

        Movement();
	}

    private void HorizontalBehaviour() {
        float horJoystickTreshold = playerValues.horJoystickTreshold;
        float maxMovementSpeed = playerValues.maxMovementSpeed;

        inputHorizontal = Input.GetAxisRaw("Horizontal");

        //Player moves right or left: CONTINUOUS
        if ((inputHorizontal > horJoystickTreshold && lookDirection == 1) || (inputHorizontal < -horJoystickTreshold && lookDirection == -1)) {
            movementSpeed = maxMovementSpeed;
        }
        //Player turns to the right: 1 FRAME
        else if (inputHorizontal > horJoystickTreshold && lookDirection == -1) {
            movementSpeed = maxMovementSpeed;
            if (!isDashing) {
                lookDirection = 1;
                //Event Setter
                if (onEventChangeDirection != null) {
                    onEventChangeDirection(lookDirection);
                }
            } 
        }
        //Player turns to the left: 1 FRAME
        else if (inputHorizontal < -horJoystickTreshold && lookDirection == 1) {
            movementSpeed = maxMovementSpeed;
            if (!isDashing) {
                lookDirection = -1;
                //Event Setter
                if (onEventChangeDirection != null) {
                    onEventChangeDirection(lookDirection);
                }
            }
        }
        //No input, movementSpeed needs to be 0 (ground) or needs to decelerate (sky)
        else {
            if (playerRaycasts.coyoteGrounded) {
                movementSpeed = 0;
            }
            else {
                if(movementSpeed == playerValues.maxMovementSpeed) {
                    StartCoroutine(DecelerateAirMovement());
                }
            } 
        }
    }

    private IEnumerator DecelerateAirMovement() {
        float t = 1;
        while (t >= 0) {
            t -= Time.deltaTime / playerValues.airDelayTime;
            movementSpeed = playerValues.maxMovementSpeed * t;
            movementSpeed = Mathf.Clamp(movementSpeed, 0f, playerValues.maxMovementSpeed);
            yield return null;
        }
    }

    private bool canDash = true;

    public bool CanDash {
        get {
            return canDash;
        }
        set {
            canDash = value;
            //EventSetter
            if (onEventDashRecharged != null) {
                onEventDashRecharged();
            }
        }
    }

    private void DashBehaviour() {
        //CanDash Normal Check
        if (dashCooldownOver && playerRaycasts.coyoteGrounded && !canDash) {
            CanDash = true;
        }

        float previousInput = inputRt;
        inputRt = Input.GetAxisRaw("RT");
            //RT INPUT
        //if (previousInput != inputRt && inputRt > playerValues.RtTreshold) {
            //X INPUT
        //if (Input.GetButtonDown("X"){ 
        if (Input.GetButtonDown("X") || (previousInput != inputRt && inputRt > playerValues.RtTreshold)) { 
            if (CanDash == true) {
                StopCoroutine(dashCoroutine);
                dashCoroutine = DashLoop();
                StartCoroutine(dashCoroutine);
            }
        }
    }

    private IEnumerator DashLoop() {
        dashCooldownOver = false;
        canDash = false;
        isDashing = true;
        dashSpeed = playerValues.dashSpeed;
        //Event Setter
        if (onEventDash != null) {
            onEventDash();
        }

        yield return new WaitForSeconds(playerValues.dashTime);

        isDashing = false;
        dashSpeed = 0f;

        //Cooldown for CanDash Check
        bool hittedGround = false;
        float t = 0f;
        while (t < 1) {
            t += Time.deltaTime / playerValues.dashGroundCooldown;
            if (playerRaycasts.coyoteGrounded) {
                hittedGround = true;
            }
            yield return null;
        }
        if (hittedGround == true) {
            CanDash = true;
        }
        dashCooldownOver = true;
    }

    private void JumpBehaviour() {
        if (Input.GetButtonDown("A") && playerRaycasts.coyoteGrounded) {
            upVelocity = playerValues.yVelClamp_max;
            //Event Setter
            if (onEventJump != null) {
                onEventJump();
            }
        }
        else {
            upVelocity = Mathf.Clamp(rb.velocity.y, playerValues.yVelClamp_min, playerValues.yVelClamp_max);
        }
    }

    private void SetGravity() {
        if (isDashing) {
            rb.gravityScale = 0f;
        }
        else {
            if (rb.velocity.y >= 0) {
                if (!Input.GetButton("A")) {
                    rb.gravityScale = playerValues.normalGravity * playerValues.earlyReleaseFactor;
                }
                else {
                    rb.gravityScale = playerValues.normalGravity;
                }  
            }
            else {
                rb.gravityScale = playerValues.fallGravity;
            }
        }
    }

    private void Movement() {
        Vector2 movementDirection;
        //movementSpeed = Mathf.Clamp(movementSpeed, 0f, playerValues.maxMovementSpeed);
        if (PlayerFreeze.instance.playerIsFrozen) {
            movementDirection = Vector2.zero;
        }
        else {
            if (playerRaycasts.beforeWall) {
                if (isDashing) {
                    movementDirection = new Vector2(0, 0);
                }
                else {
                    movementDirection = new Vector2(0, upVelocity);
                }
            }
            else {
                if (isDashing) {
                    movementDirection = new Vector2(dashSpeed * lookDirection, 0);
                }
                else {
                    movementDirection = new Vector2(movementSpeed * lookDirection, upVelocity);
                }
            }
        }
        rb.velocity = movementDirection;
    }

}
