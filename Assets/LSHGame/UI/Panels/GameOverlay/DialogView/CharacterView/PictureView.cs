using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class PictureView : BaseCharacterView<PictureLog,PictureView>
    {
        [SerializeField]
        private TMP_Text dialogField;
        [SerializeField]
        private Image imageField;
        [SerializeField]
        private float typeSpeed = 0.1f;

        [SerializeField]
        private float minScale = 0.3f;
        [SerializeField]
        private float inScaleTime = 0.7f;
        [SerializeField]
        private Ease inScaleEase = Ease.OutQuad;

        [SerializeField]
        private float outScaleTime = 0.5f;
        [SerializeField]
        private Ease outScaleEase = Ease.InQuad;

        [SerializeField]
        [FMODUnity.EventRef]
        private string defaultOpeningSound;

        private Tween typewriteTween;

        public override void OnEnter()
        {
            base.OnEnter();

            imageField.sprite = Dialog.Picture;
            imageField.rectTransform.localScale = new Vector3(minScale, minScale, 1);
            imageField.rectTransform.DOScale(Vector3.one, inScaleTime).SetEase(inScaleEase);

            dialogField.text = "";

            if (!string.IsNullOrEmpty(Dialog.OpeningSound))
            {
                FMODUnity.RuntimeManager.PlayOneShot(Dialog.OpeningSound);
            }else if (!string.IsNullOrEmpty(defaultOpeningSound))
            {
                FMODUnity.RuntimeManager.PlayOneShot(defaultOpeningSound);
            }
        }

        protected override void GetNext()
        {
            if (typewriteTween != null && typewriteTween.active)
            {
                typewriteTween.Complete();
                return;
            }

            if (Dialog.GetNext(out string text))
            {

                typewriteTween = dialogField.DOTypeWritePerSpeed(text, typeSpeed).SetEase(Ease.Linear);
                return;
            }

            End();
        }

        public override void OnLeave()
        {
            base.OnLeave();

            imageField.rectTransform.DOScale(new Vector3(minScale, minScale, 1), outScaleTime).SetEase(outScaleEase);
        }
    }
}
