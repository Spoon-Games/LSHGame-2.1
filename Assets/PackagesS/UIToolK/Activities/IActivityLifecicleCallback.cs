namespace UINavigation
{
    public interface IActivityLifecicleCallback
    {
        void OnEnter();

        void OnEnterComplete();

        void OnLeave();

        void OnLeaveComplete();
    }
}
