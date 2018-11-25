using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class BlackWhiteGeneratorWindow : EditorWindow {

	public const string COLOR_SWAP_ROOT_DIRECTORY = "Assets/Resources/Generated/ColorSwap";

	[SerializeField]private string presetName;
	[SerializeField]private Material defaultMaterial;
	[SerializeField]private Material swapMaterial;

	[SerializeField]private Sprite selectedSprite;
	[SerializeField]private Texture2D generatedTexture;
	[SerializeField]private int generatedTextureColorSteps;
	[SerializeField]private Color32[] colorVariants;

	[SerializeField]private int ignoreAlphaThreshold = 255;
	 
	[MenuItem("Shepherd/Black White Generator")]
	private static void Initialize(){
		BlackWhiteGeneratorWindow window = (BlackWhiteGeneratorWindow)EditorWindow.GetWindow<BlackWhiteGeneratorWindow> ();
		window.Show ();
	}

	private void OnGUI(){
		GUILayout.Label ("Standard settings:", EditorStyles.boldLabel);
		defaultMaterial = EditorGUILayout.ObjectField ("Default material", defaultMaterial, typeof(Material), false) as Material;
		swapMaterial = EditorGUILayout.ObjectField ("Swap material", swapMaterial, typeof(Material), false) as Material;
		ignoreAlphaThreshold = EditorGUILayout.IntSlider ("Ignore alpha threshold", ignoreAlphaThreshold, 0, 255);

		GUILayout.Label ("Custom settings:", EditorStyles.boldLabel);
		presetName = EditorGUILayout.TextField ("Preset name", presetName);
		Sprite newSelectedSprite = EditorGUILayout.ObjectField ("Original", selectedSprite, typeof(Sprite), false) as Sprite;
		if (newSelectedSprite != selectedSprite) {
			selectedSprite = newSelectedSprite;
			presetName = selectedSprite.name;
			if (generatedTexture != null) {
				DestroyImmediate (generatedTexture);			
			}
		}

		if (selectedSprite == null) {
			GUILayout.Label ("Please select a sprite in order to be able to convert it to a black-white image");
			return;
		}
		else if (generatedTexture == null) {
			if (GUILayout.Button ("Convert to black white map")) {
				ConvertSpriteToBlackWhiteMap ();
			}
		}
		else {
			GUILayout.Label ("Result:", EditorStyles.boldLabel);
			GUILayout.Label (generatedTexture);
			GUILayout.Label (generatedTextureColorSteps + " customizeable colors");

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Save")) {
				CreateColorSwapConfigurator ();
			}
			else if (GUILayout.Button ("Discard")) {
				DestroyImmediate (generatedTexture);
			}
			GUILayout.EndHorizontal ();
		}
	}

	private void ConvertSpriteToBlackWhiteMap(){
		double startTime = EditorApplication.timeSinceStartup;

		Texture2D existingTexture = selectedSprite.texture;
		generatedTexture = new Texture2D (existingTexture.width, existingTexture.height, existingTexture.format, false);
		Graphics.CopyTexture (existingTexture, generatedTexture);

		Color32[] pixels = generatedTexture.GetPixels32 ();
		List<Color32> colorVariants = new List<Color32> ();

		int pixelCount = pixels.Length;
		generatedTextureColorSteps = 0;

		// Analyze texture
		for (int i = 0; i < pixelCount; i ++) {
			bool stopProcess = EditorUtility.DisplayCancelableProgressBar ("Analyzing texture", "Analyzing " + pixelCount + " pixels", (float)i / pixelCount);
			if (stopProcess) {
				EditorUtility.ClearProgressBar ();
				generatedTexture = null;
				return;
			}

			if (pixels[i].a < ignoreAlphaThreshold) {
				continue;
			}

			bool newColor = !ColorExists (colorVariants, pixels [i]);
			if (newColor) {
				generatedTextureColorSteps++;
				colorVariants.Add (pixels[i]);
			}
		}

		// Fill in new texture
		for (int i = 0; i < pixelCount; i++) {
			bool stopProcess = EditorUtility.DisplayCancelableProgressBar ("Creating texture", "Creating " + pixelCount + " pixels", (float)i / pixelCount);
			if (stopProcess) {
				EditorUtility.ClearProgressBar ();
				generatedTexture = null;
				return;
			}

			if (pixels[i].a < ignoreAlphaThreshold) {
				continue;
			}

			int colorIndex = colorVariants.IndexOf (pixels[i]);
			int colorValue = Mathf.FloorToInt((colorIndex * 255) / generatedTextureColorSteps);
			pixels[i] = new Color32 (Convert.ToByte(colorValue), Convert.ToByte(colorValue), Convert.ToByte(colorValue), 255);
		}

		// Clear progress bar
		EditorUtility.ClearProgressBar ();

		// Apply new data
		this.colorVariants = colorVariants.ToArray ();
		generatedTexture.SetPixels32 (pixels);
		generatedTexture.Apply (false);

		// Log process duration
		double duration = EditorApplication.timeSinceStartup - startTime;
		Debug.Log ("Finished converting in " + duration.ToString () + " seconds");
	}

	private bool ColorExists(List<Color32> colorVariants, Color32 colorToCompare){
		foreach (Color32 color in colorVariants) {
			if (color.Equals (colorToCompare)) {
				return true;
			}
		}
		return false;
	}

	/// <returns> The created preset material's path </returns>
	private string CreatePresetMaterial(string resourcePath){
		string materialDestinationPath = resourcePath + "/" + presetName + ".mat";
		Material presetMaterial = AssetDatabase.LoadAssetAtPath (materialDestinationPath, typeof(Material)) as Material;

		if (presetMaterial == null) {
			string materialSourcePath = AssetDatabase.GetAssetPath (swapMaterial);
			FileUtil.CopyFileOrDirectory (materialSourcePath, materialDestinationPath);
			presetMaterial = AssetDatabase.LoadAssetAtPath (materialDestinationPath, typeof(Material)) as Material;
		}
		else {
			presetMaterial.CopyPropertiesFromMaterial (swapMaterial);
		}

		return materialDestinationPath;
	}

	private void CopyTextureImportSettings(string sourcePath, string destinationPath){
		TextureImporter originalImporter = AssetImporter.GetAtPath (sourcePath) as TextureImporter;
		TextureImporter newImporter = AssetImporter.GetAtPath (destinationPath) as TextureImporter;
		EditorUtility.CopySerialized (originalImporter, newImporter);

		List<SpriteMetaData> newImporterData = new List<SpriteMetaData> ();
		for (int i = 0; i < originalImporter.spritesheet.Length; i++) {
			SpriteMetaData spriteData = originalImporter.spritesheet [i];
			newImporterData.Add (spriteData);
		}
		newImporter.spritesheet = newImporterData.ToArray ();

		AssetDatabase.ImportAsset (destinationPath, ImportAssetOptions.ForceUpdate);
	}

	private void CreateColorSwapConfigurator(){
		string resourcePath = COLOR_SWAP_ROOT_DIRECTORY + "/" + presetName;
		if (AssetDatabase.IsValidFolder (resourcePath)) {
			Debug.LogWarning (resourcePath + " already exists! Overwriting file data...");// TODO: request if the developer is sure to overwrite existing data
		}
		else {
			AssetDatabase.CreateFolder (COLOR_SWAP_ROOT_DIRECTORY, presetName);
		}

		// Create Texture:
		byte[] textureBytes = generatedTexture.EncodeToPNG ();
		string blackWhiteTexturePath = resourcePath + "/" + presetName + "_texture.png";
		File.WriteAllBytes (blackWhiteTexturePath, textureBytes);

		// Create Material:
		string materialPath = CreatePresetMaterial (resourcePath);

		// Delay the using of this texture and material since the editor does not recognize it yet
		EditorApplication.delayCall = () => {
			AssetDatabase.Refresh ();

			// Get references to the created assets
			Texture2D createdTexture = AssetDatabase.LoadAssetAtPath<Texture2D> (blackWhiteTexturePath);
			Material presetMaterial = AssetDatabase.LoadAssetAtPath (materialPath, typeof(Material)) as Material;

			// Create Texture:
			string sourceTexturePath = AssetDatabase.GetAssetPath (selectedSprite);
			CopyTextureImportSettings (sourceTexturePath, blackWhiteTexturePath);

			// Create ColorSwapPreset if it does not yet exist
			string colorSwapPresetPath = resourcePath + "/" + presetName + ".asset";
			ColorSwapConfigurator colorSwapHolder = AssetDatabase.LoadAssetAtPath (colorSwapPresetPath, typeof(ColorSwapConfigurator)) as ColorSwapConfigurator;
			if(colorSwapHolder == null){
				colorSwapHolder = ScriptableObject.CreateInstance<ColorSwapConfigurator> ();
				AssetDatabase.CreateAsset (colorSwapHolder, colorSwapPresetPath);
			}
			colorSwapHolder.Initialize (colorVariants, selectedSprite.texture, createdTexture, defaultMaterial, presetMaterial);

			AssetDatabase.Refresh ();
			EditorGUIUtility.PingObject (colorSwapHolder);
		};
	}

}