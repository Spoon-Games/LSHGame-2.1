using DG.Tweening;
using SceneM;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SpeechBubble : Singleton<SpeechBubble>
    {
        private CanvasGroup canvasGroup;

        [SerializeField]
        private float typeSpeed = 1;

        [Header("References")]
        [SerializeField]
        private TMP_Text textField;

        [SerializeField]
        private Button nextButton;

        private Dialog currantDialog;

        public override void Awake()
        {
            base.Awake();

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            nextButton.onClick.AddListener(GetNext);
        }

        public void SetDialog(Dialog dialog)
        {
            currantDialog = dialog;
            dialog.Reset();
            canvasGroup.DOKill(true);

            textField.DOKill(true);
            textField.SetText("");
            canvasGroup.DOFade(1, 1).SetEase(Ease.OutQuad).OnComplete(GetNext);

            RectTransform r;
            
            Debug.Log("Fade in");
        }

        private void GetNext()
        {
            textField.DOComplete();
            textField.DOKill(true);

            if (currantDialog.GetNext(out string text))
                textField.DOTypeWritePerSpeed(text, typeSpeed);
            else
            {
                canvasGroup.DOFade(0, 1).SetEase(Ease.InQuad);
                textField.DOComplete();
                Debug.Log("Fade Out");
            }
        }
    } 
}
