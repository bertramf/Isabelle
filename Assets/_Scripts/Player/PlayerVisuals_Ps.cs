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
    private bool isInGrass;
    private float quarterDashTime;
    private int switchDashInt;
    private int dashPsCount;
    private int switchLandInt;
    private int landPsCount;
    private int switchJumpInt;
    private int jumpPsCount;
    private Vector3 ps_pos_left;
    private Vector3 ps_pos_right;

    [Header("Important Public Values")]
    public Color ps_groundColor;
    public Color ps_grassColor;
    public Material mat_ground;
    public Material mat_grass;
    public int grassWalkingEmission;
    public int grassDashingEmission;

    [Header("Other Particle Systems")]
    public ParticleSystem ps_grassWalking;
    public ParticleSystem ps_grassDashingL;
    public ParticleSystem ps_grassDashingR;
    public ParticleSystem ps_death;

    [Header("Jump Particle Systems")]
    public ParticleSystem[] ps_jump_left_1;
    public ParticleSystem[] ps_jump_right_1;
    public ParticleSystem[] ps_jump_left_2;
    public ParticleSystem[] ps_jump_right_2;
    public float jumpParentTime = 0.45f;

    [Header("Land Particle Systems")]
    public ParticleSystem[] ps_land_left_1;
    public ParticleSystem[] ps_landGrass_left_1;
    public ParticleSystem[] ps_land_right_1;
    public ParticleSystem[] ps_landGrass_right_1;
    public ParticleSystem[] ps_land_left_2;
    public ParticleSystem[] ps_landGrass_left_2;
    public ParticleSystem[] ps_land_right_2;
    public ParticleSystem[] ps_landGrass_right_2;
    public float landParentTime = 0.4f;

    [Header("Dash Particle Systems")]
    public ParticleSystem[] ps_dash_1;
    public ParticleSystem[] ps_dash_2;
    public float dashParentTime = 0.4f;

    private void Start() {
        PlayerVfx = transform.Find("PlayerVfx");
        PlayerBase = GetComponentInParent<PlayerBase>();
        PlayerGroundDetection = GetComponentInParent<PlayerGroundDetection>();
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
        for (int i = 0; i < dashPsCount; i++) {
            yield return new WaitForSeconds(i * quarterDashTime);
            if (switchDashInt == 1) {
                StartCoroutine(Ps_Logic(ps_dash_1, i, dashParentTime, Vector3.zero, false));
            }
            else {
                StartCoroutine(Ps_Logic(ps_dash_2, i, dashParentTime, Vector3.zero, false));
            }
        }
    }

    private void Event_LandParticles() {
        switchLandInt *= -1;
        for (int i = 0; i < landPsCount; i++) {
            if (switchLandInt == 1) {
                if (PlayerGroundDetection.isInGrass) {
                    StartCoroutine(Ps_Logic(ps_landGrass_left_1, i, landParentTime, ps_pos_left, true));
                    StartCoroutine(Ps_Logic(ps_landGrass_right_1, i, landParentTime, ps_pos_right, true));
                }
                else {
                    StartCoroutine(Ps_Logic(ps_land_left_1, i, landParentTime, ps_pos_left, true));
                    StartCoroutine(Ps_Logic(ps_land_right_1, i, landParentTime, ps_pos_right, true));
                } 
            }
            else {
                if (PlayerGroundDetection.isInGrass) {
                    StartCoroutine(Ps_Logic(ps_landGrass_left_2, i, landParentTime, ps_pos_left, true));
                    StartCoroutine(Ps_Logic(ps_landGrass_right_2, i, landParentTime, ps_pos_right, true));
                }
                else {
                    StartCoroutine(Ps_Logic(ps_land_left_2, i, landParentTime, ps_pos_left, true));
                    StartCoroutine(Ps_Logic(ps_land_right_2, i, landParentTime, ps_pos_right, true));
                }  
            }
        }
    }

    private void Event_JumpParticles() {
        switchJumpInt *= -1;
        for (int i = 0; i < jumpPsCount; i++) {
            if (switchJumpInt == 1) {
                StartCoroutine(Ps_Logic(ps_jump_left_1, i, jumpParentTime, ps_pos_left, true));
                StartCoroutine(Ps_Logic(ps_jump_right_1, i, jumpParentTime, ps_pos_right, true));
            }
            else {
                StartCoroutine(Ps_Logic(ps_jump_left_2, i, jumpParentTime, ps_pos_left, true));
                StartCoroutine(Ps_Logic(ps_jump_right_2, i, jumpParentTime, ps_pos_right, true));
            }
        }
    }

    private IEnumerator Ps_Logic(ParticleSystem[] ps_System, int number, float timeBeforeParent, Vector3 localPos, bool changeStartColor) {
        //Unparent & Play
        ps_System[number].transform.parent = null;
        //if (changeStartColor) {
        //    var main = ps_System[number].main;
        //    if (!PlayerGroundDetection.isInGrass) {
        //        ps_System[number].GetComponent<ParticleSystemRenderer>().material = mat_ground;
        //        main.startColor = ps_groundColor;
        //    }
        //    else {
        //        ps_System[number].GetComponent<ParticleSystemRenderer>().material = mat_grass;
        //        main.startColor = ps_grassColor;
        //    }
        //}
        ps_System[number].Play();

        //Wait longer then lifetime before parent
        yield return new WaitForSeconds(timeBeforeParent);

        //Parent & Set LocalPos
        ps_System[number].transform.parent = PlayerVfx;
        ps_System[number].transform.localPosition = localPos;
        ps_System[number].transform.localScale = Vector3.one;
    }

}
