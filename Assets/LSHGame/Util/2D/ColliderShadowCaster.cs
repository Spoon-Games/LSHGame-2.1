using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace LSHGame.Util
{
    [RequireComponent(typeof(CompositeShadowCaster2D))]
    public class ColliderShadowCaster : MonoBehaviour
    {
        [SerializeField]
        public CompositeCollider2D compositeCollider;

        [HideInInspector]
        [SerializeField]
        public List<ShadowCaster2D> shadowCasters;
    }
}

