using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorSwapConfigurator))]
public class ColorSwapConfiguratorEditor : Editor {

	private const int TEXTURE_HEIGHT = 300;
	private const int TEXTURE_SPACING = 10;

	private ColorSwapConfigurator targetScript;

	private ColorSwapPreset selectedPreset{ 
		get { 
			return targetScript.currentPreset; 
		}
		set {
			targetScript.currentPreset = value;	
		} 
	}

	[SerializeField]private bool showPreviewTextures;
	private string newPresetName = "preset";

	private void OnEnable(){
		targetScript = (ColorSwapConfigurator)target;

		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets ();
	}

	public override void OnInspectorGUI () {
		serializedObject.Update (); 

		string titleText = selectedPreset == null ? "No preset selected..." : "Editing preset: " + selectedPreset.name;
		GUILayout.Label (titleText, EditorStyles.boldLabel);
		showPreviewTextures = EditorGUILayout.Toggle ("Show preview textures", showPreviewTextures);
		if (showPreviewTextures) {
			DrawTextures ();
		}

		DrawPresetColors ();

		if (selectedPreset != null) {
			if (GUILayout.Button ("Update scene view")) {
				EditorUtility.SetDirty(selectedPreset);
				AssetDatabase.SaveAssets ();
			}
		}

		GUILayout.Space (18);

		GUILayout.BeginHorizontal ();
		newPresetName = GUILayout.TextField (newPresetName);
		if(GUILayout.Button("Add '" + newPresetName + "'")){
			CreateNewPreset ();
		}
		GUILayout.EndHorizontal ();

		serializedObject.ApplyModifiedProperties ();
	}

	private void DrawTextures(){
		float startY = GUILayoutUtility.GetLastRect ().y + 18;
		float textureWidth = EditorGUIUtility.currentViewWidth / 2 - TEXTURE_SPACING / 2f;
		Rect screenRect = new Rect (0, startY, textureWidth, TEXTURE_HEIGHT);

		EditorGUI.DrawPreviewTexture (screenRect, targetScript.originalTexture, targetScript.defaultMaterial, ScaleMode.ScaleToFit);
		screenRect.x += textureWidth + TEXTURE_SPACING;
		EditorGUI.DrawPreviewTexture (screenRect, targetScript.presetTexture, targetScript.presetMaterial, ScaleMode.ScaleToFit);
		GUILayout.Space (TEXTURE_HEIGHT + 36); 
	}

	private void DrawPresetColors(){
		selectedPreset = EditorGUILayout.ObjectField ("Selected preset", selectedPreset, typeof(ColorSwapPreset), false) as ColorSwapPreset;

		if (selectedPreset != null) {
			Rect originalColorRect = GUILayoutUtility.GetLastRect ();
			originalColorRect.width = 40;

			Color guiColor = GUI.color;

			for (int i = 0; i < selectedPreset.colorVariants.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUI.color = targetScript.sourceColors [i];
				originalColorRect.y += 18;
				GUI.DrawTexture (originalColorRect, EditorGUIUtility.whiteTexture);

				GUILayout.Space(50);
				EditorGUILayout.LabelField (i.ToString(), GUILayout.Width (30));

				selectedPreset.colorVariants [i] = EditorGUILayout.ColorField (selectedPreset.colorVariants [i]);
				EditorGUILayout.EndHorizontal ();
			}

			GUI.color = guiColor;

			targetScript.UpdateMaterial (selectedPreset);
			SceneView.RepaintAll ();
		}
	}

	private void CreateNewPreset() {
		string targetScriptPath = AssetDatabase.GetAssetPath (target);
		targetScriptPath = targetScriptPath.Remove(targetScriptPath.Length - targetScript.name.Length - 7);
		string folderPath = targetScriptPath + "/presets";
		bool folderExists = AssetDatabase.IsValidFolder (folderPath);
		if (!folderExists) {
			AssetDatabase.CreateFolder (targetScriptPath, "presets");
		}

		string presetPath = folderPath + "/" + newPresetName + ".asset";

		ColorSwapPreset newPreset = ScriptableObject.CreateInstance<ColorSwapPreset> ();
		AssetDatabase.CreateAsset (newPreset, presetPath);
		newPreset.Initialize (targetScript, targetScript.presetMaterial, targetScript.sourceColors);

		targetScript.currentPreset = newPreset;
	}

	private void OnDisable(){
		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets ();		
	}

}