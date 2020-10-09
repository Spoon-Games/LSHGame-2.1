using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    [CreateAssetMenu(fileName = "LevelInfo", menuName = "SceneM/LevelInfo", order = 2)]
    public class LevelInfo : ScriptableObject
    {
        public string Name;

        public MainSceneInfo StartScene;

        public bool IsGlobal = false;
    }
}
