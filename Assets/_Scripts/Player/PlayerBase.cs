using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Designed for all player core mechanics & their behaviour
public class PlayerBase : MonoBehaviour {

    private Rigidbody2D rb;
    private PlayerValues playerValues;
    private PlayerRaycasts playerRaycasts;
    private PlayerVisuals playerVisuals;
    private CameraController cameraController;

    [Header("Horizontal Movement Values")]
    public int lookDirection = 1; //deze moet public zijn voor de camera en voor raycasts
    public float inputHorizontal;
    public float movementSpeed;

    [Header("Jump Values")]
    public bool earlyRelease;
    public int jumpState;
    public float upVelocity;
    public float jumpTime;
    public float gravityMultiplier;

    [Header("Dash Values")]
    public bool canDash = true;
    public bool dashCooldownOver = true;
    public bool canTurn = true;
    public bool isDashing;
    public float dashSpeed;

    private int i = 0;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerValues = GetComponent<PlayerValues>();
        playerRaycasts = GetComponent<PlayerRaycasts>();
        playerVisuals = GetComponent<PlayerVisuals>();
        cameraController = GameObject.Find("CameraParent").GetComponent<CameraController>();
        cameraController.SetStartPosition(transform.position.x, transform.position.y);

        playerRaycasts.onTouchGround += OnTouchGround;
    }
	
	private void Update () {
        playerRaycasts.Raycasting();
        playerRaycasts.CoyoteTime();

        HorizontalInput();
        DashInput();
        JumpBehaviour();
        SetGravity();

        Movement();

        JumpStates();
	}

    private void FixedUpdate() {
        JumpTimer();  
    }

    private void HorizontalInput() {
        float horJoystickTreshold = playerValues.horJoystickTreshold;
        float maxMovementSpeed = playerValues.maxMovementSpeed;

        inputHorizontal = Input.GetAxisRaw("Horizontal");

        //Player moves to the right or to the left
        if ((inputHorizontal > horJoystickTreshold && lookDirection == 1) || (inputHorizontal < -horJoystickTreshold && lookDirection == -1)) {
            movementSpeed = maxMovementSpeed;
        }
        //Player turns to the right: 1 frame true
        else if (inputHorizontal > horJoystickTreshold && lookDirection == -1) {
            movementSpeed = maxMovementSpeed;
            if (canTurn) {
                ChangeDirection(1);
            } 
        }
        //Player turns to the left: 1 frame true
        else if (inputHorizontal < -horJoystickTreshold && lookDirection == 1) {
            movementSpeed = maxMovementSpeed;
            if (canTurn) {
                ChangeDirection(-1);
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

    private void ChangeDirection(int direction) {
        lookDirection = direction;
        Vector3 playerScale = new Vector3(lookDirection, 1, 1);
        transform.localScale = playerScale;
    }

    private IEnumerator DecelerateAirMovement() {
        float t = 1;
        while (t >= 0) {
            t -= Time.deltaTime / playerValues.airDelayTime;
            movementSpeed = playerValues.maxMovementSpeed * t;
            yield return null;
        }
    }

    private void DashInput() {

        if(playerRaycasts.coyoteGrounded && dashCooldownOver) {
            canDash = true;
            dashCooldownOver = false;
        }

        float inputRt = Input.GetAxisRaw("RT");
        if(inputRt > playerValues.RtTreshold) {
            if (canDash) {
                StartCoroutine(DashLoop());
            }
        }
    }

    private IEnumerator DashLoop() {

        canDash = false;
        dashCooldownOver = false;
        canTurn = false;
        isDashing = true;
        dashSpeed = playerValues.dashSpeed;

        yield return new WaitForSeconds(playerValues.dashTime);

        canTurn = true;
        isDashing = false;
        dashSpeed = 0f;

        yield return new WaitForSeconds(playerValues.dashGroundCooldown);

        dashCooldownOver = true; 

    }

    private void SetGravity() {
        if (playerRaycasts.coyoteGrounded) {
            gravityMultiplier = 1;
        }

        if (isDashing) {
            rb.gravityScale = 0f;
        }
        else {
            if (rb.velocity.y >= 0) {
                rb.gravityScale = playerValues.playerNormalGravity * gravityMultiplier;
            }
            else if (rb.velocity.y < 0) {
                rb.gravityScale = playerValues.playerFallGravity * gravityMultiplier;
            }
        }
    }

    private void JumpBehaviour() {
        //EarlyRelease check
        if (Input.GetButtonUp("A") && jumpTime < playerValues.minJumpInputTime) {
            earlyRelease = true;
        }

        //Ga omhoog als je input A geeft
        if (Input.GetButtonDown("A") && playerRaycasts.coyoteGrounded) {
            jumpState = 1;
            jumpTime = 0f;
            upVelocity = playerValues.yVelClamp_max;
            //Jump Audio moet hier aangeroepen worden
        }
        //Dit gebeurd er nadat je na een earlyRelease bij de minimumJumpInputTime komt
        else if ((earlyRelease || Input.GetButtonUp("A")) && jumpTime >= playerValues.minJumpInputTime) {
            if (rb.velocity.y > 0) {
                earlyRelease = false;
                gravityMultiplier = playerValues.gravityMultiplierMax;
                upVelocity *= playerValues.minimumJumpFactor;
            }
        }
        else {
            upVelocity = Mathf.Clamp(rb.velocity.y, playerValues.yVelClamp_min, playerValues.yVelClamp_max);
        }
    }

    private void JumpStates() {

        ////Van jumpstate 1 naar jumpstate 2 (normale jump)
        //if (jumpState == 1 && rb.velocity.y < 2) {
        //    jumpState = 2;
        //}

        ////Van lopen naar vallen
        //if (jumpState == 0 && rb.velocity.y < -2) {
        //    jumpState = 2;
        //}

        //Wanneer grounded
        //Als ie voor het eerst grounded wordt
        if (upVelocity < -0.5f && playerRaycasts.grounded) {
            
            //JumpAudio();
        }
    }

    private void OnTouchGround() {
        upVelocity = 0f;
        jumpState = 0;
        playerVisuals.LandParticles();
        i++;
        print(i);
    }

    private void JumpTimer() {
        if (jumpState > 0) {
            jumpTime += Time.deltaTime;
        }
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
        rb.velocity = movementDirection;
    }

}
