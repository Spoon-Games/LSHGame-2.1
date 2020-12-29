using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerIsLadderFeetSubProp : SubstanceProperty
    {
        [SerializeField]
        bool isFeetLadder = true;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IIsLadderFeetRec r)
            {
                r.IsFeetLadder = isFeetLadder;
            }
        }
    }

    public interface IIsLadderFeetRec
    {
        bool IsFeetLadder { get; set; }
    }
}
