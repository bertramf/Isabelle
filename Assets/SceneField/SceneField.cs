using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SceneField {

	public string SceneName { get { return name; } }

	[SerializeField] private string name;
#if UNITY_EDITOR
	[SerializeField] private Object scene;
#endif

	public static implicit operator string(SceneField sceneField) {
		return sceneField.name;
	}

}