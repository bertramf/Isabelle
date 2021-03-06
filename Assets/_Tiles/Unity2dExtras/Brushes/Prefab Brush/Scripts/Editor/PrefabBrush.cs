using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
    [CreateAssetMenu(fileName = "Prefab brush", menuName = "Brushes/Prefab brush")]
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : GridBrush{

        private const float k_PerlinOffset = 100000f;
        private GameObject prev_brushTarget;
        private Vector3Int prev_position;

        //Don't use these fields
        [HideInInspector()]
        public GameObject[] m_Prefabs;
        [HideInInspector()]
        public int m_Z;

        //Custom randomness
        [System.Serializable]
        public class PrefabClass {
            public GameObject obj;
            public float priority;
            [HideInInspector()]
            public float chanceValue;
        }
        public PrefabClass[] allPrefabs;
        private GameObject currentPrefab;

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position){
            if (position == prev_position){
                        return;
                    }
                    prev_position = position;
            if (brushTarget) {
                prev_brushTarget = brushTarget;
            }
            brushTarget = prev_brushTarget;

            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            //Old perlin stuff
            //int perlinValue = Mathf.FloorToInt(GetPerlinValue(position, m_PerlinScale, 0) * m_Prefabs.Length);
            //int index = Mathf.Clamp(perlinValue, 0, m_Prefabs.Length - 1);

            float totalSpawnPriority = 0f;
            foreach(PrefabClass pref in allPrefabs) {
                totalSpawnPriority += pref.priority;
            }

            float chanceValue = 0f;
            foreach (PrefabClass pref in allPrefabs) {
                chanceValue += pref.priority / totalSpawnPriority;
                pref.chanceValue = chanceValue;
            }

            //Custom randomness
            float randomValue = Random.value;
            foreach (PrefabClass pref in allPrefabs) {
                if(randomValue < pref.chanceValue) {
                    currentPrefab = pref.obj;
                    break;
                }
            }
            GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(currentPrefab);

            if (instance != null){
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z) + new Vector3(.5f, .5f, .5f)));
            }
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget)
                    {
                        prev_brushTarget = brushTarget;
                    }
                    brushTarget = prev_brushTarget;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Transform erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
            if (erased != null)
                Undo.DestroyObjectImmediate(erased.gameObject);
        }

        private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            int childCount = parent.childCount;
            Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
            Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
            Bounds bounds = new Bounds((max + min)*.5f, max - min);

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (bounds.Contains(child.position))
                    return child;
            }
            return null;
        }

        private static float GetPerlinValue(Vector3Int position, float scale, float offset)
        {
            return Mathf.PerlinNoise((position.x + offset)*scale, (position.y + offset)*scale);
        }
    }

    [CustomEditor(typeof(PrefabBrush))]
    public class PrefabBrushEditor : GridBrushEditor
    {
        private PrefabBrush prefabBrush { get { return target as PrefabBrush; } }

        private SerializedProperty m_Prefabs;
        private SerializedObject m_SerializedObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_SerializedObject = new SerializedObject(target);
            m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
        }

        public override void OnPaintInspectorGUI()
        {
            m_SerializedObject.UpdateIfRequiredOrScript();
            //Old perlin stuff
            //prefabBrush.m_PerlinScale = EditorGUILayout.Slider("Perlin Scale", prefabBrush.m_PerlinScale, 0.001f, 1000f);
            //prefabBrush.m_Z = EditorGUILayout.IntField("Position Z", prefabBrush.m_Z);
                
            //EditorGUILayout.PropertyField(m_Prefabs, true);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
