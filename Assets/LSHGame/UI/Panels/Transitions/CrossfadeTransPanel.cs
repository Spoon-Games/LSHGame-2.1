using DG.Tweening;
using UnityEngine;

namespace LSHGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CrossfadeTransPanel : TransitionPanel
    {
        [SerializeField]
        private Ease fadeInEase = Ease.OutCubic;
        [SerializeField]
        private Ease fadeOutEase = Ease.InCubic;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnSwitchState(State state)
        {
            switch (state)
            {
                case State.Idle:
                    canvasGroup.DOKill();
                    canvasGroup.alpha = 0;
                    break;
                case State.Start:
                    canvasGroup.alpha = 0;
                    canvasGroup.DOFade(1, base.PanelName.StartDurration).SetEase(fadeInEase);
                    break;
                case State.Middle:
                    canvasGroup.DOKill(true);
                    break;
                case State.End:
                    canvasGroup.DOFade(0, base.PanelName.EndDurration).SetEase(fadeOutEase);
                    break;
            }
        }
    }
}
