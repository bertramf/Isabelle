using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

    public bool randomXflip;

	private void Start () {
        //If randomXflip == true, set random xScale : 50% chance for x=1 or x=-1
        if (randomXflip) {
            float xScale = 1;
            int randomXScale = Random.Range(0, 2);
            if (randomXScale == 1) {
                xScale = 1;
            }
            else {
                xScale = -1;
            }
            transform.localScale = new Vector3(xScale, 1, 0);
        }
	}

}
