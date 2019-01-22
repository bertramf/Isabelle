using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour{

    private Animator anim;

    public float idleTime = 0.5f;

    public ParticleSystem vfxSplash;
    public GameObject bulletObj;
    public float bulletSpeed;

    private void Start() {
        anim = GetComponent<Animator>();

        StartCoroutine(TurretLoop());
    }

    private IEnumerator TurretLoop() {
        while (true) {
            yield return new WaitForSeconds(idleTime);
            StartCoroutine(ShootBullet());
        }
    }

    private IEnumerator ShootBullet() {
        anim.SetTrigger("shoot");

        yield return new WaitForSeconds(0.12f);

        if(vfxSplash != null) {
            vfxSplash.Play();
        }

        GameObject bullet;
        bullet = Instantiate(bulletObj, transform.position, Quaternion.identity);

        Rigidbody2D bulletRb;
        bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = new Vector3(0f, bulletSpeed);
    }



}
