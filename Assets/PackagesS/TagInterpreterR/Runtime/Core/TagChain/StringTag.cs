namespace TagInterpreterR
{
    public sealed class StringTag : BaseTag
    {
        public string Text;

        public StringTag(string text)
        {
            Text = text;
            IsSingle = true;
            Name = null;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
