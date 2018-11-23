using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour {

    public static Screenshake instance = null;
    
    private void Awake () {
        instance = this;
    }
	
	public IEnumerator ShakeHorizontal(int framesPerShake, float shakeDur, float shakeOffset) {
        float t = 0f;
        int a = 0;
        Vector3 shakePos = Vector3.zero;
        while(t < 1) {
            t += Time.deltaTime * framesPerShake / shakeDur;
            a++;
            if (a%2 == 1) {
                shakePos = new Vector3(0 + shakeOffset, 0, transform.position.z);
            }
            else if(a%2 == 0) {
                shakePos = new Vector3(0 - shakeOffset, 0, transform.position.z);
            }
            transform.localPosition = shakePos;

            for (int i = 0; i < framesPerShake; i++) {
                yield return null;
            }
        }
        transform.localPosition = new Vector3(0, 0, transform.position.z);
    }
}
