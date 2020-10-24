using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    [RequireComponent(typeof(Substance))]
    public class SubstanceTilePointer : MonoBehaviour
    {
        public List<TileBase> tilesOfSubstance = new List<TileBase>();
    }
}
