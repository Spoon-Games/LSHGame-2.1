using UnityEngine;

namespace LSHGame.Util
{
    public class MatVelocitySubProp : SubstanceProperty
    {
        public Vector2 MovingVelocity; 

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IMatVelocityRec r)
            {
                Debug.Log("Recive Moving Platform Velocity");
                r.MovingVelocity = MovingVelocity;
            }
        }
    }

    public interface IMatVelocityRec
    {
        Vector2 MovingVelocity { get; set; }
    }
}
