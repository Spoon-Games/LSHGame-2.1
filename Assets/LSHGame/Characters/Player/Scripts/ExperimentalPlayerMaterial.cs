using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [CreateAssetMenu(fileName = "PlayerMaterial", menuName = "LSHGame/ExperimentalPlayerMaterial")]
    public class ExperimentalPlayerMaterial : ScriptableObject
    {
        public static string[] PropertyTable = new string[]{"RunAccelCurve","RunDeaccelCurve","jumpSpeed"};
        public static Type[] PropertyTypeTable = new Type[] { typeof(AnimationCurve), typeof(AnimationCurve), typeof(int) };

        [SerializeField]
        public List<int> savedPropertys = new List<int>();

        [SerializeReference]
        public List<object> propertys = new List<object>();
    }
}
