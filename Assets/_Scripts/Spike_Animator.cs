using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Animator : MonoBehaviour {

    private Animator anim;
    private int randomId;
    private float randomWaitingTime;

    public float minTime;
    public float maxTime;
    public float minTimeShort;
    public float maxTimeShort;

	private void Start () {
        anim = GetComponent<Animator>();

        randomId = Random.Range(1, 6);
        randomWaitingTime = Random.Range(minTime, maxTime);

        StartCoroutine(PlayRandomAnim(randomId, randomWaitingTime));
	}

    private IEnumerator PlayRandomAnim(int id, float waitingTime) {
        string idString = id.ToString();
        anim.SetTrigger(idString);

        yield return new WaitForSeconds(waitingTime);

        randomId = Random.Range(1, 6);
        int randomInt = Random.Range(0, 5);
        if (randomInt == 0) {
            randomWaitingTime = Random.Range(minTimeShort, maxTimeShort);
        }
        else {
            randomWaitingTime = Random.Range(minTime, maxTime);
        }

        StartCoroutine(PlayRandomAnim(randomId, randomWaitingTime));
    }

}
