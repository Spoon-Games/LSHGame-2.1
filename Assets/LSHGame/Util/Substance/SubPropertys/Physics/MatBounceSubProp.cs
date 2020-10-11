namespace LSHGame.Util
{
    public class MatBounceSubProp : SubstanceProperty
    {
        public BounceSettings BounceSettings;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IMatBounceRec r)
            {
                r.BounceSettings = BounceSettings;
            }
        }
    }

    public interface IMatBounceRec
    {
        BounceSettings BounceSettings { get; set; }
    }

    [System.Serializable]
    public struct BounceSettings
    {
        public float BounceSpeed;
        public float Rotation;
        public bool AddGameObjectRotation;
        public bool ConstantHeight;
    }
}
