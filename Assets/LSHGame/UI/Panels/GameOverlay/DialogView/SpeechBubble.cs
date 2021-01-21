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

        [SerializeField]
        private float delayOfCompletion = 1;

        [Header("References")]
        [SerializeField]
        private TMP_Text textField;

        [SerializeField]
        private Button nextButton;

        private Dialog currantDialog;

        private Tween typewriteTween;

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
            
        }

        private void GetNext()
        {
            bool wasActive = typewriteTween != null ?  typewriteTween.active : false;
            //textField.DOKill(false);
            typewriteTween?.Kill(true);

            if (currantDialog.GetNext(out string text))
                typewriteTween = textField.DOTypeWritePerSpeed(text, typeSpeed).SetDelay(wasActive?delayOfCompletion:0);
            else
            {
                canvasGroup.DOFade(0, 1).SetEase(Ease.InQuad).SetDelay(wasActive ? delayOfCompletion : 0);
                textField.DOComplete();
            }
        }
    } 
}
