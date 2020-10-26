using LSHGame.Environment;
using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Singleton<Player>
    {
        private PlayerController characterController;

        [SerializeField]
        public bool IsDashEnabled = true;

        [SerializeField]
        public bool IsWallClimbEnabled = true;

        [SerializeField]
        private PlayerMaterial defaultPlayerMaterial;
        private PlayerStats stats;

        public override void Awake()
        {
            base.Awake();
            characterController = GetComponent<PlayerController>();

            stats = new PlayerStats(defaultPlayerMaterial);
            characterController.Initialize(this,stats);
        }

        public void PlayFootstep()
        {
            characterController.PlayFootstep();
        }

        public void AddPlayerMaterial(PlayerMaterial material)
        {
            stats.AddMaterial(material);
        }

        public void RemovePlayerMaterial(PlayerMaterial material)
        {
            stats.RemoveMaterial(material);
        }
    }
}
