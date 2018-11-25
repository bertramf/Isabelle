using UnityEngine;

[System.Serializable]
public class ColorSwapPreset : ScriptableObject {

	public Color[] colorVariants;
	public Material PresetMaterial{get{ return presetMaterial; }}
	public string TexturePath{get{ return colorSwapData.texturePath; }}

	[SerializeField] private Material presetMaterial;
	[SerializeField] private ColorSwapConfigurator colorSwapData;

	public void Initialize(ColorSwapConfigurator colorSwapData, Material presetMaterial, Color[] sourceColors){
		this.colorSwapData = colorSwapData;
		this.presetMaterial = presetMaterial;
		colorVariants = new Color[sourceColors.Length];
		for(int i = 0; i < sourceColors.Length; i ++) {
			Color c = sourceColors [i];
			colorVariants[i] = new Color(c.r, c.g, c.b, c.a);
		}
	}

}