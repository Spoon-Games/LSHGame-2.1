using System;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class MatBounceSubProp : SubstanceProperty
    {
        [SerializeField]
        public BounceSettings BounceSettings;

        public bool AddGameObjectRotation = false;

        public UnityEvent OnBounceEvent;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IMatBounceRec r)
            {
                r.BounceSettings = BounceSettings;
                r.OnBounce += () => { OnBounceEvent.Invoke(); };
            }
        }
    }

    public interface IMatBounceRec
    {
        BounceSettings BounceSettings { get; set; }

        Action OnBounce { get; set; }
    }

    [System.Serializable]
    public class BounceSettings
    {
        public bool IsRelativeSpeed = true;
        public float BounceDamping = 0.7f;
        public Vector2 MinMaxBounceSpeed = -Vector2.one;

        public float BounceSpeed = 15f;


        public bool FixedRotation = false;
        public Vector2 MinMaxRotation = -Vector2.one * 1000;
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
                return GetDirDeg(Rotation + 90);
            }
            else
            {
                float initRot = GetAngleDeg(initialDirection) - 90;

                if (MinMaxRotation.x >= -360)
                    initRot = Mathf.Max(initRot, MinMaxRotation.x);

                if (MinMaxRotation.y >= -360)
                    initRot = Mathf.Min(initRot, MinMaxRotation.y);

                return GetDirDeg(initRot + 90);
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
