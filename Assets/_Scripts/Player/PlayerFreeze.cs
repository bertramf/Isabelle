using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeze : MonoBehaviour {

    public static PlayerFreeze instance = null;

    public bool playerIsFrozen;
    
    private void Awake () {
        instance = this;
	}

    public void FreezePlayer(float freezeTime) {
        StartCoroutine(FreezePlayerLogic(freezeTime));
    }

    private IEnumerator FreezePlayerLogic(float time) {
        playerIsFrozen = true;

        yield return new WaitForSeconds(time);

        playerIsFrozen = false;
    }
	

}
