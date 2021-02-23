using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerIsLadderSubProp : SubstanceProperty
    {
        [SerializeField]
        bool isLadder = true;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IIsLadderRec r)
            {
                r.IsLadder = isLadder;
            }
        }
    }

    public interface IIsLadderRec
    {
        bool IsLadder { get; set; }
    }
}
