using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class DuoCharacterView : GenericCharacterView<DuoDialog>
    {
        [Header("References")]
        public Image characterImageRight;
        public Image characterImageLeft;

        public TMP_Text nameFieldRight;
        public TMP_Text nameFieldLeft;

        public TMP_Text dialogField;


        [Header("Attributes")]
        public Color colorNoneFocusedImage;
        public Color colorNoneFocusedText;
        public float fadeFocusTime = 0.2f;

        private Color colorFocusedText;

        [SerializeField]
        private float typeSpeed = 0.1f;

        private Tween typewriteTween;

        protected override void Awake()
        {
            base.Awake();
            colorFocusedText = Color.white;
        }

        protected override void OnStart()
        {
            GetCharacterDefaultData(out Sprite leftSprite, out string leftName, false);
            characterImageLeft.sprite = leftSprite;
            nameFieldLeft.text = leftName;

            GetCharacterDefaultData(out Sprite rightSprit, out string rightName, true);
            characterImageRight.sprite = rightSprit;
            nameFieldRight.text = rightName;

            if (dialog.Lines.Length > 0)
                SetFocus(dialog.Lines[0].isRight);

            base.OnStart();
        }

        protected override void Reset()
        {
            base.Reset();
            dialogField.DOKill(false);
            dialogField.text = "";
        }

        protected override void GetNext()
        {
            if(typewriteTween != null && typewriteTween.active)
            {
                typewriteTween.Complete();
                return;
            }

            if(dialog.GetNext(out LineData lineData))
            {
                FadeFocus(lineData.isRight);

                if (!string.IsNullOrEmpty(lineData.sound))
                    FMODUnity.RuntimeManager.PlayOneShot(lineData.sound);

                if(lineData.image != null)
                {
                    (lineData.isRight ? characterImageRight : characterImageLeft).sprite = lineData.image;
                }

                typewriteTween = dialogField.DOTypeWritePerSpeed(lineData.text, typeSpeed);
                return;
            }

            OnEnd();
        }

        private void SetFocus(bool isRight)
        {
            if (isRight)
            {
                characterImageRight.color = Color.white;
                nameFieldRight.color = colorFocusedText;

                characterImageLeft.color = colorNoneFocusedImage;
                nameFieldLeft.color = colorNoneFocusedText;
            }
            else
            {
                characterImageLeft.color = Color.white;
                nameFieldLeft.color = colorFocusedText;

                characterImageRight.color = colorNoneFocusedImage;
                nameFieldRight.color = colorNoneFocusedText;
            }
        }

        private void FadeFocus(bool isRight)
        {
            FadeFocus(characterImageRight, nameFieldRight, isRight);
            FadeFocus(characterImageLeft, nameFieldLeft, !isRight);
        }

        private void FadeFocus(Image image,TMP_Text textField,bool focus)
        {
            image.DOKill(false);
            textField.DOKill(false);
            if (focus)
            {
                if (image.color != Color.white)
                    image.DOColor(Color.white, fadeFocusTime).SetEase(Ease.InCubic);

                if (textField.color != colorFocusedText)
                    textField.DOColor(colorFocusedText, fadeFocusTime).SetEase(Ease.InCubic);
            }
            else
            {
                if (image.color != colorNoneFocusedImage)
                    image.DOColor(colorNoneFocusedImage, fadeFocusTime).SetEase(Ease.OutCubic);

                if (textField.color != colorNoneFocusedText)
                    textField.DOColor(colorNoneFocusedText, fadeFocusTime).SetEase(Ease.OutCubic);
            }
        }

        private void GetCharacterDefaultData(out Sprite sprite, out string name,bool isRight)
        {
            sprite = null;
            name = null;

            foreach (var line in dialog.Lines)
            {
                if (sprite != null && name != null)
                    break;

                if (line.isRight == isRight)
                {
                    if (sprite == null)
                        sprite = line.image;
                    if (name == null)
                        name = line.name;
                }
            }
        }
    }
}
