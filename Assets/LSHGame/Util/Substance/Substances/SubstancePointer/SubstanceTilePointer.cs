using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    [RequireComponent(typeof(Substance))]
    public class SubstanceTilePointer : SubstancePointer
    {
        public List<TileBase> tilesOfSubstance = new List<TileBase>();
    }
}
