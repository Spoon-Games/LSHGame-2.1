using UnityEngine;

namespace LSHGame.Util
{
    public class MatBounceSubProp : SubstanceProperty
    {
        public BounceSettings BounceSettings;

        public bool AddGameObjectRotation = false;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IMatBounceRec r)
            {
                r.BounceSettings = BounceSettings;
            }
        }
    }

    public interface IMatBounceRec
    {
        BounceSettings BounceSettings { get; set; }
    }

    [System.Serializable]
    public class BounceSettings
    {
        public bool IsRelativeSpeed = true;
        public float BounceDamping = 0.7f;
        public Vector2 MinMaxBounceSpeed = -Vector2.one;

        public float BounceSpeed = 15f;


        public bool FixedRotation = false;
        public Vector2 MinMaxRotation;
        public float Rotation;

        public float GetBounceSpeed(float initialSpeed)
        {
            if (IsRelativeSpeed)
            {
                initialSpeed *= BounceDamping;
                if (MinMaxBounceSpeed.x >= 0)
                    initialSpeed = Mathf.Max(initialSpeed, MinMaxBounceSpeed.x);

                if (MinMaxBounceSpeed.y >= 0)
                    initialSpeed = Mathf.Min(initialSpeed, MinMaxBounceSpeed.y);
            }
            else
                initialSpeed = BounceSpeed;

            return initialSpeed;
        }

        public Vector2 GetRotation(Vector2 initialDirection)
        {
            if (FixedRotation)
            {
                return GetDirDeg(Rotation);
            }
            else
            {
                float initRot = GetAngleDeg(initialDirection);

                if (MinMaxRotation.x >= 0)
                    initRot = Mathf.Max(initRot, MinMaxRotation.x);

                if (MinMaxRotation.y >= 0)
                    initRot = Mathf.Min(initRot, MinMaxRotation.y);

                return GetDirDeg(initRot);
            }
        }

        private Vector2 GetDirDeg(float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        private float GetAngleDeg(Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }
}
