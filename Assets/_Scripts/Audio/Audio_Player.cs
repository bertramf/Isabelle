using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Player : MonoBehaviour {

    private void OnEnable() {
        PlayerBase.onEventDash += Event_Dash;
        PlayerBase.onEventJump += Event_Jump;
        PlayerRaycasts.onEventGrounded += Event_Land;
        
    }

    private void OnDisable() {
        PlayerBase.onEventDash -= Event_Dash;
        PlayerBase.onEventJump -= Event_Jump;
        PlayerRaycasts.onEventGrounded -= Event_Land;
    }

    private void Event_Dash() {

    }

    private void Event_Jump() {

    }

    private void Event_Land() {

    }
 
}
