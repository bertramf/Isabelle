using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenManager : MonoBehaviour {

    public static BlackScreenManager Instance;

    [Header("Public References")]
    public CanvasGroup black_CanvasGroup;
    
    private void Awake () {
        Instance = this;
    }

    public IEnumerator FadeBlackScreen_In(float time) {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / time;
            black_CanvasGroup.alpha = t;
            yield return null;
        }
    }

    public IEnumerator FadeBlackScreen_Out(float time) {
        float t = 1f;
        while (t > 0f) {
            t -= Time.deltaTime / time;
            black_CanvasGroup.alpha = t;
            yield return null;
        }
    }

}
