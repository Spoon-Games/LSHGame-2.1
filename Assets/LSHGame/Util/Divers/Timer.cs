using UnityEngine;

namespace LSHGame.Util
{
    [System.Serializable]
    public class Timer : BaseTimer
    {
        public float durration = 1;

        public override bool Finished()
        {
            return ClockedTime + durration <= Time.fixedTime;
        }

        
    }

    [System.Serializable]
    public class RandomTimer : BaseTimer
    {
        public float minDurration = 1;
        public float maxDurration = 2;
        private float randomDurration = 0;

        public override void Clock()
        {
            randomDurration = Random.Range(minDurration, maxDurration);
            base.Clock();
        }

        public override bool Finished()
        {
            return ClockedTime + randomDurration <= Time.fixedTime;
        }
    }

    public abstract class BaseTimer
    {
        protected float _clockedTime = float.NegativeInfinity;
        public float ClockedTime => _clockedTime;

        public virtual void Clock()
        {
            _clockedTime = Time.fixedTime;
        }

        public abstract bool Finished();

        public virtual bool Active() => !Finished();
    }
}
