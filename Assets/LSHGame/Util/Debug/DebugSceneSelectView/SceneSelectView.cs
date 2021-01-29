using SceneM;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

namespace LSHGame.Util
{
    public class SceneSelectView : MonoBehaviour
#if UNITY_EDITOR
        , UnityEditor.Build.IPreprocessBuildWithReport
#endif
    {
        [SerializeField]
        private SceneSelectElement elementPrefab;

        [SerializeField]
        private RectTransform sceneElementContent;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private GameObject content;

        [SerializeField]
        private MainSceneInfo[] serializedSceneInfos;

        public int callbackOrder { get; }

        private void Awake()
        {
            Load();

            backButton.onClick.AddListener(() => SetVisible(false));
            GameInput.ToggleDebugSceneView += ToggleVisible;
            SetVisible(false);
        }

        private void Load()
        {
#if UNITY_EDITOR
            LoadMainSceneInfos();
#endif

            foreach (Transform child in sceneElementContent)
                Destroy(child.gameObject);

            var querry = from info in serializedSceneInfos
                         group info by info.ScenePath into newGroup
                         select newGroup;

            foreach(var q in querry)
            {
                SceneSelectElement element = Instantiate(elementPrefab, sceneElementContent);
                element.Initialize(q.ToArray());
            }
        }

        private void ToggleVisible()
        {
            SetVisible(!content.activeSelf);
        }

        private void SetVisible(bool visilbe)
        {
            content.SetActive(visilbe);
        }

        private void OnDestroy()
        {
            GameInput.ToggleDebugSceneView -= ToggleVisible;
        }

#if UNITY_EDITOR



        [RuntimeInitializeOnLoadMethod]
        private void LoadMainSceneInfos()
        {
            List<MainSceneInfo> sceneInfos = new List<MainSceneInfo>();
            //var paths = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Substance).ToString());
            //foreach (var path in paths)
            //{
            //    substances.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<Substance>(path));
            //}

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:MainSceneInfo");
            foreach (var guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                // Debug.Log("Path: " + path);

                MainSceneInfo s = UnityEditor.AssetDatabase.LoadAssetAtPath<MainSceneInfo>(path);
                if (s != null)
                    sceneInfos.Add(s);
            }
            serializedSceneInfos = sceneInfos.ToArray();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            LoadMainSceneInfos();
        }

#endif
    }
}
