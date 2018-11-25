using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class ColorSwap : MonoBehaviour {

	[SerializeField]private ColorSwapPreset colorSwapPreset;

	private Material spriteMaterial;

	private void Start () {
		UpdateVisualData ();
	}

	#if UNITY_EDITOR
	private void Update () {
		if (!Application.isPlaying && colorSwapPreset != null) {
			spriteMaterial.SetVectorArray ("_ColorMatrix", ColorMatrix ());
		}
	} 
	#endif

	private Vector4[] ColorMatrix(){
		Color[] colors = colorSwapPreset.colorVariants;
		Vector4[] matrix = new Vector4[colors.Length];
		for (int i = 0; i < colors.Length; i++) {
			matrix[i] = ColorToVec(colors[i]); 
		}

		return matrix;
	}

	private Vector4 ColorToVec(Color c){
		return new Vector4 (c.r, c.g, c.b, c.a);
	}

	public void UpdateVisualData(){
		if (colorSwapPreset == null) {
			Debug.LogWarning ("Can't update visual data for " + name + ", since there is no preset available", transform);
			return;
		}

		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		renderer.material = spriteMaterial = new Material(colorSwapPreset.PresetMaterial);
		spriteMaterial.SetVectorArray ("_ColorMatrix", ColorMatrix ());

        if (!Application.isPlaying) {
            renderer.sprite = Resources.LoadAll<Sprite>(colorSwapPreset.TexturePath)[0];
        }
	}

    public void UpdateVisualData(ColorSwapPreset newPreset) {
        colorSwapPreset = newPreset;
        UpdateVisualData();
    }

}