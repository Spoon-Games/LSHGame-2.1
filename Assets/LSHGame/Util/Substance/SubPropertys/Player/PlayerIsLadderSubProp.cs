namespace LSHGame.Util
{
    public class PlayerIsLadderSubProp : SubstanceProperty
    {
        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IIsLadderRec r)
            {
                r.IsLadder = true;
            }
        }
    }

    public interface IIsLadderRec
    {
        bool IsLadder { get; set; }
    }
}
