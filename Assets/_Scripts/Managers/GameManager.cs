using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    [Header("Public Values")]
    public float fadeInTime = 0.25f;
    public float fadeBlackTime = 0.5f;
    public float fadeOutTime = 0.25f;

    [Header("Debug Values")]
    public string currentGameplayScene;

    private Vector3 currentCheckpoint = Vector3.zero;

    public Vector3 CurrentCheckpoint {
        get {
            return currentCheckpoint;
        }
        set {
            currentCheckpoint = value;
        }
    }

    private void Awake() {
        Instance = this;
    }

    public IEnumerator Death() {
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_In(fadeInTime));

        yield return new WaitForSeconds(fadeInTime);

        //Unload old gameplay scene
        if (SceneManager.GetSceneByName(currentGameplayScene).isLoaded) {
            SceneManager.UnloadSceneAsync(currentGameplayScene);
        }

        //Moet hier niet een waituntil?!
        //yield return new WaitForSeconds(2f);

        //Load gameplay scene
        SceneManager.LoadSceneAsync(currentGameplayScene, LoadSceneMode.Additive);

        //yield return new WaitForSeconds(fadeBlackTime);

        //When gameplay scene is loaded
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(fadeOutTime));
    }

    

}
