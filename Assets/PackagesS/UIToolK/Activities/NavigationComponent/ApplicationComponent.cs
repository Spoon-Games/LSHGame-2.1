using UnityEngine;

namespace UINavigation
{
    public class ApplicationComponent : NavigationComponent
    {
        public static ApplicationComponent Instance { get; private set; }

        protected override void Awake()
        {
            ApplicationComponent[] objects = FindObjectsOfType<ApplicationComponent>();
            if (objects.Length > 1)
                Destroy(gameObject);
            else
                Instance = this;

            base.Awake();
        }
    }
}
