using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRechargeOrb : MonoBehaviour {

    private PlayerBase playerBase;
    private CircleCollider2D coll;
    private SpriteRenderer sprite;

    public Sprite chargedSpr;
    public Sprite deChargedSpr;

    [Header("Gameplay Values")]
    public float rechargeTime = 2f;
    public float playerFreezeTime = 0.2f;

    private void Start() {
        playerBase = GameObject.Find("Player").GetComponent<PlayerBase>();
        coll = GetComponent<CircleCollider2D>();
        sprite = transform.Find("DashRecharge_Visuals").GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            StartCoroutine(RechargeLogic());
        }
    }

    private IEnumerator RechargeLogic() {
        Disappear();
        yield return new WaitForSeconds(rechargeTime);
        Appear();
    }

    private void Disappear() {
        playerBase.CanDash = true;
        StartCoroutine(PlayerFreeze.instance.FreezePlayer(playerFreezeTime));
        StartCoroutine(Screenshake.instance.ShakeHorizontal(2, playerFreezeTime * 2, 0.07f));
        coll.enabled = false;
        sprite.sprite = deChargedSpr;
        sprite.sortingLayerName = "Default";
    }

    private void Appear() {
        coll.enabled = true;
        sprite.sprite = chargedSpr;
        sprite.sortingLayerName = "Gameplay";
    }
}
