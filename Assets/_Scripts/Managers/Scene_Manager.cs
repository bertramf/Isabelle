using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour {

    public static Scene_Manager Instance;
    
	private void Awake () {
        Instance = this;
	}

}
