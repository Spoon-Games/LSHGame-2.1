using UnityEngine;

namespace LSHGame.Util
{
    public class DashCenteredRedirectorSubProp : SubstanceProperty
    {
        public Vector2 TurningCenter;

        public float TurningAngle = 90;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IDashCenteredRedirectorRec r)
            {
                r.GlobalDashTurningCenter = (Vector2)transform.position + TurningCenter;
                r.DashDeltaTurningAngle = TurningAngle;
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((Vector3)TurningCenter + transform.position, 0.1f);
        }
#endif
    }

    public interface IDashCenteredRedirectorRec
    {
        Vector2 GlobalDashTurningCenter { get; set; }
        float DashDeltaTurningAngle { get; set; }
    }
}
