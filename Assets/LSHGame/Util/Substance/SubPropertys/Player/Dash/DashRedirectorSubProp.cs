using UnityEngine;

namespace LSHGame.Util
{
    public class DashRedirectorSubProp : SubstanceProperty
    {
        public float TargetDashAngle = 90;
        public float TurningRadius = 1;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IDashRedirectorRec r)
            {
                r.TargetDashAngle = TargetDashAngle;
                r.DashTurningRadius = TurningRadius;
            }
        }
    }

    public interface IDashRedirectorRec
    {
        float TargetDashAngle { get; set; }
        float DashTurningRadius { get; set; }
    }
}
