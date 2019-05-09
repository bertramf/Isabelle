using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
    [CreateAssetMenu(fileName = "Prefab brush", menuName = "Brushes/Prefab brush")]
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : GridBrush
    {
        //public float m_PerlinScale = 0.5f;
        private const float k_PerlinOffset = 100000f;
        public GameObject[] m_Prefabs;
        public float[] prefab_randomness; //index is linked aan index van m_Prefabs; so must be same value!
        public float[] randomness_margins; //index is linked aan index van m_Prefabs; so must be same value!

        public int m_Z;
        private GameObject prev_brushTarget;
        private Vector3Int prev_position;

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (position == prev_position)
                    {
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

            //Custom randomness
            float oldMargin = 0f;
            for (int i = 0; i < randomness_margins.Length; i++) {
                randomness_margins[i] = oldMargin + prefab_randomness[i];
                oldMargin = randomness_margins[i];
            }
            float totalMargin = 0f;
            for (int i = 0; i < randomness_margins.Length; i++) {
                if(totalMargin < randomness_margins[i]) {
                    totalMargin = randomness_margins[i];
                }   
            }
            float randomIndex = Random.Range(0f, totalMargin);
            GameObject prefab = m_Prefabs[0];
            for (int i = 0; i < m_Prefabs.Length; i++) {
                if(i == 0) {
                    if (randomIndex > 0 && randomIndex < randomness_margins[i]) {
                        prefab = m_Prefabs[i];
                    }
                }
                else {
                    if (randomIndex > randomness_margins[i - 1] && randomIndex < randomness_margins[i]) {
                        prefab = m_Prefabs[i];
                    }
                }
                   
            }
            
            GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            if (instance != null)
            {
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
            prefabBrush.m_Z = EditorGUILayout.IntField("Position Z", prefabBrush.m_Z);
                
            EditorGUILayout.PropertyField(m_Prefabs, true);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
