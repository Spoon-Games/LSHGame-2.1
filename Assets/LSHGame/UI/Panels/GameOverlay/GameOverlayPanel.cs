using LSHGame.Util;
using SceneM;
using TMPro;
using UnityEngine;

namespace LSHGame.UI
{
    public class GameOverlayPanel : Singleton<GameOverlayPanel>
    {
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text helpText;

        [SerializeField]
        private InventoryItem scoreItem;

        private void Start()
        {
            Inventory.OnInventoryChanged += OnInventoryChanged;
            OnInventoryChanged();
        }

        private void OnInventoryChanged()
        {
            scoreText.text = Inventory.GetCount(scoreItem).ToString();
        }

        public void SetHelpText(string text)
        {
            helpText.gameObject.SetActive(true);
            helpText.text = text;
        }

        public void HideHelpText()
        {
            helpText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameInput.Controller.Player.Enable();
        }

        private void OnDisable()
        {
            GameInput.Controller.Player.Disable();
        }
    } 
}
