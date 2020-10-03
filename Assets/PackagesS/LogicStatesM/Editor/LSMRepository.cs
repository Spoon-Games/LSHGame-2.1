using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace LogicStateM.Editor
{
    public class LSMRepository : ScriptableObject
    {
        #region Singleton
        private static LSMRepository instance;
        internal static LSMRepository Instance
        {
            get
            {
                if (instance == null)
                    instance = GetRepository();
                return instance;
            }
        }

        private static LSMRepository GetRepository()
        {
            string path = "";
            foreach(string p in AssetDatabase.GetAllAssetPaths())
            {
                if ("LSMRepository".Equals(Path.GetFileNameWithoutExtension(p)))
                {
                    path = Path.GetDirectoryName(p);
                }
            }
            LSMRepository instance = AssetDatabase.LoadAssetAtPath<LSMRepository>(path + "/LSMRepository.asset");
            if (instance == null)
            {
                instance = CreateInstance<LSMRepository>();
                AssetDatabase.CreateAsset(instance, path + "/LSMRepository.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return instance;
        }
        #endregion

        [Tooltip("For all these animators, a suiting C#-class will be generated.")]
        [SerializeField]
        private List<AnimatorController> animators = new List<AnimatorController>();

        [SerializeField]
        private bool generateScripts = true;

        private void OnValidate()
        {

        }

        internal void UpdateController(int index)
        {
            LSMCodeGenerator.GenerateLSM(animators[index]);
        }

        internal void UpdateController(string path)
        {
            if (!generateScripts)
                return;

            foreach(var a in animators)
            {
                if(a != null && AssetDatabase.GetAssetPath(a).Equals(path))
                {
                    LSMCodeGenerator.GenerateLSM(a);
                }
            }
        }
    }
}
