using UnityEngine;

namespace SceneM
{
    public class ScriptableSingletonManager : MonoBehaviour
    {
        [SerializeField]
        public ScriptableSingleton[] references;
    }
}
