using UnityEngine;

namespace UINavigation
{
    public class SceneSetNavGraphComponent : MonoBehaviour
    {
        [SerializeField]
        private UINavRepository navGraph;

        private void Awake()
        {
            UINavigationComponent.Instance.NavGraph = navGraph;
        }
    }
}
