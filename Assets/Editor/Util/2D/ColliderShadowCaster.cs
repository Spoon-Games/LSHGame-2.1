using LSHGame.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(ColliderShadowCaster))]
    public class ColliderShadowCasterEditor : UnityEditor.Editor
    {
        private ColliderShadowCaster child;
        private ColliderShadowCaster Caster { get
            {
                if (child == null)
                    child = target as ColliderShadowCaster;
                return child;
            } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                Generate();
            }
        }

        private void Generate()
        {
            if (Caster.compositeCollider == null)
            {
                Debug.Log("You have to asign a CompositeCollider2D");
                return;
            }

            for (int casterIndex = 0; casterIndex < Caster.shadowCasters.Count || casterIndex < Caster.compositeCollider.pathCount; casterIndex++)
            {
                if(casterIndex >= Caster.shadowCasters.Count)
                {
                    GameObject go = new GameObject("Shadow Caster " + casterIndex);
                    go.transform.SetParent(Caster.transform);

                    Caster.shadowCasters.Add(go.AddComponent<ShadowCaster2D>());
                }

                ShadowCaster2D shadowCaster = Caster.shadowCasters[casterIndex];

                if (casterIndex < Caster.compositeCollider.pathCount)
                {

                    SerializedObject shadowO = new SerializedObject(shadowCaster);
                    shadowO.Update();
                    SerializedProperty shapePathProp = shadowO.FindProperty("m_ShapePath");

                    Vector2[] points = new Vector2[Caster.compositeCollider.GetPathPointCount(casterIndex)];
                    Caster.compositeCollider.GetPath(casterIndex, points);

                    shapePathProp.ClearArray();

                    for (int i = points.Length - 1; i >= 0; i--)
                    {
                        shapePathProp.InsertArrayElementAtIndex(0);
                        shapePathProp.GetArrayElementAtIndex(0).vector3Value = points[i];
                    }

                    shadowO.ApplyModifiedProperties();
                }
                else
                {
                    Caster.shadowCasters.RemoveAt(casterIndex);
                    DestroyImmediate(shadowCaster.gameObject);
                }

            }
        }
    }

}
