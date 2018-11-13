using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeze : MonoBehaviour {

    public static PlayerFreeze instance = null;

    public bool playerIsFrozen;
    
    private void Awake () {
        instance = this;
	}

    public IEnumerator FreezePlayer(float freezeTime) {
        playerIsFrozen = true;

        yield return new WaitForSeconds(freezeTime);

        playerIsFrozen = false;
    }
	

}
