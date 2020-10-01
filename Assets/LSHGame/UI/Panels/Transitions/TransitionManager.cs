using LSHGame.Util;
using SceneM;
using System;
using UnityEngine;

namespace LSHGame.UI
{
    public class TransitionManager : BasePanelManager<TransitionInfo,Transition,TransitionManager>
    {

        private void Start()
        {
            LevelManager.OnStartLoadingMainScene += OnStartLoadingMainScene;
        }


        private void OnStartLoadingMainScene(Func<float> getProgress,MainSceneInfo sceneInfo)
        {
            if(sceneInfo.Transition != null)
            {
                Transition transition = base.ShowPanel(sceneInfo.Transition);
                transition?.StartTransition(getProgress);
            }
        }

    } 
}
