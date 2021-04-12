using SceneM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.Util
{
    public class SceneSelectElement : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text sceneNameField;

        [SerializeField]
        private Button loadButton;

        //[SerializeField]
        //private TMP_Dropdown loadSelectionField;

        //private MainSceneInfo[] sceneInfos;

        private MainSceneInfo sceneInfo;

        private void Awake()
        {
            loadButton.onClick.AddListener(OnLoad);
        }

        private void OnLoad()
        {
            if (sceneInfo == null)
                Debug.Log("No SceneInfo assigned");

            LevelManager.LoadScene(sceneInfo);
            //if(sceneInfos == null || sceneInfos.Length == 0)
            //{
            //    Debug.Log("No SceneInfos assigned");
            //    return;
            //}

            //int selected = Mathf.Clamp(loadSelectionField.value, 0, sceneInfos.Length - 1);
            //LevelManager.LoadScene(sceneInfos[selected]);
        }

        public void Initialize(MainSceneInfo sceneInfo)
        {
            this.sceneInfo = sceneInfo;

            string name = Path.GetFileNameWithoutExtension(sceneInfo.name);
            if (name.EndsWith("_Info"))
                name = name.Substring(0, name.Length - 5);

            sceneNameField.text = name;
        }

        private void OnDestroy()
        {
            loadButton.onClick.RemoveListener(OnLoad);
        }

        //public void Initialize(MainSceneInfo[] sceneInfos)
        //{
        //    loadSelectionField.ClearOptions();

        //    List<string> options = new List<string>();
        //    foreach(var sceneInfo in sceneInfos)
        //    {
        //        string text = sceneInfo.name;
        //        if (sceneInfo.StartCheckpoint != null)
        //            text += " " + sceneInfo.StartCheckpoint.name;

        //        options.Add(text);
        //    }
        //    loadSelectionField.AddOptions(options);

        //    sceneNameField.text = Path.GetFileNameWithoutExtension(sceneInfos[0].ScenePath);

        //    this.sceneInfos = sceneInfos;
        //}
    }
}
