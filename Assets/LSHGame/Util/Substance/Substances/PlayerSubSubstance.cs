using UnityEngine;

namespace LSHGame.Util
{
    public enum PlayerSubstanceColliderType { Main, Feet, Sides, Head, All}

    [DisallowMultipleComponent]
    public class PlayerSubSubstance : SubSubstance,IPlayerSubstanceFilterable
    {
        [SerializeField]
        private PlayerSubstanceColliderType colliderType = PlayerSubstanceColliderType.Main;

        public PlayerSubstanceColliderType ColliderType => colliderType;
    }

    public interface IPlayerSubstanceFilterable
    {
        PlayerSubstanceColliderType ColliderType { get; }
    }

    public class PlayerSubstanceFilter : ISubstanceFilter
    {
        public PlayerSubstanceColliderType ColliderType;

        public bool IsValidSubstance(ISubstance substance, out bool searchChildren)
        {
            searchChildren = false;
            if(substance is IPlayerSubstanceFilterable filterable)
            {

            }else if(substance is Substance)
            {

            }
            return false;
        }
    }
}
