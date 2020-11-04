using UnityEngine;

namespace LSHGame.Util
{
    public class DelayedSubstance : Substance
    {
        [SerializeField]
        public float Delay = 0;

        public override void AddToSet(SubstanceSet set, ISubstanceFilter filter)
        {
            if (set.Contains(this))
                return;
            if (SubstanceSpecifier.Count == 0)
            {
                AddToSetP(set, filter);
                return;
            }
            foreach (var specifier in SubstanceSpecifier)
            {
                if (filter.IsValidSubstance(specifier))
                {
                    AddToSetP(set, filter);
                    return;
                }
            }
        }

        private void AddToSetP(SubstanceSet set, ISubstanceFilter filter)
        {
            if (Delay <= 0)
            {
                DelayedAddToSet(set, filter);
                return;
            }
            set.SubSetQuery.Add(new DelayedSubExecutable(Delay, this, filter));
        }

        private void DelayedAddToSet(SubstanceSet set,ISubstanceFilter filter)
        {
            set.Add(this);
            foreach (var c in ChildSubstances)
            {
                c.AddToSet(set, filter);
            }
        }

        public class DelayedSubExecutable : ISubSetQueryable
        {
            private readonly float exeTime;
            private DelayedSubstance parent;
            private ISubstanceFilter filter;

            public DelayedSubExecutable(float delay, DelayedSubstance parent,ISubstanceFilter filter)
            {
                exeTime = delay + Time.fixedTime;
                this.parent = parent;
                this.filter = filter;
            }

            public bool ExeAddToSet(SubstanceSet substanceSet)
            {
                if (Time.fixedTime >= exeTime)
                {
                    parent.DelayedAddToSet(substanceSet, filter);
                    return true;
                }
                return false;
            }
        }
    }
}
