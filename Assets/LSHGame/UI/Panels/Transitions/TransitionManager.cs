using SceneM;
using System;
using UINavigation;
using UnityEngine;

namespace LSHGame.UI
{
    public class TransitionManager : BasePanelManager<TransitionInfo, TransitionPanel, TransitionManager>
    {
        public static TransitionManager Instance;

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            base.Awake();
            
        }

        protected override void Start()
        {
            base.Start();
            LevelManager.OnStartLoadingMainScene += OnStartLoadingMainScene;
        }


        private void OnStartLoadingMainScene(Func<float> getProgress, TransitionInfo transition)
        {

            ShowTransition(transition, getProgress);

        }

        public void ShowTransition(TransitionInfo transInfo, Func<float> getProgress = null,Action onMiddle = null)
        {
            if (transInfo == null)
            {
                onMiddle?.Invoke();
                return;
            }
            TransitionPanel transition = base.ShowPanel(transInfo);
            transition?.StartTransition(getProgress);

            if(onMiddle != null)
            {
                TimeSystem.Delay(transInfo.StartDurration,f => onMiddle.Invoke());
            }
        }

    }
}
