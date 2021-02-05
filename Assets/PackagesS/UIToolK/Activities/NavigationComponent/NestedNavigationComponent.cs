using UnityEngine;

namespace UINavigation
{
    [RequireComponent(typeof(Activity))]
    public class NestedNavigationComponent : NavigationComponent, IActivityLifecicleCallback
    {
        public void OnEnter()
        {
            if (CurrantTask is Activity a)
                a.OnGainFocus();
        }

        public void OnEnterComplete()
        {
        }

        public void OnLeave()
        {
            if (CurrantTask is Activity a)
                a.OnLoseFocus();
        }

        public void OnLeaveComplete()
        {
        }
    }
}
