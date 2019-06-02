using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour{

    public enum BirdState {
        idle,
        flying
    }
    public BirdState birdState;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 flyDirection;
    private int lookDirection;
    private float timer;
    private float idleTime;

    public LayerMask groundLayer;
    public float raycastLength;
    public float yRaycastOffset;
    public float flySpeed;

    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = transform.Find("BirdVisuals").GetComponent<Animator>();

        birdState = BirdState.idle;
        lookDirection = Mathf.RoundToInt(transform.localScale.x);
        idleTime = Random.Range(2f, 2.5f);
    }
    
    private void Update(){
        Raycasting();
        if(birdState == BirdState.idle) {
            RandomIdles();
        }
    }

    private void RandomIdles() {
        timer += Time.deltaTime;

        if (timer > idleTime) {
            timer = 0f;
            int randomRange = Random.Range(0, 20);
            if (randomRange == 0) {
                FlipSprite();
            }
            else if (randomRange < 5) {
                anim.SetTrigger("idle3");
            }
            else if (randomRange >= 5 && randomRange < 13) {
                anim.SetTrigger("idle2");
            }
            else {
                anim.SetTrigger("idle1");
            }
        }
    }

    private void FlipSprite() {
        lookDirection *= -1;
        transform.localScale = new Vector3(lookDirection, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            float xPosPlayer = other.gameObject.transform.position.x;
            StartCoroutine(FlyLogic(xPosPlayer));
        }
    }

    private IEnumerator FlyLogic(float xPosPlayer) {
        //Set State to Flying
        birdState = BirdState.flying;

        //Little cooldown
        float randomCooldown = Random.Range(0, 0.5f);
        yield return new WaitForSeconds(randomCooldown);

        //Direction calculation
        float xDirection;
        float xPosBird = transform.position.x;
        if((xPosBird - xPosPlayer) >= 0) {
            lookDirection = 1;
            xDirection = Random.Range(0.5f, 1f);
        }
        else {
            lookDirection = -1;
            xDirection = Random.Range(-0.5f, -1f);
        }
        transform.localScale = new Vector3(lookDirection, 1, 1);

        //Fly away
        flyDirection = new Vector2(xDirection, 1);
        rb.velocity = flyDirection.normalized * flySpeed;

        //Visuals
        anim.SetTrigger("fly");

        //Destroy after 10 seconds
        StartCoroutine(DestroyBird());
    }

    private IEnumerator DestroyBird() {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }

    private void Raycasting() {
        RaycastHit2D beforeWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + yRaycastOffset), new Vector2(lookDirection, 0), raycastLength, groundLayer);
        if (beforeWall) {
            ChangeFlyDirection();
        }
    }

    private void ChangeFlyDirection() {
        //Change facing direction (localXScale)
        lookDirection *= -1;
        transform.localScale = new Vector3(lookDirection, 1, 1);
        //Change flying direction (vector2)
        flyDirection = new Vector2(flyDirection.x * -1, 1);
        rb.velocity = flyDirection.normalized * flySpeed;
    }

    private void OnDrawGizmos() {
        BeforeWallGizmo();
    }

    private void BeforeWallGizmo() {
        int birdDirection = 1;
        if (Application.isPlaying) {
            birdDirection = lookDirection;
        }
        //Red
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + yRaycastOffset, transform.position.z), new Vector3(transform.position.x + (raycastLength * lookDirection), transform.position.y + yRaycastOffset, transform.position.z));
    }
}
