using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTrigger : MonoBehaviour{

    private TextManager TextManager;
    private TextMeshPro text;
    private string triggerState;
    private float currentAlpha = 0f;

    private void Start(){
        TextManager = GetComponentInParent<TextManager>();
        text = GetComponentInParent<TextMeshPro>();
        text.faceColor = new Color(1, 1, 1, currentAlpha);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag != "Player") {
            return;
        }

        triggerState = "goEnter";
        if (currentAlpha == 0f) {
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }

        triggerState = "goExit";
        if (currentAlpha == 1f) {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn() {
        float t = 0f;
        float fadeTime = TextManager.fadeInTime;

        while (t < 1f) {
            t += Time.deltaTime / fadeTime;
            currentAlpha = t;
            text.faceColor = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }
        currentAlpha = 1f;
        text.faceColor = new Color(1, 1, 1, currentAlpha);

        if (triggerState == "goExit") {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut() {
        float t = 1f;
        float fadeTime = TextManager.fadeOutTime;

        while (t > 0f) {
            t -= Time.deltaTime / fadeTime;
            currentAlpha = t;
            text.faceColor = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }
        currentAlpha = 0f;
        text.faceColor = new Color(1, 1, 1, currentAlpha);

        if(triggerState == "goEnter") {
            StartCoroutine(FadeIn());
        }
    }

}
