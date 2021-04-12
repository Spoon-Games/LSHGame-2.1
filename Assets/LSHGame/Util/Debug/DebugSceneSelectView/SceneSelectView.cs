using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.Util
{
    public class SceneSelectView : MonoBehaviour
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
        private SceneInfoRepository sceneInfoRepository;

        private void Awake()
        {
            Load();

            backButton.onClick.AddListener(() => SetVisible(false));
            GameInput.ToggleDebugSceneView += ToggleVisible;
            SetVisible(false);
        }

        private void Load()
        {

            foreach (Transform child in sceneElementContent)
                Destroy(child.gameObject);

            //var querry = from info in serializedSceneInfos
            //             group info by info.ScenePath into newGroup
            //             select newGroup;

            foreach(var info in sceneInfoRepository.MainSceneInfos)
            {
                SceneSelectElement element = Instantiate(elementPrefab, sceneElementContent);
                element.Initialize(info);
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
    }
}
