using LSHGame.PlayerN;
using UnityEngine;

namespace LSHGame
{
    public class PlayerInfluencer : MonoBehaviour 
    {
        [SerializeField]
        private PlayerMaterial playerMaterial;
        public PlayerMaterial PlayerMaterial { get => playerMaterial; set => playerMaterial = value; }

        private bool isActive = false;

        private void Awake()
        {
            if(playerMaterial == null)
            {
                Debug.LogError("PlayerMaterial of PlayerInfluencer: "+name+" is null!");
            }
        }

        public void Activate() {
            SetActive(true);
        }

        public void Deactivate()
        {
            SetActive(false);
        }

        private void SetActive(bool active)
        {
            if (active == isActive)
                return;

            isActive = active;

            if (isActive)
            {
                Player.Instance.AddPlayerMaterial(playerMaterial);
            }
            else
            {
                Player.Instance.RemovePlayerMaterial(playerMaterial);
            }
        }
    }
}
