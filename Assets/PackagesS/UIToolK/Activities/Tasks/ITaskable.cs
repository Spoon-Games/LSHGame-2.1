namespace UINavigation
{
    public interface ITaskable
    {
        void OnEnter();

        void OnLeave();
    }

    public interface INextableTask
    {
        void GoToNext(string key);
    }
}
