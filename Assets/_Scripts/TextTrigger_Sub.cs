using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTrigger_Sub : MonoBehaviour{

    public enum TextState {
        inActive,
        active
    }
    public TextState textState;

    private TextManager TextManager;
    private TextMeshPro text;
    private string triggerState;
    private float currentAlpha;
    private float timer;

    public ParticleSystem ps_ring;
    public bool fadingOut;
    public float maxFadeTime;

    private void Start() {
        TextManager = GetComponentInParent<TextManager>();
        text = GetComponentInParent<TextMeshPro>();

        textState = TextState.inActive;
        currentAlpha = 0f;
        text.faceColor = new Color(1, 1, 1, currentAlpha);
    }

    private void Update() {
        text.faceColor = new Color(1, 1, 1, currentAlpha);

        if (textState == TextState.inActive) {
            return;
        }

        if (timer < maxFadeTime && !fadingOut) {
            
            currentAlpha = (timer / maxFadeTime);
            timer += Time.deltaTime; 
        }
        else {
            fadingOut = true;
            timer -= Time.deltaTime * 2;
            currentAlpha = (timer / maxFadeTime);
            if(timer <= -1f) {
                timer = 0f;
                ps_ring.Play();
                fadingOut = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }
        ps_ring.Play();
        textState = TextState.active;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag != "Player") {
            return;
        }
        textState = TextState.inActive;
        StartCoroutine(FadeOut(currentAlpha));
    }

    private IEnumerator FadeOut(float alpha) {
        float t = alpha;
        while (t > 0) {
            t -= (Time.deltaTime / alpha);
            currentAlpha = t;
            yield return null;
        }

    }

}
