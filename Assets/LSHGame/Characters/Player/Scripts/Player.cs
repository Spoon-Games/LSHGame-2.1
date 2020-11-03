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

        public override void Awake()
        {
            base.Awake();
            characterController = GetComponent<PlayerController>();

            characterController.Initialize(this);
        }

        public void PlayFootstep()
        {
            characterController.PlayFootstep();
        }
    }
}
