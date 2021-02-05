using UnityEngine;

namespace UINavigation
{
    public class GoToNextComponent : MonoBehaviour
    {
        [SerializeField]
        private string nextKey;

        private IGoToNextManager manager;

        private void Awake()
        {
            manager = GetComponentInParent<IGoToNextManager>();
            if(manager == null)
            {
                Debug.LogError("No IGoToNextManager was found in parents GoToNextComponent: " + name);
            }
        }

        public void GoToNext()
        {
            if (manager == null)
                return;

            string s = nextKey;
            if (string.IsNullOrEmpty(s))
                s = gameObject.name;

            manager.GoToNext(s);
        }
    }

    public interface IGoToNextManager
    {
        void GoToNext(string key);
    }
}
