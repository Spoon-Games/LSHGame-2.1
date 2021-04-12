using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    [CreateAssetMenu(menuName ="LSHGame/Debug/SceneInfoRepository")]
    public class SceneInfoRepository:ScriptableObject
    {
        public MainSceneInfo[] MainSceneInfos;
    }
}
