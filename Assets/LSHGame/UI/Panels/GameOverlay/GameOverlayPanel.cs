using LSHGame.Util;
using SceneM;
using TMPro;
using UnityEngine;

namespace LSHGame.UI
{
    public class GameOverlayPanel : Singleton<GameOverlayPanel>
    {
        [SerializeField]
        private TMP_Text helpText;

        [SerializeField]
        private InventoryItem scoreItem;

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
