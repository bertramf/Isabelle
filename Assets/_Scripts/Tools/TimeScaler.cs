using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaler : MonoBehaviour {

    public Text timeScaleText;
    public float timeScale = 1f;
	
	private void Update () {
        if (DevMode.Instance.devMode) {
            if (timeScale > 0.2f) {
                if (Input.GetKeyDown(KeyCode.LeftBracket) || (!Input.GetButton("B") && Input.GetButtonDown("LB"))) {
                    timeScale -= 0.2f;
                }
            }

            if (timeScale < 2.0f) {
                if (Input.GetKeyDown(KeyCode.RightBracket) || (!Input.GetButton("B") && Input.GetButtonDown("RB"))) {
                    timeScale += 0.2f;
                }
            }
        }
        else {
            timeScale = 1f;
        }
        

        Time.timeScale = timeScale;

        //Show in UI
        string timeScaleStr = timeScale.ToString("F1");
        timeScaleText.text = "TimeScale : " + timeScaleStr;

	}
}
