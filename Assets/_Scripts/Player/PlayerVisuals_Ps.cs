using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals_Ps : MonoBehaviour {

    //References
    private Transform PlayerVfx;
    private PlayerBase PlayerBase;
    private PlayerGroundDetection PlayerGroundDetection;
    private PlayerValues PlayerValues;

    //Private Values
    private bool isPlayingLandPs;
    private int switchDashInt;
    private int switchLandInt;
    private int switchJumpInt;
    private float quarterDashTime;
    private Vector3 ps_pos_left;
    private Vector3 ps_pos_right;

    [Header("Important Public Values")]
    public int grassWalkingEmission;
    public int grassDashingEmission;
    public Color ps_dash_startColor;

    [Header("Other Particle Systems")]
    public ParticleSystem ps_grassWalking;
    public ParticleSystem ps_grassDashingL;
    public ParticleSystem ps_grassDashingR;
    public ParticleSystem ps_death;

    [Header("Jump Particle Systems")]
    public ParticleSystem ps_jumpL_1;
    public ParticleSystem ps_jumpR_1;
    public ParticleSystem ps_jumpL_2;
    public ParticleSystem ps_jumpR_2;
    public ParticleSystem ps_jumpL_grass_1;
    public ParticleSystem ps_jumpR_grass_1;
    public ParticleSystem ps_jumpL_grass_2;
    public ParticleSystem ps_jumpR_grass_2;
    public float jumpParentTime = 0.6f;

    [Header("Land Particle Systems")]
    public ParticleSystem ps_landL_large_1;
    public ParticleSystem ps_landR_large_1;
    public ParticleSystem ps_landL_small_1;
    public ParticleSystem ps_landR_small_1;
    public ParticleSystem ps_landL_large_2;
    public ParticleSystem ps_landR_large_2;
    public ParticleSystem ps_landL_small_2;
    public ParticleSystem ps_landR_small_2;
    public ParticleSystem ps_landL_grass_1;
    public ParticleSystem ps_landR_grass_1;
    public ParticleSystem ps_landL_grass_2;
    public ParticleSystem ps_landR_grass_2;
    public float landParentTime = 0.4f;

    [Header("Dash Particle Systems")]
    public ParticleSystem ps_dash1_1;
    public ParticleSystem ps_dash2_1;
    public ParticleSystem ps_dash3_1;
    public ParticleSystem ps_dash1_2;
    public ParticleSystem ps_dash2_2;
    public ParticleSystem ps_dash3_2;
    public float dashParentTime = 0.4f;

    private void Start() {
        PlayerVfx = transform.Find("PlayerVfx");
        PlayerBase = GetComponentInParent<PlayerBase>();
        PlayerGroundDetection = GetComponentInParent<PlayerGroundDetection>();
        PlayerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");

        switchDashInt = -1;
        switchLandInt = -1;
        switchJumpInt = -1;

        quarterDashTime = PlayerValues.dashTime / 4;
        ps_pos_left = ps_landL_large_1.transform.localPosition;
        ps_pos_right = ps_landR_large_1.transform.localPosition;
        var ps_dash1_1_main = ps_dash1_1.main;
        var ps_dash2_1_main = ps_dash2_1.main;
        var ps_dash3_1_main = ps_dash3_1.main;
        var ps_dash1_2_main = ps_dash1_2.main;
        var ps_dash2_2_main = ps_dash2_2.main;
        var ps_dash3_2_main = ps_dash3_2.main;
        ps_dash1_1_main.startColor = ps_dash_startColor;
        ps_dash2_1_main.startColor = ps_dash_startColor;
        ps_dash3_1_main.startColor = ps_dash_startColor;
        ps_dash1_2_main.startColor = ps_dash_startColor;
        ps_dash2_2_main.startColor = ps_dash_startColor;
        ps_dash3_2_main.startColor = ps_dash_startColor;
    }

    private void OnEnable() {
        PlayerHitted.onEventHittedBox += Event_PlayerHittedBox;
        PlayerBase.onEventDash += Event_DashParticles;
        PlayerRaycasts.onEventGrounded += Event_LandParticles;
        PlayerBase.onEventJump += Event_JumpParticles;
    }

    private void OnDisable() {
        PlayerHitted.onEventHittedBox -= Event_PlayerHittedBox;
        PlayerBase.onEventDash -= Event_DashParticles;
        PlayerRaycasts.onEventGrounded -= Event_LandParticles;
        PlayerBase.onEventJump -= Event_JumpParticles;
    }

    //Grass Particles
    private void Update() {
        Vector3 grassScale = new Vector3(PlayerBase.lookDirection, 1, 1);
        ps_grassWalking.transform.localScale = grassScale;
        var emGrassWalking = ps_grassWalking.emission;
        var emGrassDashingL = ps_grassDashingL.emission;
        var emGrassDashingR = ps_grassDashingR.emission;

        //GrassWalking
        if (PlayerBase.movementSpeed > 0 && PlayerGroundDetection.isInGrass) {
            emGrassWalking.rateOverTime = grassWalkingEmission;   
        }
        else {
            emGrassWalking.rateOverTime = 0f;
        }
        //GrassDashing
        if (PlayerBase.isDashing && PlayerGroundDetection.isInGrass) {
            if(PlayerBase.lookDirection == 1) {
                emGrassDashingL.rateOverTime = grassDashingEmission;
            }
            else {
                emGrassDashingR.rateOverTime = grassDashingEmission;
            }
        }
        else {
            emGrassDashingL.rateOverTime = 0f;
            emGrassDashingR.rateOverTime = 0f;
        }
    }

    private void Event_PlayerHittedBox() {
        ps_death.Play();
        //StartCoroutine(Death_Ps_Logic());
    }

    private IEnumerator Death_Ps_Logic() {
        yield return new WaitForSeconds(0.25f);
    }

    private void Event_DashParticles() {
        StartCoroutine(Dash_Ps_Logic());
    }

    private IEnumerator Dash_Ps_Logic() {
        switchDashInt *= -1;
        if (switchDashInt == 1) {
            yield return new WaitForSeconds(quarterDashTime);
            StartCoroutine(Ps_Logic(ps_dash1_1, dashParentTime, Vector3.zero));
            yield return new WaitForSeconds(quarterDashTime);
            StartCoroutine(Ps_Logic(ps_dash2_1, dashParentTime, Vector3.zero));
            yield return new WaitForSeconds(quarterDashTime);
            StartCoroutine(Ps_Logic(ps_dash3_1, dashParentTime, Vector3.zero));
        }
        else {
            yield return new WaitForSeconds(quarterDashTime);
            StartCoroutine(Ps_Logic(ps_dash1_2, dashParentTime, Vector3.zero));
            yield return new WaitForSeconds(quarterDashTime);
            StartCoroutine(Ps_Logic(ps_dash2_2, dashParentTime, Vector3.zero));
            yield return new WaitForSeconds(quarterDashTime);
            StartCoroutine(Ps_Logic(ps_dash3_2, dashParentTime, Vector3.zero));
        }
    }

    private void Event_LandParticles() {
        isPlayingLandPs = true;
        StartCoroutine(IsPlayingLandParticles());
        switchLandInt *= -1;
        if (switchLandInt == 1) {
            if (PlayerGroundDetection.isInGrass) {
                StartCoroutine(Ps_Logic(ps_landL_grass_1, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_landR_grass_1, landParentTime, ps_pos_right));
            }
            else {
                StartCoroutine(Ps_Logic(ps_landL_large_1, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_landR_large_1, landParentTime, ps_pos_right));
                StartCoroutine(Ps_Logic(ps_landL_small_1, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_landR_small_1, landParentTime, ps_pos_right));
            }
        }
        else {
            if (PlayerGroundDetection.isInGrass) {
                StartCoroutine(Ps_Logic(ps_landL_grass_2, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_landR_grass_2, landParentTime, ps_pos_right));
            }
            else {
                StartCoroutine(Ps_Logic(ps_landL_large_2, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_landR_large_2, landParentTime, ps_pos_right));
                StartCoroutine(Ps_Logic(ps_landL_small_2, landParentTime, ps_pos_left));
                StartCoroutine(Ps_Logic(ps_landR_small_2, landParentTime, ps_pos_right));
            }
        }
    }

    private IEnumerator IsPlayingLandParticles() {
        yield return new WaitForSeconds(landParentTime);
        isPlayingLandPs = false;
    }

    private void Event_JumpParticles() {
        if (!isPlayingLandPs) {
            switchJumpInt *= -1;
            if (switchJumpInt == 1) {
                if (PlayerGroundDetection.isInGrass) {
                    StartCoroutine(Ps_Logic(ps_jumpL_grass_1, jumpParentTime, ps_pos_left));
                    StartCoroutine(Ps_Logic(ps_jumpR_grass_1, jumpParentTime, ps_pos_right));
                }
                else {
                    StartCoroutine(Ps_Logic(ps_jumpL_1, jumpParentTime, ps_pos_left));
                    StartCoroutine(Ps_Logic(ps_jumpR_1, jumpParentTime, ps_pos_right));
                }
            }
            else {
                if (PlayerGroundDetection.isInGrass) {
                    StartCoroutine(Ps_Logic(ps_jumpL_grass_2, jumpParentTime, ps_pos_left));
                    StartCoroutine(Ps_Logic(ps_jumpR_grass_2, jumpParentTime, ps_pos_right));
                }
                else {
                    StartCoroutine(Ps_Logic(ps_jumpL_2, jumpParentTime, ps_pos_left));
                    StartCoroutine(Ps_Logic(ps_jumpR_2, jumpParentTime, ps_pos_right));
                }
            }
        }
    }

    private IEnumerator Ps_Logic(ParticleSystem ps_System, float timeBeforeParent, Vector3 localPos) {
        //Unparent & Play
        ps_System.transform.parent = null;
        ps_System.Play();

        //Wait longer then lifetime before parent
        yield return new WaitForSeconds(timeBeforeParent);

        //Parent & Set LocalPos
        ps_System.transform.parent = PlayerVfx;
        ps_System.transform.localPosition = localPos;
        ps_System.transform.localScale = Vector3.one;
    }

}
