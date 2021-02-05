using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class ImageScaler : MonoBehaviour
    {
        [SerializeField]
        private bool ScaleX = true;

        private Image image;
        private Sprite sprite;
        private RectTransform rectTransform;
        private float lastScale;

        private void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if(sprite != image.sprite || rectTransform.rect.size[ScaleX?1:0] != lastScale)
            {
                UpdateScale();
            }

        }

        private void UpdateScale()
        {
            sprite = image.sprite;
            lastScale = rectTransform.rect.size[ScaleX?1:0];

            if (sprite == null)
                return;

            if (ScaleX)
            {
                rectTransform.sizeDelta = new Vector2(sprite.rect.width / sprite.rect.height * lastScale, rectTransform.sizeDelta.y);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,sprite.rect.height / sprite.rect.width * lastScale);
            }
        }
    }
}
