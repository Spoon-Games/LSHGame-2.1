namespace LSHGame.Util
{
    [System.Serializable]
    public class DefaultableProperty<T>
    {
        public bool isDefault = true;
        public T value;

        public void SetValue(ref T v)
        {
            if (!isDefault)
                v = value;
        }

        public static T operator +(T a,DefaultableProperty<T> b)
        {
            if (!b.isDefault)
                return b.value;
            return a;
        }
    }
}
