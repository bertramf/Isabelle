using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Designed for all Player core mechanics & their behaviour
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
    private PlayerRaycasts playerRaycasts;
    private PlayerValues playerValues;

    [Header("Boolean Values")]
    public bool canDash = true;
    public bool isAlive = true; //Moet public zijn voor PlayerHitted 

    [Header("Horizontal Movement Values")]
    public int lookDirection; //Moet public zijn voor camera & raycasting
    public float inputHorizontal;
    public float movementSpeed; //Moet public zijn voor playerVisuals

    [Header("Jump Values")]
    public float upVelocity; //Moet public zijn voor playerVisuals

    [Header("Dash Values")]
    public bool dashCooldownOver = false;
    public bool isDashing;
    public float dashSpeed;
    public float inputRt;

    private IEnumerator dashCoroutine;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerRaycasts = GetComponent<PlayerRaycasts>();
        playerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");

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

        //Input check
        if (isAlive) {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
        }
        
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

    public void EnableCanDash() {
        canDash = true;
        //EventSetter
        if (onEventDashRecharged != null) {
            onEventDashRecharged();
        }
    }

    private void StopDash() {
        StopCoroutine(dashCoroutine);
    }

    private void DashBehaviour() {
        //CanDash Normal Check
        if (dashCooldownOver && playerRaycasts.coyoteGrounded && !canDash) {
            EnableCanDash();
        }

        float previousInput = inputRt;
        inputRt = Input.GetAxisRaw("RT");

        if (!isAlive) {
            return;
        }

        //Input check
        if (Input.GetButtonDown("X") || (previousInput != inputRt && inputRt > playerValues.RtTreshold)) {
            if (canDash == true) {
                StopDash();
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

        //yield return new WaitForSeconds(playerValues.dashTime);

        //HEEL ERG DISCUTABEL; WIL IK ECHT DAT EERSTE DASH NOG VERDERGAAT UIT DE DASH RECHARGE ORB?
        //CODE IS NU OOK ZO DAT DIT VOOR ALLE PLAYER FREEZES GELDT
        float t1 = 0f;
        while (t1 < 1) {
            if (rb.bodyType == RigidbodyType2D.Dynamic) {
                t1 += Time.deltaTime / playerValues.dashTime;
            }
            yield return null;
        }

        isDashing = false;
        dashSpeed = 0f;

        //Cooldown for CanDash Check
        bool hittedGround = false;
        float t2 = 0f;
        while (t2 < 1) {
            t2 += Time.deltaTime / playerValues.dashGroundCooldown;
            if (playerRaycasts.coyoteGrounded) {
                hittedGround = true;
            }
            yield return null;
        }
        if (hittedGround == true) {
            EnableCanDash();
        }
        dashCooldownOver = true;
    }

    private void JumpBehaviour() {
        if (!isAlive) {
            return;
        }

        //Input check & grounded check
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

    public void FreezePlayer(float freezeTime) {
        StartCoroutine(FreezePlayerLogic(freezeTime));
    }

    private IEnumerator FreezePlayerLogic(float time) {
        Vector2 savedDirection = rb.velocity;
        rb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(time);
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        float yDirection = savedDirection.y;
        if(yDirection < 0) {
            yDirection = 0f;
        }
        savedDirection = new Vector2(savedDirection.x, yDirection);
        rb.velocity = savedDirection;
    }

    public void KillPlayer() {
        isAlive = false;
        isDashing = false;
        StopDash();
        movementSpeed = 0f;
    }

    private void Movement() {
        Vector2 movementDirection;
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
        if (rb.bodyType == RigidbodyType2D.Dynamic) {
            rb.velocity = movementDirection;
        }
    }

}