using DG.Tweening;
using SceneM;
using TMPro;
using UnityEngine;

namespace LSHGame.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ScoreField : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreTextField;

        private RectTransform rectTransform;

        [SerializeField]
        private int debugCount = 1;
        private int lastDebugCount = 1;

        public Ease easeIn = Ease.OutCubic;
        public float inDurration = 1;
        public float textInStartDelay = 1;
        public Color highlightColor = Color.green;
        public float colorHighlightDurration = 0.5f;
        public Ease colorHighlightEase = Ease.OutCubic;
        public float xMaxScale = 1.1f;
        public Ease xScaleEase = Ease.OutCubic;

        [SerializeField]
        public float highlightDelay = 0.1f;

        public Ease xScaleOutEase = Ease.InCubic;
        public Color normalColor = Color.white;
        public Ease colorNormalEase = Ease.InCubic;
        public float colorNormalDurration = 0.5f;

        public Ease easeOut = Ease.InCubic;
        public float outDurration = 1;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(1,rectTransform.pivot.y);

            Inventory.OnItemAdded += OnItemAdded;
            scoreTextField.color = normalColor;
        }

        private void Update()
        {
            if(lastDebugCount != debugCount)
            {
                lastDebugCount = debugCount;
                GenerateSequence(debugCount.ToString());
            }
        }

        private void OnItemAdded(InventoryItem item,int count)
        {
            if (Equals(item.ItemType, InvetoryTags.SCORE_TAG))
            {
                GenerateSequence(count.ToString());
            }
        }

        private void GenerateSequence(string newText)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOPivotX(0, inDurration).SetEase(easeIn));
            sequence.AppendInterval(textInStartDelay);
            sequence.Append(scoreTextField.DOColor(highlightColor, colorHighlightDurration).SetEase(colorHighlightEase));

            sequence.AppendCallback(() => { scoreTextField.text = newText; });
            sequence.AppendInterval(highlightDelay);

            sequence.Append(scoreTextField.DOColor(normalColor, colorNormalDurration).SetEase(colorNormalEase));
            sequence.AppendInterval(textInStartDelay);
            sequence.Append(rectTransform.DOPivotX(1, outDurration).SetEase(easeOut));

            sequence.Insert(inDurration + textInStartDelay, scoreTextField.rectTransform.DOScaleX(xMaxScale, colorHighlightDurration).SetEase(xScaleEase));
            sequence.Insert(inDurration + textInStartDelay + colorHighlightDurration + highlightDelay, scoreTextField.rectTransform.DOScaleX(1, colorNormalDurration).SetEase(xScaleOutEase));

            sequence.Play();
        }
    }
}
