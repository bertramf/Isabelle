using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUp_Visuals : MonoBehaviour {

    private BlockUp_Base blockUpBase;
    private Animator anim;

    private void Start () {
        blockUpBase = transform.parent.GetComponent<BlockUp_Base>();
        anim = GetComponent<Animator>();
    }
}
