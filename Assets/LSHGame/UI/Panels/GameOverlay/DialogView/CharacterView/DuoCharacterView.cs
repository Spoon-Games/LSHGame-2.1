using DG.Tweening;
using System.Linq;
using TagInterpreterR;
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

        private CharacterVoice currentVoice;

        public override void Awake()
        {
            base.Awake();
            colorFocusedText = Color.white;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            dialogField.text = "";
            SetDefaultMood(Dialog.PersonLeft, false);
            SetDefaultMood(Dialog.PersonRight, true);
        }

        #region Tag Callbacks
        protected override void OnCreateTag(BaseTag tag)
        {
            base.OnCreateTag(tag);

            if (tag is TagReference tagReference && !tag.IsSingle)
            {
                if (tag.Name == Dialog.PersonLeft.Name)
                    CreatePersonTag(tagReference, Dialog.PersonLeft, false);
                else if (tag.Name == Dialog.PersonRight.Name)
                    CreatePersonTag(tagReference, Dialog.PersonRight, true);
            }else if(tag is EndTag endTag && endTag.ReferenceTag is TagReference tagReference2)
            {
                if (tagReference2.Name == Dialog.PersonLeft.Name || tagReference2.Name == Dialog.PersonRight.Name)
                {
                    if (!(tagReference2.IsAttribute("IsButton", "false")))
                        ConsumeIsFurther();
                }
            }
        }

        protected override bool OnUpdateTag(BaseTag tag)
        {
            if (base.OnUpdateTag(tag))
                return true;

            if(tag is EndTag endTag && endTag.ReferenceTag is TagReference tagReference)
            {
                if(tagReference.Name == Dialog.PersonLeft.Name || tagReference.Name == Dialog.PersonRight.Name)
                {
                    if (!(tagReference.IsAttribute("IsButton","false")))
                        if (!ConsumeIsFurther())
                            return true;
                }
            }

            return false;
        }

        protected override void OnDestroyTag(BaseTag tag, bool returnToDefault)
        {
            base.OnDestroyTag(tag, returnToDefault);

            if(tag is TagReference tagReference)
            {
                if (tagReference.Name == Dialog.PersonLeft.Name || tagReference.Name == Dialog.PersonRight.Name)
                {
                    if (!(tagReference.IsAttribute("Page","false")))
                        ClearText();
                }
            }
        }
        #endregion

        #region Person

        private void CreatePersonTag(TagReference tag,Person person,bool isRight)
        {
            Mood mood= null;
            Mood defaultMood = person.Moods.FirstOrDefault();
            if (tag.Attributes.TryGetValue("value", out string moodName) || tag.Attributes.TryGetValue("Mood", out moodName))
            {
                foreach(var m in person.Moods)
                {
                    if (Equals(m.Name, moodName))
                    {
                        mood = m;
                        break;
                    }
                }
            }
            
            if(mood == null)
            {
                mood = defaultMood;
            }

            if(!tag.Attributes.TryGetValue("Name",out string name))
                name = person.Name;


            SetMood(mood, defaultMood, name, isRight);
            SetFocus(isRight);

            //if (!tag.IsAttribute("Page","false"))
            //    ClearText();

        }

        private void SetDefaultMood(Person person,bool isRight)
        {
            Mood defaultMood = person.Moods.FirstOrDefault();
            SetMood(defaultMood, defaultMood, person.Name, isRight);
        }

        private void SetMood(Mood mood,Mood defaultMood,string name,bool isRight)
        {
            if(mood != null && defaultMood != null){
                (isRight ? characterImageRight : characterImageLeft).sprite = (mood.Picture != null ? mood.Picture : defaultMood.Picture);
                currentVoice = (mood.Voice != null ? mood.Voice : defaultMood.Voice);
            }

            (isRight ? nameFieldRight : nameFieldLeft).text = name;
        }

        #endregion

        #region Text

        protected override void SetText(string text)
        {
            dialogField.text = text;
        }

        protected override void OnAddFractionChar(char c)
        {
            currentVoice?.PlayChar(c);
        }
        #endregion

        #region Focus Helper
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

        private void FadeFocus(Image image, TMP_Text textField, bool focus)
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
        #endregion
    }
}
