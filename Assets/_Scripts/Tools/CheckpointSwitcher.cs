using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSwitcher : MonoBehaviour{

    private Component[] checkpointComponents;
    public Vector3[] checkpointPositions;

    private PlayerValues playerValues;
    private float inputRt;
    private float inputLt;
    private float timerRt = 0f;
    private float timerLt = 0f;
    private float timerCooldown = 0.5f;

    public int checkpointIndex;
    public int checkpointLength;

    private void Start() {
        playerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");
    }

    private void OnEnable() {
        GameManager.onEventSceneLoaded += Event_LevelLoaded;
    }

    private void OnDisable() {
        GameManager.onEventSceneLoaded -= Event_LevelLoaded;
    }

    public void Event_LevelLoaded() {
        checkpointIndex = -1;
        //Find reference
        if (GameObject.Find("CheckpointParent") == null) {
            print("Er is geen checkpoint parent, of hij kan hem niet vinden");
            return;
        }
        GameObject CheckpointParent = GameObject.Find("CheckpointParent");

        if (CheckpointParent.GetComponentsInChildren<CheckpointBehaviour>() == null) {
            print("De checkpoint parent is leeg");
            return;
        }
        checkpointComponents = CheckpointParent.GetComponentsInChildren<CheckpointBehaviour>();
        
        checkpointPositions = new Vector3[checkpointComponents.Length];
        checkpointLength = checkpointPositions.Length;
        for (int i = 0; i < checkpointComponents.Length; i++) {
            checkpointPositions[i] = checkpointComponents[i].GetComponent<CheckpointBehaviour>().spawnPosition;
        }
    }

    private void Update() {
        if (!DevMode.Instance.devMode) {
            return;
        }
        if(GameManager.Instance.gameState != GameStates.playing) {
            return;
        }

        CheckCheckpointSwitch();
    }

    private void CheckCheckpointSwitch() {
        float previousInputRT = inputRt;
        inputRt = Input.GetAxisRaw("RT");
        float previousInputLT = inputLt;
        inputLt = Input.GetAxisRaw("LT");

        if (inputRt > playerValues.RtTreshold && timerRt < 0) {
            timerRt = timerCooldown;
        }
        if (inputLt > playerValues.RtTreshold && timerLt < 0) {
            timerLt = timerCooldown;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0) || (Input.GetButton("B") && (inputRt > playerValues.RtTreshold && timerRt == timerCooldown))) {
            if (checkpointIndex >= (checkpointLength - 1)) {
                return;
            }
            checkpointIndex += 1;
            TeleportPlayer();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || (Input.GetButton("B") && (inputLt > playerValues.RtTreshold && timerLt == timerCooldown))) {
            if (checkpointIndex <= 0) {
                return;
            }
            checkpointIndex -= 1;
            TeleportPlayer();
        }

        timerRt -= Time.fixedDeltaTime;
        timerLt -= Time.fixedDeltaTime;
    }

    private void TeleportPlayer() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = checkpointPositions[checkpointIndex];
    }

}
