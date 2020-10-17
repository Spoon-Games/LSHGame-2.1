namespace LSHGame.Util
{
    public class GravitySubProp : SubstanceProperty
    {
        public DefaultableProperty<float> Gravity;
        public DefaultableProperty<float> FallDamping;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IGravityRec r)
            {
                r.Gravity += Gravity;
                r.FallDamping += FallDamping;
            }
        }
    }

    public interface IGravityRec
    {
        float Gravity { get; set; }
        float FallDamping { get; set; }
    }
}
