using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals_Ps : MonoBehaviour {

    private Transform playerVfx;
    private Transform randomT;
    private PlayerBase playerBase;
    private PlayerValues playerValues;

    [Header("Important Values; need to be correct")]
    public int landPsCount = 4;
    public int jumpPsCount = 2;

    [Header("Jump Particle Systems")]
    public ParticleSystem[] ps_jump_1;
    public ParticleSystem[] ps_jump_2;
    public float xOffsetWalking = 0.12f;
    private ParticleSystem[] ps_jump_holder;
    private Vector3[] ps_jump_startPos;
    private float[] ps_jump_xPos;
    private float switchInt_jump = 1;
    private float switchInt_land = 1;

    [Header("Dash Particle Systems")]
    public ParticleSystem ps_dash_1;
    public ParticleSystem ps_dash_2;
    public ParticleSystem ps_dash_3;

    private void Start () {
        playerVfx = transform.Find("PlayerVfx");
        if(GameObject.Find("ParticleCatcher") != null) {
            randomT = GameObject.Find("ParticleCatcher").transform;
        }
        playerBase = GetComponent<PlayerBase>();
        playerValues = GetComponent<PlayerValues>();

        ps_jump_holder = new ParticleSystem[landPsCount + jumpPsCount];
        ps_jump_startPos = new Vector3[landPsCount + jumpPsCount];
        ps_jump_xPos = new float[landPsCount + jumpPsCount];

        for (int i = 0; i < ps_jump_1.Length; i++) {
            ps_jump_holder[i] = ps_jump_1[i];
        }
        for (int i = 0; i < ps_jump_holder.Length; i++) {
            ps_jump_startPos[i] = ps_jump_holder[i].transform.localPosition;
        }
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
        StartCoroutine(DashParticle_Logic());
    }

    private IEnumerator DashParticle_Logic() {
        float partialTime = playerValues.dashTime / 4;

        DashParticle_Play(ps_dash_1);
        yield return new WaitForSeconds(partialTime);
        DashParticle_Play(ps_dash_2);
        yield return new WaitForSeconds(partialTime);
        DashParticle_Play(ps_dash_3);
    }

    private void DashParticle_Play(ParticleSystem ps_dash) {
        Vector3 vfxScale = new Vector3(playerBase.lookDirection, 1, 1);
        //Unparent
        ps_dash.transform.parent = null;
        //Set Scale goed
        ps_dash.transform.localScale = vfxScale;
        //Play particle
        ps_dash.Play();
        //Parent particle
        StartCoroutine(DashParticle_Parent(ps_dash));
    }

    private IEnumerator DashParticle_Parent(ParticleSystem ps_dash) {
        Vector3 localPositionPsDash = Vector3.zero;

        yield return new WaitForSeconds(playerValues.dashTime + playerValues.dashGroundCooldown - 0.05f);
        ps_dash.transform.SetParent(playerVfx);
        ps_dash.transform.localPosition = localPositionPsDash;
    }

    private void Event_LandParticles() {
        StartCoroutine(JumpParticleLogic(0, landPsCount, 0.35f));
    }

    private void Event_JumpParticles() {
        StartCoroutine(JumpParticleLogic(landPsCount, (landPsCount + jumpPsCount), 0.45f));
    }

    private IEnumerator JumpParticleLogic(int iStart, int iEnd, float timeBeforeParent) {
        Vector3 vfxScale = new Vector3(playerBase.lookDirection, 1, 1);

        //Parent particle
        for (int i = iStart; i < iEnd; i++) {
            ps_jump_holder[i].transform.SetParent(playerVfx);
        }

        //Set xPos, Unparent & Play particle
        for (int i = iStart; i < iEnd; i++) {
            xOffsetWalking = playerBase.lookDirection == -1 ? -0.17f : 0.12f;
            ps_jump_xPos[i] = playerBase.movementSpeed > 0 ? (ps_jump_startPos[i].x + xOffsetWalking) * playerBase.lookDirection : ps_jump_startPos[i].x * playerBase.lookDirection;
            ps_jump_holder[i].transform.localScale = vfxScale;
            ps_jump_holder[i].transform.localPosition = new Vector3(ps_jump_xPos[i], ps_jump_startPos[i].y, ps_jump_startPos[i].z);
            ps_jump_holder[i].transform.parent = null;
            if(randomT != null) {
                ps_jump_holder[i].transform.SetParent(randomT);
            }
            ps_jump_holder[i].Play();
        }

        //Switch System
        if (iStart == 0) {
            switchInt_land *= -1;
            for (int i = iStart; i < iEnd; i++) {
                ps_jump_holder[i] = switchInt_land == -1 ? ps_jump_2[i] : ps_jump_1[i];
            }
        }
        else {
            switchInt_jump *= -1;
            for (int i = iStart; i < iEnd; i++) {
                ps_jump_holder[i] = switchInt_jump == -1 ? ps_jump_2[i] : ps_jump_1[i];
            }
        }

        yield return new WaitForSeconds(timeBeforeParent);
    }

}
