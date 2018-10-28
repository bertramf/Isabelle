using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMode : MonoBehaviour {

    public static DevMode Instance;
    public bool devMode = false;

    public CanvasGroup debugCanvasGroup;

	private void Awake () {
		if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
	}

    private void Update() {
        if (Input.GetButtonDown("DevMode")) {
            if(devMode) {
                devMode = false;
                DeActivateDevMode();
            }
            else {
                devMode = true;
                ActivateDevMode();
            }
        }
    }

    private void ActivateDevMode() {
        debugCanvasGroup.alpha = 1f;
    }

    private void DeActivateDevMode() {
        debugCanvasGroup.alpha = -1f;
    }


}
