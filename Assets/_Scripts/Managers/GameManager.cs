using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    [Header("Fill all scenes in here!")]
    public SceneField[] scenes;
    
    public string newSceneName {
        get {
            SceneField newSceneField = scenes[sceneIndex]; 
            return newSceneField;
        }
    }

    //dit werkt voor geen meter -> Scene wordt altijd GameManager!
    private Scene newScene {
        get {
            Scene[] allScenes = SceneManager.GetAllScenes();
            for(int i = 0; i < allScenes.Length; i++) {
                if (newSceneName.Contains(allScenes[i].name)) {
                    return allScenes[i];
                }
            }
            return allScenes[0];
        }
    }

    private string currentSceneName;
    private Scene currentScene;

    [Header("Public Testjes")]
    public string newSceneNameString;
    public string newSceneString;
    public int sceneIndex;

    private void Awake() {
        Instance = this;
        BlackScreenManager.Instance.SetCanvasAlpha(0f);
        sceneIndex = 0;

        StartCoroutine(FirstLoadingLoop());
    }

    private void Update() {
        newSceneNameString = newSceneName;
        newSceneString = newScene.name;
    }

    private IEnumerator FirstLoadingLoop() {
        gameState = GameStates.loading;

        #if UNITY_EDITOR
        yield return new WaitForSeconds(0.1f); //This is UGLY but necessary for Editor Testing!
        #endif

        //If scene is NOT loaded -> load scene
        if (!newScene.isLoaded) {
            SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            while (!newScene.isLoaded) {
                yield return null;
            }
        }

        //If scene is loaded
        SceneManager.SetActiveScene(newScene);
        InstatiateCore();
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
        yield return new WaitForSeconds(BlackScreenManager.Instance.fadeOutTime);

        currentSceneName = newSceneName;
        currentScene = newScene;
        gameState = GameStates.playing;
    }

    public void DEV_ChangeSceneIndex(int indexChange) {
        sceneIndex += indexChange;
        sceneIndex = Mathf.Clamp(sceneIndex, 0, (scenes.Length - 1));
        CheckForSceneChange(true, 0f);
    }

    public void CheckForSceneChange(bool devModeChange, float delay) {
        if (gameState == GameStates.loading) {
            return;
        }
        if (devModeChange) {
            if (currentSceneName != newSceneName) {
                StartCoroutine(ChanceScene(delay));
            }
        }
        else {
            StartCoroutine(ChanceScene(delay));
        }
    }

    private IEnumerator ChanceScene(float delayBeforeDeath) {
        gameState = GameStates.loading;

        //Wait for some time if player dies to Death_Box
        yield return new WaitForSeconds(delayBeforeDeath);

        //Load blackScreen
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_In(BlackScreenManager.Instance.fadeInTime));

        //Wait for blackScreen fadeInTime
        yield return new WaitForSeconds(BlackScreenManager.Instance.fadeInTime);

        //Unload current gameplay scene if loaded
        if (currentScene.isLoaded) {
            SceneManager.UnloadSceneAsync(currentSceneName);
        }

        //Wait for blackScreen fadeBlackTime
        yield return new WaitForSeconds(BlackScreenManager.Instance.fadeBlackTime);

        //Load gameplay scene
        SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

        //Wait for gameplay scene to be loaded
        while (!newScene.isLoaded) {
            yield return null;
        }

        //1. Set gameplay scene active 2. Unload blackScreen 3. Instatiate Player & Camera 4. Set currentSceneName to newSceneName
        SceneManager.SetActiveScene(newScene);
        StartCoroutine(BlackScreenManager.Instance.FadeBlackScreen_Out(BlackScreenManager.Instance.fadeOutTime));
        InstatiateCore();

        //Wait for blackScreen fadeOutTime
        yield return new WaitForSeconds(BlackScreenManager.Instance.fadeOutTime);
        
        currentSceneName = newSceneName;
        currentScene = newScene;
        gameState = GameStates.playing;
    }

    private void InstatiateCore() {
        if (GameObject.FindGameObjectWithTag("CameraParent") != null) {
            GameObject camera = GameObject.FindGameObjectWithTag("CameraParent");
            camera.transform.position = currentCheckpoint;
        }
        if (GameObject.FindGameObjectWithTag("Player") != null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = currentCheckpoint;
            PlayerBase playerBase = player.GetComponent<PlayerBase>();
            playerBase.PlayerStartLogic();
        }
        
        if (GameObject.FindGameObjectWithTag("CameraParent") == null) {
            Instantiate(cameraObj, currentCheckpoint, Quaternion.identity); //Positie x zou eigenlijk + cameraController.xOffset moeten zijn
        }
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            Instantiate(playerObj, currentCheckpoint, Quaternion.identity);
        }
    }
}
