using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationFire : MonoBehaviour {

    private PlayerVisuals playerVisuals;

	private void Start () {
        playerVisuals = transform.GetComponentInParent<PlayerVisuals>();
	}

}
