using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    [Serializable]
    public class DepthSceneArray {
        [SerializeField] private List<Object> horizontalScenes;
        [SerializeField] private Object defaultScene;
        private int horizontalSceneIndex;

        public void Initialize() {
            if (defaultScene == null) {
                horizontalSceneIndex = 0;
            }
            else {
                //Wat is de eerste index uit de lijst met value = 'defaultscene' ?
                horizontalSceneIndex = horizontalScenes.IndexOf(defaultScene);
            }
        }

        public string selectedHorizontalSceneName {
            get {
                return horizontalScenes[horizontalSceneIndex].name;
            }
        }

        public void DEV_ChangeHorizontalSceneIndex(int indexChange) {
            horizontalSceneIndex += indexChange;
            horizontalSceneIndex = Mathf.Clamp(horizontalSceneIndex, 0, (horizontalScenes.Count - 1));
        }
    }

    public DepthSceneArray[] depthSceneLevels;
    
    public int depthSceneIndex; //Moet deze niet private zijn? :o
    private string oldSceneName;
    public DepthSceneArray currentDepthSceneLevel {
        get {
            return depthSceneLevels[depthSceneIndex];
        }
    }

    public string currentSceneName { //Deze public gemaakt vanwege aanroepen vanuit LevelSwitcher.cs
        get {
            return currentDepthSceneLevel.selectedHorizontalSceneName;
        }
    }

    public void DEV_ChangeDepthSceneIndex(int indexChange) {
        oldSceneName = currentSceneName;
        depthSceneIndex += indexChange;
        depthSceneIndex = Mathf.Clamp(depthSceneIndex, 0, (depthSceneLevels.Length - 1));
        StartCoroutine(Death(oldSceneName, 0f));
    }

    public GameObject playerObj;
    public GameObject cameraObj;
    [HideInInspector()]
    public Vector3 currentCheckpoint = Vector3.zero;

    private void Awake() {
        Instance = this;

        foreach (DepthSceneArray depthSceneArray in depthSceneLevels) {
            depthSceneArray.Initialize();
        }
        depthSceneIndex = 0;

        BlackScreenManager.Instance.SetCanvasAlpha(1f);
        StartCoroutine(StartSceneLoading());
    }

    private IEnumerator StartSceneLoading() {
        yield return new WaitForSeconds(0.1f); //Dit is LELIJK!
        if (!SceneManager.GetSceneByName(currentSceneName).isLoaded) {
            SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
            while (!SceneManager.GetSceneByName(currentSceneName).isLoaded) {
                yield return null;
            }
            Scene scene = SceneManager.GetSceneByName(currentSceneName);
            SceneManager.SetActiveScene(scene);
            InstatiateCore();
            StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
        }
        else {
            Scene scene = SceneManager.GetSceneByName(currentSceneName);
            SceneManager.SetActiveScene(scene);
            InstatiateCore();
            StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
        }
    }

    public void TriggerDeath(float delay) {
        StartCoroutine(Death(currentSceneName, delay));
    }

    private IEnumerator Death(string oldScene, float delayBeforeDeath) {
            //Wait for some time if player dies to Death_Box
            yield return new WaitForSeconds(delayBeforeDeath); 

        //Load blackScreen
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_In(BlackScreenManager.Instance.fadeInTime));

            //Wait for blackScreen fadeInTime
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeInTime);
        
        //Unload old gameplay scene
        if (SceneManager.GetSceneByName(oldScene).isLoaded) {
            SceneManager.UnloadSceneAsync(oldScene);
        }
        
            //Wait for blackScreen fadeBlackTime
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeBlackTime);

        //Load gameplay scene
        SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);

            //Wait for gameplay scene to be loaded
            while (!SceneManager.GetSceneByName(currentSceneName).isLoaded) {
                yield return null;
            }

        //1. Set gameplay scene active 2. Unload blackScreen 3. Instatiate Player & Camera
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
        InstatiateCore();

            //Wait for blackScreen fadeOutTime
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeOutTime);

    }

    private void InstatiateCore() {
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            Instantiate(playerObj, currentCheckpoint, Quaternion.identity);
        }
        if (GameObject.FindGameObjectWithTag("CameraParent") == null) {
            //Positie x zou eigenlijk + cameraController.xOffset moeten zijn
            Instantiate(cameraObj, currentCheckpoint, Quaternion.identity);
        }
    }

}
