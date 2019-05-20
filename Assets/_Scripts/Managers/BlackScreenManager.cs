using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenManager : MonoBehaviour {

    public static BlackScreenManager Instance;
    
    public CanvasGroup black_CanvasGroup;
    public float fadeInTime = 0.35f;
    public float fadeBlackTime = 0.3f;
    public float fadeOutTime = 0.35f;

    private void Awake () {
        Instance = this;
    }

    public void SetCanvasAlpha(float newAlpha) {
        black_CanvasGroup.alpha = newAlpha;
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
