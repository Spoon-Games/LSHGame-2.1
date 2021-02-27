using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    [CreateAssetMenu(menuName ="LSHGame/Editor/Scene Select Repository")]
    public class SceneSelectRepository : ScriptableObject
    {
        public SceneGroup[] groups;
    }

    [System.Serializable]
    public class SceneGroup
    {
        public string name;

        [LoadScene]
        public SceneAsset[] scenes;
    }
}
