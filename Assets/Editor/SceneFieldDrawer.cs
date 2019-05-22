using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, GUIContent.none, property);

		SerializedProperty sceneProperty = property.FindPropertyRelative("scene");
		SerializedProperty nameProperty = property.FindPropertyRelative("name");
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		if(sceneProperty != null) {
			EditorGUI.BeginChangeCheck();
			Object sceneValue = EditorGUI.ObjectField(position, sceneProperty.objectReferenceValue, typeof(SceneAsset), false);
			if (EditorGUI.EndChangeCheck()) {
				sceneProperty.objectReferenceValue = sceneValue;
				if(sceneValue != null) {
					string scenePath = AssetDatabase.GetAssetPath(sceneValue);
					int assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
					int extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
					nameProperty.stringValue = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
				}
			}
		}

		EditorGUI.EndProperty();
	}

}