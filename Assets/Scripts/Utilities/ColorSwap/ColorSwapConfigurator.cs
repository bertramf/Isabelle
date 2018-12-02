using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ColorSwapConfigurator : ScriptableObject {

	public Color[] sourceColors;
	public List<ColorSwapPreset> colorPresets;
	public ColorSwapPreset currentPreset;

	public Texture2D originalTexture;
	public Material defaultMaterial;

	public Material presetMaterial;
	public Texture2D presetTexture;

	public string texturePath;

	public void Initialize(Color32[] sourceColors, Texture2D originalTexture, Texture2D presetTexture, Material defaultMaterial, Material presetMaterial){
		this.colorPresets = new List<ColorSwapPreset> ();
		this.sourceColors = new Color[sourceColors.Length];
		for(int i = 0; i < sourceColors.Length; i ++) {
			this.sourceColors [i] = sourceColors [i];
		}

		this.originalTexture = originalTexture;
		this.presetTexture = presetTexture;
		this.defaultMaterial = defaultMaterial;
		this.presetMaterial = presetMaterial;

		#if UNITY_EDITOR
		texturePath = AssetDatabase.GetAssetPath(presetTexture);
		string[] stringSeparator = new string[] {"Resources/", "."};
		texturePath = texturePath.Split(stringSeparator, System.StringSplitOptions.None)[1];
		if(string.IsNullOrEmpty(texturePath)){
			Debug.LogWarning("Failed to find the correct path to the preset texture...", presetTexture);
		}
		#endif
	}

	public void AddPreset(ColorSwapPreset preset){
		colorPresets.Add (preset);
		currentPreset = preset;
	}

	public void UpdateMaterial(ColorSwapPreset preset){
		presetMaterial.SetInt ("_Range", preset.colorVariants.Length);
		presetMaterial.SetColorArray ("_ColorMatrix", preset.colorVariants);		
	}

}