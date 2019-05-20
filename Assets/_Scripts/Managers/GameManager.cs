using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public enum GameStates {
    playing,
    loading,
    mainMenu
}

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;
    public GameStates gameState;

    public GameObject playerObj;
    public GameObject cameraObj;
    [HideInInspector] public Vector3 currentCheckpoint = Vector3.zero; //Public for referencing

    [Serializable]
    public class DepthSceneArray {
        [SerializeField] private List<Object> horizontalScenes; 
        [Tooltip("Default scene; if empty this is filled in with first index of the horizontalScenes list")]
        [SerializeField] private Object defaultScene;
        private int horizontalSceneIndex;

        public string selectedHorizontalSceneName {
            get {
                return horizontalScenes[horizontalSceneIndex].name;
            }
        }

        public void InitializeHorizontalIndex() {
            if (defaultScene == null) {
                horizontalSceneIndex = 0;
            }
            else {
                horizontalSceneIndex = horizontalScenes.IndexOf(defaultScene);
            }
        }

        public void TrySelectScene(Object scene, out bool success) {
            success = false;
            if (horizontalScenes.Contains(scene)) {
                horizontalSceneIndex = horizontalScenes.IndexOf(scene);
                success = true;
            }
        }

        public void DEV_ChangeHorizontalSceneIndex(int indexChange) {
            horizontalSceneIndex += indexChange;
            horizontalSceneIndex = Mathf.Clamp(horizontalSceneIndex, 0, (horizontalScenes.Count - 1));
            GameManager.Instance.CheckForSceneChange(true, 0f);
        }
    }

    [Header("Drag all scenes here!")]
    [SerializeField] private Object defaultStartScene;
    public DepthSceneArray[] depthSceneLevels;

    private string currentSceneName;
    public int depthSceneIndex { get; private set; }
    public string newSceneName {
        get {
            DepthSceneArray currentDepthSceneLevel = depthSceneLevels[depthSceneIndex];
            return currentDepthSceneLevel.selectedHorizontalSceneName;
        }
    }

    private void Awake() {
        Instance = this;

        depthSceneIndex = 0;
        foreach (DepthSceneArray depthSceneArray in depthSceneLevels) {
            depthSceneArray.InitializeHorizontalIndex();
        }
        #if UNITY_EDITOR
            OverrideStartIndexes();
        #endif

        BlackScreenManager.Instance.SetCanvasAlpha(1f);
        StartCoroutine(FirstSceneLoadingLogic());
    }

    private void OverrideStartIndexes() {
        if(defaultStartScene == null) { return; }
        for (int i = 0; i < depthSceneLevels.Length; i++) {
            bool defaultStartSceneFound = false;
            depthSceneLevels[i].TrySelectScene(defaultStartScene, out defaultStartSceneFound);
            if (defaultStartSceneFound) {
                depthSceneIndex = i;
                return;
            }
        }
    }

    private IEnumerator Loop() {
        bool a = true;
        if (a) {
            while (!SceneManager.GetSceneByName(newSceneName).isLoaded) {
                yield return null;
            }
        }
        //Do Stuff
    }

    private IEnumerator FirstSceneLoadingLogic() {
        gameState = GameStates.loading;
        yield return new WaitForSeconds(0.1f); //Dit is LELIJK!
        if (!SceneManager.GetSceneByName(newSceneName).isLoaded) {
            SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            while (!SceneManager.GetSceneByName(newSceneName).isLoaded) {
                yield return null;
            }
            Scene scene = SceneManager.GetSceneByName(newSceneName);
            SceneManager.SetActiveScene(scene);
            InstatiateCore();
            StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeOutTime);
            gameState = GameStates.playing;
            currentSceneName = newSceneName;
        }
        else {
            Scene scene = SceneManager.GetSceneByName(newSceneName);
            SceneManager.SetActiveScene(scene);
            InstatiateCore();
            StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeOutTime);
            gameState = GameStates.playing;
            currentSceneName = newSceneName;
        }
    }

    public void DEV_ChangeDepthSceneIndex(int indexChange) {
        depthSceneIndex += indexChange;
        depthSceneIndex = Mathf.Clamp(depthSceneIndex, 0, (depthSceneLevels.Length - 1));
        CheckForSceneChange(true, 0f);
    }

    public void CheckForSceneChange(bool devModeChange, float delay) {
        if (gameState == GameStates.loading) {
            return;
        }
        if (devModeChange) {
            if(currentSceneName != newSceneName) {
                StartCoroutine(ChanceScene(delay));
            }
        }
        else {
            StartCoroutine(ChanceScene(delay));
        }
    }

    private IEnumerator ChanceScene(float delayBeforeDeath) {
        //GameState = loading
        gameState = GameStates.loading;

            //Wait for some time if player dies to Death_Box
            yield return new WaitForSeconds(delayBeforeDeath);

        //Load blackScreen
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_In(BlackScreenManager.Instance.fadeInTime));

            //Wait for blackScreen fadeInTime
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeInTime);
        
        //Unload current gameplay scene if loaded
        if (SceneManager.GetSceneByName(currentSceneName).isLoaded) {
            SceneManager.UnloadSceneAsync(currentSceneName);
        }
        
            //Wait for blackScreen fadeBlackTime
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeBlackTime);

        //Load gameplay scene
        SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

            //Wait for gameplay scene to be loaded
            while (!SceneManager.GetSceneByName(newSceneName).isLoaded) {
                yield return null;
            }

        //1. Set gameplay scene active 2. Unload blackScreen 3. Instatiate Player & Camera 4. Set currentSceneName to newSceneName
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
        InstatiateCore();
        currentSceneName = newSceneName;

            //Wait for blackScreen fadeOutTime
            yield return new WaitForSeconds(BlackScreenManager.Instance.fadeOutTime);

        //GameState = playing
        gameState = GameStates.playing;
    }

    private void InstatiateCore() {
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            Instantiate(playerObj, currentCheckpoint, Quaternion.identity);
        }
        if (GameObject.FindGameObjectWithTag("CameraParent") == null) {
            Instantiate(cameraObj, currentCheckpoint, Quaternion.identity); //Positie x zou eigenlijk + cameraController.xOffset moeten zijn
        }
    }

}
