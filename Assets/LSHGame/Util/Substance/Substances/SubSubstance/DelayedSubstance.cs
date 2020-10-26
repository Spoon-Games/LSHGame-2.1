using SceneM;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class DelayedSubstance : FilterableSubstance
    {
        [SerializeField]
        public float Delay = 0;

        private ISubstanceFilter currentFilter;

        public override void AddToSet(HashSet<ISubstance> set, ISubstanceFilter filter)
        {
            if (set.Contains(this))
                return;
            if (SubstanceSpecifier.Count == 0)
            {
                set.Add(this);
                currentFilter = filter;
                return;
            }
            foreach (var specifier in SubstanceSpecifier)
            {
                if (filter.IsValidSubstance(specifier))
                {
                    set.Add(this);
                    currentFilter = filter;
                    return;
                }
            }
        }

        public override void RecieveData(IDataReciever dataReciever)
        {
            TimeSystem.Delay(Delay, DelayedRecive, true);
        }

        private void DelayedRecive(float t)
        {

        }
    }
}
