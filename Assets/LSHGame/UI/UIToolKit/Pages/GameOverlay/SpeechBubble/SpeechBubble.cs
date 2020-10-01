using SceneM;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace LSHGame.UI
{
    public class SpeechBubble : VisualElement
    {
        private Label mainText;
        private Button nextButton;

        private float typingSpeed;

        private string currentText = "";

        public new class UxmlFactory : UxmlFactory<SpeechBubble, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlFloatAttributeDescription typingSpeed = new UxmlFloatAttributeDescription { name = "Typeing Speed", defaultValue = 1f };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as SpeechBubble;
                ate.Clear();

                ate.typingSpeed = typingSpeed.GetValueFromBag(bag, cc);

                var tree = Resources.Load<VisualTreeAsset>("SpeechBubble"); 
                if (tree == null)
                    Debug.Log("Tree is null");
               
                ate.Add(tree.CloneTree());

                ate.mainText = ate.Q<Label>(name: "text-main");
                ate.mainText.text = "";

                ate.nextButton = ate.Q<Button>(name: "next-button");
                ate.nextButton.RegisterCallback<ClickEvent>(evt => ate.NextText());
            }
        }

        public void ShowText(string text)
        {
            currentText = text;
            NextText();
        }

        private void NextText()
        {
            if (string.IsNullOrEmpty(currentText))
            {
                visible = false;
                return;
            }

            this.visible = true;

            EvaluateText(currentText, out string current, out currentText);

            TimeSystem.Instance.StopCoroutine("TypewriteText");
            TimeSystem.Instance.StartCoroutine(TypewriteText(current));
        }

        private IEnumerator TypewriteText(string text)
        {
            mainText.text = "";
            float startTime = Time.time;
            int i = 0;
            while(i < text.Length)
            {
                i = (int) ((Time.time - startTime) * typingSpeed);
                mainText.text = text.Substring(0,Mathf.Min(i,text.Length-1));

                yield return null;
            }
        }

        private void EvaluateText(string text,out string current,out string rest)
        {
            int split = text.IndexOf("\n\n");
            current = text.Substring(0, split);
            rest = text.Substring(split + 2);
        }

    }

}