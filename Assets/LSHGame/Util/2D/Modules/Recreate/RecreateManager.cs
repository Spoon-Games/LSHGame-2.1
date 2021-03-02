

using SceneM;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Experimental.SceneManagement;
#endif

using UnityEngine;

namespace LSHGame.Util
{
    [CreateAssetMenu(menuName ="LSHGame/Editor/RecreateManager")]
    public class RecreateManager : SceneM.ScriptableSingleton<RecreateManager>
    {
        [SerializeField]
        private RecreateModule[] serializedModules;   

        public void Recreate(RecreateModule ghost, Vector3 originPosition, Quaternion originRotation,Vector3 originScale,Transform originParent)
        {
            var vessel = serializedModules.FirstOrDefault(m => Equals(m.prefabGuid,ghost.prefabGuid)); // Maybe make it more specific
            if(vessel != null)
            {
                RecreateModule o = Instantiate(vessel, originPosition, originRotation, originParent);
                o.SetLocalScale(originScale);
            }
        }

#if UNITY_EDITOR

        private void LoadPrefabs()
        {
            List<RecreateModule> modules = new List<RecreateModule>();
            //var paths = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Substance).ToString());
            //foreach (var path in paths)
            //{
            //    substances.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<Substance>(path));
            //}

            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Debug.Log("Path: " + path);

                RecreateModule m = AssetDatabase.LoadAssetAtPath<RecreateModule>(path);
                if (m != null)
                {
                    if (string.IsNullOrEmpty(m.prefabGuid))
                        m.prefabGuid = Guid.NewGuid().ToString();
                    modules.Add(m);
                }
            }
            serializedModules = modules.ToArray();
        }

        private void OnEnable()
        {
            PrefabStage.prefabSaved += OnPrefabSaved;
        }

        private void OnPrefabSaved(GameObject prefab)
        {
            if(prefab != null && prefab.TryGetComponent<RecreateModule>(out RecreateModule module) && string.IsNullOrEmpty(module.prefabGuid))
            {
                //module.prefabPath = Guid.NewGuid().ToString();
                var so = new SerializedObject(module);
                so.FindProperty("prefabGuid").stringValue = Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();
                Debug.Log("Saved Prefab "+module.prefabGuid);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [MenuItem("Assets/Load Prefabs")]
        private static void OnRuntimeInitialize()
        {
            Instance.LoadPrefabs();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Instance.LoadPrefabs();
        }


#endif
    }
}
