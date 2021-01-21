using DG.Tweening;
using LSHGame.Util;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class GenericCharacterView<T> : BaseCharacterView where T : BaseDialog
    {
        [SerializeField]
        protected Button furtherButton;

        protected T dialog;

        [SerializeField]
        private float fadeInTime = 1f;
        [SerializeField]
        private Ease fadeInEase = Ease.OutQuad;

        [SerializeField]
        private float fadeOutTime = 0.7f;
        [SerializeField]
        private Ease fadeOutEase = Ease.InQuad;

        protected CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            furtherButton?.onClick.AddListener(GetNext);

            GameInput.Controller.UI.Further.performed += ctx => GetNext();
        }

        public override bool TrySetDialog(BaseDialog baseDialog)
        {
            if(baseDialog is T d)
            {
                dialog = d;
                Reset();
                OnStart();
                return true;
            }
            return false;
        }

        protected virtual void Reset()
        {
            canvasGroup.DOKill(false);
            dialog.Reset();
        }

        protected virtual void OnStart()
        {
            GameInput.Controller.Player.Disable();
            gameObject.SetActive(true);
            canvasGroup.DOFade(1, fadeInTime).SetEase(fadeInEase).OnComplete(GetNext);
        }

        protected virtual void GetNext()
        {

        }

        protected virtual void OnEnd()
        {
            canvasGroup.DOFade(0, fadeOutTime).SetEase(fadeOutEase).OnComplete(OnCompleteEnd);
        }

        protected virtual void OnCompleteEnd()
        {
            GameInput.Controller.Player.Enable();
            gameObject.SetActive(false);
        }
    }
}
