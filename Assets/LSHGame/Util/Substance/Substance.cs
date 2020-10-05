using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    public class Substance : ScriptableObject
    {
        public List<TileBase> tilesOfSubstance = new List<TileBase>();

        public List<SubstanceProperty> substanceProperties = new List<SubstanceProperty>();

        public void GetSubProps(ISubPropFilter filter, HashSet<SubstanceProperty> subProps)
        {

            foreach (var p in substanceProperties)
            {
                if ((filter == null || filter.IsAcceptedSubProp(p)) && !subProps.Contains(p))
                    subProps.Add(p);
            }
        }
    }

    public interface ISubPropFilter
    {
        bool IsAcceptedSubProp(SubstanceProperty subProp);
    }
}
