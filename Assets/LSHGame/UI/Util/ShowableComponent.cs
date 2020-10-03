using DG.Tweening;
using UnityEngine;

namespace LSHGame.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ShowableComponent : MonoBehaviour
    {
        [SerializeField]
        private bool isVisible = false;
        public bool IsVisible
        {
            get => isVisible; set
            {
                if (value == isVisible)
                    return;
                if (value)
                    ShowComponent();
                else
                    HideComponent();
                isVisible = value;
            }
        }

        protected RectTransform rectTransform;

        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            TryGetComponent<SpriteRenderer>(out spriteRenderer);

            isVisible = !isVisible;
            IsVisible = !isVisible;
        }

        protected virtual void ShowComponent()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.DOFade(1, 1).SetEase(Ease.OutQuad);
            }
        }

        protected virtual void HideComponent()
        {
            spriteRenderer?.DOFade(0, 1).SetEase(Ease.InQuad);
        }
    }
}
