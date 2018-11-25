using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTestje : MonoBehaviour {

    private Rigidbody2D rb;
    private float xMovement = 0;
    
    public float xSpeed = 5;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Movement();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            StartCoroutine(MovementToLeft());
        }
    }
     
    private IEnumerator MovementToLeft() {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / 0.5f;
            xMovement = -1f;
            yield return null;
        }
        xMovement = 0f;
    }

    private void Movement() {
        rb.velocity = new Vector3(xMovement * xSpeed, 0, 0);
    }
}
