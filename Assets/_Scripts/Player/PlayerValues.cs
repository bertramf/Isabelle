using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValues : MonoBehaviour {

    [Header("Horizontal Movement Values")]
    public float horJoystickTreshold = 0.8f;
    public float maxMovementSpeed = 2.3f;
    public float airDelayTime = 0.15f;

    [Header("Gravity Values")]
    public float normalGravity = 1f;
    public float fallGravity = 1.1f;

    [Header("Jump Values")]
    public float yVelClamp_min = -7.5f;
    public float yVelClamp_max = 7.3f;
    public float earlyReleaseFactor = 3f;
    public float coyoteTime = 0.05f;

    [Header("Dash Values")]
    public float RtTreshold = 0.7f;
    public float dashSpeed = 10f;
    public float dashTime = 0.2f;
    public float dashGroundCooldown = 0.4f;

    [Header("Other Values")]
    public float freezeTime = 0.1f;

}
