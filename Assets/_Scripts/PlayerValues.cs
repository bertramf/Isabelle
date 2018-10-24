using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValues : MonoBehaviour {

    [Header("Horizontal Movement Values")]
    public float horJoystickTreshold = 0.8f;
    public float maxMovementSpeed = 2.5f;
    public float airDelayTime = 0.1f;

    [Header("Gravity Values")]
    public float playerNormalGravity = 1f;
    public float playerFallGravity = 1.1f;
    public float gravityMultiplierMax = 1.25f;

    [Header("Jump Values")]
    public float yVelClamp_min = -6.5f;
    public float yVelClamp_max = 8.3f;
    public float minimumJumpFactor = 0.3f;
    public float coyoteTime = 0.07f;
    public float minJumpInputTime = 0.08f;

    [Header("Dash Values")]
    public float RtTreshold = 0.8f;
    public float dashSpeed = 9f;
    public float dashTime = 0.2f;
    public float dashGroundCooldown = 0.5f;


}
