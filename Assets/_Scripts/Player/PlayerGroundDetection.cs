using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour{

    public delegate void SwitchGround(string ground);
    public static event SwitchGround onSwitchGround;

    public bool isInGrass;

    //GrassTriggerEnter
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "GrassLow" || other.gameObject.tag == "GrassHigh") {
            isInGrass = true;
            if (other.gameObject.tag == "GrassLow") {
                SetSwitchGround_Grass();
            }
            else if (other.gameObject.tag == "GrassHigh") {
                SetSwitchGround_GrassHigh();
            }
        }
    }

    //GrassTriggerExit
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "GrassLow" || other.gameObject.tag == "GrassHigh") {
            isInGrass = false;
            SetSwitchGround_Earth();
        }
    }

    private void SetSwitchGround_Earth() {
        if (onSwitchGround != null) {
            onSwitchGround("Earth");
        }
    }

    private void SetSwitchGround_Grass() {
        if (onSwitchGround != null) {
            onSwitchGround("GrassLow");
        }
    }

    private void SetSwitchGround_GrassHigh() {
        if (onSwitchGround != null) {
            onSwitchGround("GrassHigh");
        }
    }

}
