namespace UINavigation
{
    public interface IAnimationComponent
    {
        bool InAnimate(string animation,IAnimActivity animActivity);

        bool OutAnimate(string animation,IAnimActivity animActivity);
    }

    public interface IAnimActivity
    {
        void SetVisible(bool visible);

        void OnInAnimationComplete();

        void OnOutAnimationComplete();
    }
}
