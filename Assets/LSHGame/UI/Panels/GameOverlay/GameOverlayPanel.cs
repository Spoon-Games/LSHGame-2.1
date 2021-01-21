using DG.Tweening;
using LSHGame.Util;
using SceneM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class GameOverlayPanel : Singleton<GameOverlayPanel>
    {
        [SerializeField]
        private TMP_Text helpText;
        [SerializeField]
        private Image helpTextImage;
        [SerializeField]
        private float fadeInTime = 0.5f;

        [SerializeField]
        private InventoryItem scoreItem;

        public void SetHelpText(string text)
        {
            helpTextImage.gameObject.SetActive(true);
            helpText.text = text;

            helpTextImage.DOKill();
            helpTextImage.DOFade(1, fadeInTime).SetEase(Ease.OutQuad);

        }

        public void HideHelpText()
        {
            helpTextImage.DOKill();
            helpTextImage.DOFade(0, fadeInTime).SetEase(Ease.InQuad).OnComplete(() => helpTextImage.gameObject.SetActive(false));
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
