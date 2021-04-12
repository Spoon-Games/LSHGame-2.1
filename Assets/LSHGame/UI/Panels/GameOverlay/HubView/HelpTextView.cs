using DG.Tweening;
using SceneM;
using TMPro;
using UnityEngine;

namespace LSHGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HelpTextView : Singleton<HelpTextView>
    {
        [SerializeField]
        private TMP_Text helpText;
        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        [SerializeField]
        private float fadeInTime = 0.5f;
        [SerializeField]
        private Ease fadeInEase = Ease.OutQuad;
        [SerializeField]
        private float fadeOutTime = 0.5f;
        [SerializeField]
        private Ease fadeOutEase = Ease.InQuad;
        private CanvasGroup _canvasGroup;

        private GameObject ShowIdentifier = null;

        public override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        public void SetHelpText(string text, GameObject identifier = null)
        {
            ShowIdentifier = identifier;

            gameObject.SetActive(true);
            helpText.text = text;

            CanvasGroup.DOKill();
            CanvasGroup.DOFade(1, fadeInTime).SetEase(fadeInEase);

        }

        public void HideHelpText(GameObject identifier = null)
        {
            if (identifier != null && !Equals(identifier, ShowIdentifier))
                return;
            ShowIdentifier = null;

            CanvasGroup.DOKill();
            CanvasGroup.DOFade(0, fadeOutTime).SetEase(fadeOutEase).OnComplete(() => gameObject.SetActive(false));
        }

        public void HideHelpText(string originText)
        {
            if (Equals(helpText.text, originText))
                HideHelpText();
        }
    }
}
