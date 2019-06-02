using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashOrb : MonoBehaviour {

    private PlayerBase playerBase;
    private CircleCollider2D coll;
    private SpriteRenderer sprite;

    public ParticleSystem ps_disappear_1;
    public ParticleSystem ps_disappear_2;
    public ParticleSystem ps_alive;
    public Sprite chargedSpr;
    public Sprite deChargedSpr;

    [Header("Gameplay Values")]
    public float rechargeTime = 2f;
    public float PlayerFreezeTime = 0.05f;
    public float screenShakeTime = 0.1f;

    private void Start() {
        coll = GetComponent<CircleCollider2D>();
        sprite = transform.Find("DashOrb_Visuals").GetComponent<SpriteRenderer>();

        ps_alive.Play();
        RandomParticlePosition();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if(playerBase == null) {
                playerBase = other.gameObject.GetComponent<PlayerBase>();
            }
            StartCoroutine(RechargeLogic());
        }
    }

    private IEnumerator RechargeLogic() {
        Disappear();
        yield return new WaitForSeconds(rechargeTime);
        Appear();
    }

    private void Disappear() {
        //Player changes
        playerBase.FreezePlayer(PlayerFreezeTime);
        playerBase.CanDash = true;

        //Feedback other objects
        Screenshake.instance.StartShakeHorizontal(2, screenShakeTime, 0.1f);

        //Feedback on this gameObject
        coll.enabled = false;
        sprite.sprite = deChargedSpr;
        sprite.sortingLayerName = "Default";
        ps_disappear_1.Play();
        ps_disappear_2.Play();
        ps_alive.Stop();
    }

    private void Appear() {
        //Feedback on this gameObject
        coll.enabled = true;
        sprite.sprite = chargedSpr;
        sprite.sortingLayerName = "Gameplay";
        ps_alive.Play();

        //Set randomParticlePosition for next impact
        RandomParticlePosition();
    }

    private void RandomParticlePosition() {
        int randomInt = Random.Range(-60, 60);
        var sh1 = ps_disappear_1.shape;
        var sh2 = ps_disappear_2.shape;
        sh1.rotation = new Vector3(0, 0, randomInt);
        sh2.rotation = new Vector3(0, 0, randomInt + 60);
    }
}
