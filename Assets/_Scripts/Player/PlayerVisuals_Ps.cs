using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals_Ps : MonoBehaviour {

    //References
    private Transform PlayerVfx;
    private PlayerBase PlayerBase;
    private PlayerValues PlayerValues;

    //Private Values
    private float quarterDashTime;
    private int switchDashInt;
    private int dashPsCount;
    private int switchLandInt;
    private int landPsCount;
    private int switchJumpInt;
    private int jumpPsCount;
    private Vector3 ps_pos_left;
    private Vector3 ps_pos_right;

    [Header("Dash Particle Systems")]
    public ParticleSystem[] ps_dash_1;
    public ParticleSystem[] ps_dash_2;
    public float dashParentTime = 0.4f;

    [Header("Land Particle Systems")]
    public ParticleSystem[] ps_land_left_1;
    public ParticleSystem[] ps_land_right_1;
    public ParticleSystem[] ps_land_left_2;
    public ParticleSystem[] ps_land_right_2;
    public float landParentTime = 0.4f;

    [Header("Jump Particle Systems")]
    public ParticleSystem[] ps_jump_left_1;
    public ParticleSystem[] ps_jump_right_1;
    public ParticleSystem[] ps_jump_left_2;
    public ParticleSystem[] ps_jump_right_2;
    public float jumpParentTime = 0.45f;

    private void Start() {
        PlayerVfx = transform.Find("PlayerVfx");
        PlayerBase = GetComponentInParent<PlayerBase>();
        PlayerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");

        switchDashInt = -1;
        switchLandInt = -1;
        switchJumpInt = -1;

        dashPsCount = ps_dash_1.Length;
        landPsCount = ps_land_left_1.Length;
        jumpPsCount = ps_jump_left_1.Length;

        quarterDashTime = PlayerValues.dashTime / 4;
        ps_pos_left = ps_land_left_1[0].transform.localPosition;
        ps_pos_right = ps_land_right_1[0].transform.localPosition;
    }

    private void OnEnable() {
        PlayerBase.onEventDash += Event_DashParticles;
        PlayerRaycasts.onEventGrounded += Event_LandParticles;
        PlayerBase.onEventJump += Event_JumpParticles;
    }

    private void OnDisable() {
        PlayerBase.onEventDash -= Event_DashParticles;
        PlayerRaycasts.onEventGrounded -= Event_LandParticles;
        PlayerBase.onEventJump -= Event_JumpParticles;
    }

    private void Event_DashParticles() {
        StartCoroutine(Dash_Ps_Logic());
    }

    private IEnumerator Dash_Ps_Logic() {
        switchDashInt *= -1;
        for (int i = 0; i < dashPsCount; i++) {
            yield return new WaitForSeconds(i * quarterDashTime);
            if (switchDashInt == 1) {
                StartCoroutine(Ps_Logic(ps_dash_1, i, dashParentTime, Vector3.zero));
            }
            else {
                StartCoroutine(Ps_Logic(ps_dash_2, i, dashParentTime, Vector3.zero));
            }
        }
    }

    private void Event_LandParticles() {
        switchLandInt *= -1;
        for (int i = 0; i < landPsCount; i++) {
            if (switchLandInt == 1) {
                StartCoroutine(Ps_Logic(ps_land_left_1, i, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_land_right_1, i, landParentTime, ps_pos_right));
            }
            else {
                StartCoroutine(Ps_Logic(ps_land_left_2, i, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_land_right_2, i, landParentTime, ps_pos_right));
            }
        }
    }

    private void Event_JumpParticles() {
        switchJumpInt *= -1;
        for (int i = 0; i < jumpPsCount; i++) {
            if (switchJumpInt == 1) {
                StartCoroutine(Ps_Logic(ps_jump_left_1, i, jumpParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_jump_right_1, i, jumpParentTime, ps_pos_right));
            }
            else {
                StartCoroutine(Ps_Logic(ps_jump_left_2, i, jumpParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_jump_right_2, i, jumpParentTime, ps_pos_right));
            }
        }
    }

    private IEnumerator Ps_Logic(ParticleSystem[] ps_System, int number, float timeBeforeParent, Vector3 localPos) {
        //Unparent & Play
        ps_System[number].transform.parent = null;
        ps_System[number].Play();

        //Wait longer then lifetime before parent
        yield return new WaitForSeconds(timeBeforeParent);

        //Parent & Set LocalPos
        ps_System[number].transform.parent = PlayerVfx;
        ps_System[number].transform.localPosition = localPos;
        ps_System[number].transform.localScale = Vector3.one;
    }

}
