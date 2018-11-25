using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorSwap), true)]
public class ColorSwapEditor : Editor {

	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();

		if (GUILayout.Button ("Update")) {
			ColorSwap targetScript = (ColorSwap)target;
			targetScript.UpdateVisualData ();
		}
	}

}