using System;
using UnityEngine;

namespace LSHGame.Util
{
    [Flags]
    public enum PlayerSubstanceColliderType { Main = 1, Feet = 2, Sides = 4, Head = 8, Ladders = 16}

    [DisallowMultipleComponent]
    public class PlayerSubSpecifier : MonoBehaviour,IPlayerSubSpecifier
    {
        [SerializeField]
        private PlayerSubstanceColliderType colliderType = PlayerSubstanceColliderType.Main;

        public PlayerSubstanceColliderType ColliderType => colliderType;
    }

    public interface IPlayerSubSpecifier : ISubstanceSpecifier
    {
        PlayerSubstanceColliderType ColliderType { get; }
    }

    public class PlayerSubstanceFilter : ISubstanceFilter
    {
        public PlayerSubstanceColliderType ColliderType;

        public bool IsValidSubstance(ISubstanceSpecifier specifier)
        {
            if(specifier is IPlayerSubSpecifier spec)
            {
                return GameUtil.IsOtherAllInFlag((int)spec.ColliderType, (int)ColliderType);
            }

            return false;
        }
    }
}
