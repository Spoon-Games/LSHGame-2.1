using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class DuoCharacterView : BaseCharacterView<DuoDialog,DuoCharacterView>
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

        private BaseVoice rightVoice;
        private BaseVoice leftVoice;
        private BaseVoice currentVoice;

        public override void Awake()
        {
            base.Awake();
            colorFocusedText = Color.white;
        }

        public override void OnEnter()
        {
            GetCharacterDefaultData(out Sprite leftSprite, out string leftName, false);
            characterImageLeft.sprite = leftSprite;
            nameFieldLeft.text = leftName;

            GetCharacterDefaultData(out Sprite rightSprit, out string rightName, true);
            characterImageRight.sprite = rightSprit;
            nameFieldRight.text = rightName;

            if (Dialog.Lines.Length > 0)
                SetFocus(Dialog.Lines[0].isRight);

            base.OnEnter();
        }

        protected override void ResetView()
        {
            base.ResetView();
            dialogField.DOKill(false);
            dialogField.text = "";
        }

        protected override void GetNext()
        {
            if(typewriteTween != null && typewriteTween.active)
            {
                typewriteTween.Complete();
                currentVoice?.Stop();
                return;
            }

            if(Dialog.GetNext(out LineData lineData))
            {

                UpdateVoiceLine(lineData);
                FadeFocus(lineData.isRight);

                if (!string.IsNullOrEmpty(lineData.sound))
                    FMODUnity.RuntimeManager.PlayOneShot(lineData.sound);
                else
                    currentVoice?.Play(lineData.text);

                if(lineData.image != null)
                {
                    (lineData.isRight ? characterImageRight : characterImageLeft).sprite = lineData.image;
                }

                typewriteTween = dialogField.DOTypeWritePerSpeed(lineData.text, typeSpeed).SetEase(Ease.Linear);
                return;
            }

            End();
        }

        private void UpdateVoiceLine(LineData lineData)
        {
            currentVoice?.Stop();
            if (lineData.voice != null)
            {
                if (lineData.isRight)
                    rightVoice = lineData.voice;
                else
                    leftVoice = lineData.voice;
            }

            currentVoice = lineData.isRight ? rightVoice : leftVoice;
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

            foreach (var line in Dialog.Lines)
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
