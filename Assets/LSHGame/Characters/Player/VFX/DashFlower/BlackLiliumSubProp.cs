using LSHGame.Util;

namespace LSHGame.PlayerN
{
    public class BlackLiliumSubProp : SubstanceProperty
    {
        private bool isDead = true;
        public bool IsDead => isDead;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IBlackLiliumReciever r && IsDead)
            {
                r.BlackLiliumReference = this;
            }
        }

        public bool DeliverLilium()
        {
            if (!IsDead)
                return false;

            isDead = false;
            return true;
        }
    }

    public interface IBlackLiliumReciever
    {
        BlackLiliumSubProp BlackLiliumReference { get; set; }
    }
}
