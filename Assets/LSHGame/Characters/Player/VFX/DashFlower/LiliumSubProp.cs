using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    public class LiliumSubProp : SubstanceProperty
    {
        [SerializeField]
        private ParticleSystem liliumParticleSystem;
        public ParticleSystem LiliumParticleSystem => liliumParticleSystem;

        private bool hasLilium = true;
        public bool HasLilium => hasLilium;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is ILiliumReciever r && HasLilium)
            {
                r.LiliumReference = this;
            }
        }


        public bool GetLilium()
        {
            if (!HasLilium)
                return false;

            hasLilium = false;
            return true;
        }
    }

    public interface ILiliumReciever
    {
        LiliumSubProp LiliumReference { get; set; }
    }
}
