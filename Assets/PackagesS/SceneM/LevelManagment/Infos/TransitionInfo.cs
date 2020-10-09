using UnityEngine;

namespace SceneM
{
    [CreateAssetMenu(fileName = "TransitionInfo", menuName = "SceneM/TransitionInfo", order = 2)]
    public class TransitionInfo : ScriptableObject
    {
        public string TransitionName;

        public float StartDurration = 0f;

        public float MinMiddleDurration = 0f;

        public float EndDurration = 0f;
    }
}
