using DG.Tweening;
using UINavigation;
using UnityEngine;

namespace LSHGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAnimationComponent : MonoBehaviour, IAnimationComponent
    {
        public float FadeInTime = 0.5f;
        public Ease FadeInEase = Ease.OutQuad;
        [Space]
        public float FadeOutTime = 0.5f;
        public Ease FadeOutEase = Ease.InQuad;

        private CanvasGroup _canvasGroup;
        protected CanvasGroup CanvasGroup { get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            } }

        public bool InAnimate(string animation,IAnimActivity animActivity)
        {
            if(animation.IsNullEmptyOrEqual("fade"))
            {
                CanvasGroup.DOKill();
                animActivity.SetVisible(true);
                CanvasGroup.DOFade(1, FadeInTime).SetEase(FadeInEase).OnComplete(() => animActivity.OnInAnimationComplete());
                return true;
            }
            return false;
        }

        public bool OutAnimate(string animation,IAnimActivity animActivity)
        {
            if (animation.IsNullEmptyOrEqual("fade"))
            {
                CanvasGroup.DOKill();
                CanvasGroup.DOFade(0, FadeOutTime).SetEase(FadeOutEase).OnComplete(() => { animActivity.SetVisible(false); animActivity.OnOutAnimationComplete(); });
                return true;
            }
            return false;
        }
    }
}
