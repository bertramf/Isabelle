using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Designed for all Player core mechanics & their behaviour
public class PlayerBase : MonoBehaviour {

    public enum PlayerState {
        standard,
        dead,
        cutscene,
        dashing
    }

    public PlayerState playerState;

    public delegate void EventJump();
    public delegate void EventDash();
    public delegate void EventDashRecharged();
    public delegate void EventChangeDirection(int direction);
    public static event EventJump onEventJump;
    public static event EventDash onEventDash;
    public static event EventDashRecharged onEventDashRecharged;
    public static event EventChangeDirection onEventChangeDirection;

    private IEnumerator dashCoroutine;
    private Rigidbody2D rb;
    private PlayerRaycasts playerRaycasts;
    private PlayerValues playerValues;

    [Header("Horizontal Movement Values")]
    public int lookDirection; //Moet public zijn voor camera & raycasting
    public float inputHorizontal;
    public float movementSpeed; //Moet public zijn voor playerVisuals

    [Header("Jump Values")]
    public float upVelocity; //Moet public zijn voor playerVisuals

    [Header("Dash Values")]
    private float maxFallGravity;
    public float dashSpeed;
    public float inputRt;

    [Header("Boolean Values")]
    public bool storedX = false;
    public bool isFrozen = false;
    public bool dashCooldownOver = false;
    private bool canDash;
    public bool CanDash {
        get {
            return canDash;
        }
        set {
            if (value == true) {
                //EventSetter
                if (onEventDashRecharged != null) {
                    onEventDashRecharged();
                }
            }
            canDash = value;
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerRaycasts = GetComponent<PlayerRaycasts>();
        playerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");

        PlayerStartLogic();
    }

    public void PlayerStartLogic() {
        playerState = PlayerState.standard;
        maxFallGravity = playerValues.fallGravity;
        lookDirection = 1;
        CanDash = true;
        dashCoroutine = DashLoop();
    }

    private void Update () {
        playerRaycasts.Raycasting();
        playerRaycasts.CoyoteTime();

        if(playerState == PlayerState.standard || playerState == PlayerState.dashing) {
            HorizontalBehaviour();
            DashBehaviour();
            JumpBehaviour();
        }

        SetGravity();

        Movement();
	}

    private void HorizontalBehaviour() {
        float horJoystickTreshold = playerValues.horJoystickTreshold;
        float maxMovementSpeed = playerValues.maxMovementSpeed;

        //Only if isAlive? //HAAL DEZE COMMENT WEG ALS ALLES WERKT!
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        
        //Player moves right or left: CONTINUOUS
        if ((inputHorizontal > horJoystickTreshold && lookDirection == 1) || (inputHorizontal < -horJoystickTreshold && lookDirection == -1)) {
            movementSpeed = maxMovementSpeed;
        }
        //Player turns to the right: 1 FRAME
        else if (inputHorizontal > horJoystickTreshold && lookDirection == -1) {
            movementSpeed = maxMovementSpeed;
            lookDirection = 1;
            //Event Setter
            if (onEventChangeDirection != null) {
                onEventChangeDirection(lookDirection);
            }
        }
        //Player turns to the left: 1 FRAME
        else if (inputHorizontal < -horJoystickTreshold && lookDirection == 1) {
            movementSpeed = maxMovementSpeed;
            lookDirection = -1;
            //Event Setter
            if (onEventChangeDirection != null) {
                onEventChangeDirection(lookDirection);
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

    public void StopDash() {
        StopCoroutine(dashCoroutine);
        playerState = PlayerState.standard;
        dashSpeed = 0f;
    }

    private void DashBehaviour() {
        //CanDash Normal Check
        if (dashCooldownOver && playerRaycasts.coyoteGrounded && !CanDash) {
            CanDash = true;
        }

        //Input check
        if ((Input.GetButtonDown("X") || storedX) && !isFrozen) {
            storedX = false;
            if (CanDash == true) {
                StopDash();
                dashCoroutine = DashLoop();
                StartCoroutine(dashCoroutine);
            }
        }
    }

    private IEnumerator DashLoop() {
        playerState = PlayerState.dashing;
        dashCooldownOver = false;
        CanDash = false;
        dashSpeed = playerValues.dashSpeed;
        //Event Setter
        if (onEventDash != null) {
            onEventDash();
        }

        yield return new WaitForSeconds(playerValues.dashTime);
        
        dashSpeed = 0f;
        StartCoroutine(DashCooldown());
        //StartCoroutine(SlowGravityAfterDash()); //TWIJFEL OF DIT ECHT NODIG IS
        playerState = PlayerState.standard;
    }

    private IEnumerator DashCooldown() {
        bool hittedGround = false;

        //This loop measures if player touches 1 time the ground while in dashCooldown
        float t = 0f;
        while (t < 1) {
            t += Time.deltaTime / playerValues.dashGroundCooldown;
            if (playerRaycasts.coyoteGrounded) {
                hittedGround = true;
            }
            yield return null;
        }

        //After dashCooldownTime check if ground was touched, IF YES -> CanDash = true
        if (hittedGround == true) {
            CanDash = true;
        }
        dashCooldownOver = true;
    }

    private void JumpBehaviour() {
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

    private IEnumerator SlowGravityAfterDash() {
        float t = 0f;
        while (t < 1) {
            t += Time.deltaTime / 0.2f;
            playerValues.fallGravity = maxFallGravity * t;
            yield return null;
        }

        playerValues.fallGravity = maxFallGravity;
    }

    private void SetGravity() {
        if (playerState == PlayerState.dashing) {
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

        isFrozen = true;
        rb.bodyType = RigidbodyType2D.Static;
        
        float t = 0f;
        while(t < 1) {
            t += Time.deltaTime / time;
            if (Input.GetButtonDown("X")) {
                storedX = true;
            }
            yield return null;
        }

        isFrozen = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        playerState = PlayerState.standard;

        float yDirection = savedDirection.y;
        if (yDirection < 0) {
            yDirection = 0f;
        }
        savedDirection = new Vector2(savedDirection.x, yDirection);
        rb.velocity = savedDirection; 
    }

    public void KillPlayer() {
        playerState = PlayerState.dead;
        StopDash();
        movementSpeed = 0f;
    }

    private void Movement() {
        Vector2 movementDirection;
        if (playerRaycasts.beforeWall) {
            if (playerState == PlayerState.dashing) {
                movementDirection = new Vector2(0, 0);
            }
            else {
                movementDirection = new Vector2(0, upVelocity);
            }
        }
        else {
            if (playerState == PlayerState.dashing) {
                movementDirection = new Vector2(dashSpeed * lookDirection, 0);
            }
            else {
                movementDirection = new Vector2(movementSpeed * lookDirection, upVelocity);
            }
        }
        if(!isFrozen) {
            rb.velocity = movementDirection;
        }
    }

}