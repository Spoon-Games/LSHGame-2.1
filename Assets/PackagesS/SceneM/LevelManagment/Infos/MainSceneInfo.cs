using UnityEngine;

namespace SceneM
{
    [CreateAssetMenu(fileName = "MainSceneInfo", menuName = "SceneM/MainSceneInfo", order = 1)]
    public class MainSceneInfo : MySceneInfo
    {
        public AdditiveSceneInfo[] AdditionalStartScenes = new AdditiveSceneInfo[0];

        public TransitionInfo Transition;

        public CheckpointInfo StartCheckpoint;
    }
}
