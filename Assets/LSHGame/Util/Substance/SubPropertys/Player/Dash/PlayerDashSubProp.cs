using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerDashSubProp : SubstanceProperty
    {
        [Header("Dash")]
        public DefaultableProperty<float> DashDurration;
        public DefaultableProperty<float> DashSpeed;
        public DefaultableProperty<float> DashRecoverDurration;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IPlayerDashRec r)
            {
                r.DashDurration += DashDurration;
                r.DashSpeed += DashSpeed;
                r.DashRecoverDurration += DashRecoverDurration;
            }
        }
    }

    public interface IPlayerDashRec
    {
        float DashDurration { get; set; }
        float DashSpeed { get; set; }
        float DashRecoverDurration { get; set; }
    }
}
