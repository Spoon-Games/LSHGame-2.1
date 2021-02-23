using UnityEngine;

namespace LSHGame.Util
{
    public class RepeatedSubstance : Substance , ISubSetQueryable
    {
        [SerializeField]
        public float Delay = 0;

        //Temp
        float exeTime = float.PositiveInfinity;
        ISubstanceFilter exeFilter;

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

        //Only temporarily
        public bool ExeAddToSet(SubstanceSet substanceSet)
        {
            if (Time.fixedTime < exeTime)
            {
                DelayedAddToSet(substanceSet, exeFilter);
                return false;
            }
            return true;
        }

        private void AddToSetP(SubstanceSet set, ISubstanceFilter filter)
        {
            if(Delay<= 0)
            {
                DelayedAddToSet(set, filter);
                return;
            }
            exeTime = Delay + Time.fixedTime;
            exeFilter = filter;
            set.SubSetQuery.Add(this);
        }

        private void DelayedAddToSet(SubstanceSet set, ISubstanceFilter filter)
        {
            set.Add(this);
            foreach (var c in ChildSubstances)
            {
                c.AddToSet(set, filter);
            }
        }

        //public class RepeatedSubExecutable : ISubSetQueryable
        //{
        //    private readonly float exeTime;
        //    private RepeatedSubstance parent;
        //    private ISubstanceFilter filter;

        //    public RepeatedSubExecutable(float delay, RepeatedSubstance parent, ISubstanceFilter filter)
        //    {
        //        exeTime = delay + Time.fixedTime;
        //        this.parent = parent;
        //        this.filter = filter;
        //    }

        //    public bool ExeAddToSet(SubstanceSet substanceSet)
        //    {
        //        if (Time.fixedTime < exeTime)
        //        {
        //            parent.DelayedAddToSet(substanceSet, filter);
        //            return false;
        //        }
        //        return true;
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        if (obj == null || GetType() != obj.GetType())
        //        {
        //            return false;
        //        }
        //        if (((RepeatedSubExecutable)obj).parent == parent)
        //            return true;

        //        return false;
        //    }

        //    public override int GetHashCode()
        //    {
        //        return parent.GetHashCode();
        //    }
        //}
    }
}
