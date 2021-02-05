using UnityEngine;

namespace UINavigation
{
    public class SetApplicationNavComponent : MonoBehaviour
    {
        [SerializeField]
        private UINavRepository navGraph;

        private void Awake()
        {
            ApplicationComponent.Instance.NavGraph = navGraph;
        }
    }
}
