using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaler : MonoBehaviour {

    public Text timeScaleText;

    public float timeScale = 1f;
	
	private void Update () {
        if (DevMode.Instance.devMode) {
            if (timeScale >= 0.2f) {
                if (Input.GetKeyDown(KeyCode.LeftBracket)) {
                    timeScale -= 0.3f;
                }
            }

            if (timeScale <= 1.9f) {
                if (Input.GetKeyDown(KeyCode.RightBracket)) {
                    timeScale += 0.3f;
                }
            }
        }
        else {
            timeScale = 1f;
        }
        

        Time.timeScale = timeScale;

        //Show in UI
        string timeScaleStr = timeScale.ToString("F1");
        timeScaleText.text = "TimeScale = " + timeScaleStr;

	}
}
