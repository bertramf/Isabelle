using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSwitcher : MonoBehaviour{

    private PlayerValues playerValues;
    private float inputRt;
    private float inputLt;
    private float timerRt = 0f;
    private float timerLt = 0f;
    private float timerCooldown = 0.5f;

    public Text currentLevelText;

    private void Start() {
        playerValues = Resources.Load<PlayerValues>("Settings/PlayerValues");
    }

    private void Update() {
        if (!DevMode.Instance.devMode) {
            return;
        }

        ShowCurrentScene();

        if (GameManager.Instance.gameState == GameStates.loading) {
            return;
        }

        ChangeDepthIndex();
        ChangeHorizontalIndex();
    }

    private void ShowCurrentScene() {
        currentLevelText.text = GameManager.Instance.newSceneNameReadOnly;
    }

    private void ChangeDepthIndex() {
        if (Input.GetKeyDown(KeyCode.Equals) || (Input.GetButton("B") && Input.GetButtonDown("RB"))) {
            GameManager.Instance.DEV_ChangeDepthSceneIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.Minus) || (Input.GetButton("B") && Input.GetButtonDown("LB"))) {
            GameManager.Instance.DEV_ChangeDepthSceneIndex(-1);
        }
    }

    private void ChangeHorizontalIndex() {
        float previousInputRT = inputRt;
        inputRt = Input.GetAxisRaw("RT");
        float previousInputLT = inputLt;
        inputLt = Input.GetAxisRaw("LT");

        if(inputRt > playerValues.RtTreshold && timerRt < 0) {
            timerRt = timerCooldown;
        }
        if(inputLt > playerValues.RtTreshold && timerLt < 0) {
            timerLt = timerCooldown;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0) || (Input.GetButton("B") && (inputRt > playerValues.RtTreshold && timerRt == timerCooldown))) {
            int depthIndex = GameManager.Instance.depthSceneIndexReadOnly;
            GameManager.Instance.depthSceneLevels[depthIndex].DEV_ChangeHorizontalSceneIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || (Input.GetButton("B") && (inputLt > playerValues.RtTreshold && timerLt == timerCooldown))) {
            int depthIndex = GameManager.Instance.depthSceneIndexReadOnly;
            GameManager.Instance.depthSceneLevels[depthIndex].DEV_ChangeHorizontalSceneIndex(-1);
        }

        timerRt -= Time.fixedDeltaTime;
        timerLt -= Time.fixedDeltaTime;
    }

}
