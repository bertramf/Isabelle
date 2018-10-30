using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValues : MonoBehaviour {

    [Header("Horizontal Movement Values")]
    public float horJoystickTreshold = 0.8f;
    public float maxMovementSpeed = 2f;
    public float airDelayTime = 0.15f;

    [Header("Gravity Values")]
    public float normalGravity = 1f;
    public float fallGravity = 1.1f;

    [Header("Jump Values")]
    public float yVelClamp_min = -7f;
    public float yVelClamp_max = 6f;
    public float earlyReleaseFactor = 3f;
    public float coyoteTime = 0.05f;

    [Header("Dash Values")]
    public float RtTreshold = 0.8f;
    public float dashSpeed = 8f;
    public float dashTime = 0.18f;
    public float dashGroundCooldown = 0.3f;

}
