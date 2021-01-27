using DG.DOTweenEditor;
using DG.Tweening;
using LSHGame.Util;
using System;
using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(TweenModule),editorForChildClasses:true)]
    public class TweenEditor : UnityEditor.Editor
    {
        private TweenModule TweenModule => target as TweenModule;
        private bool needsReset = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!needsReset && GUILayout.Button("Play Tween"))
            {
                try
                {
                    Tween tween = TweenModule.GenerateTween();
                    //Tween tween = TweenModule.transform.DOMoveX(2, 1);
                    
                    DOTweenEditorPreview.PrepareTweenForPreview(tween);
                    DOTweenEditorPreview.Start();
                    needsReset = true;
                }
                catch (Exception e)
                {
                    Debug.Log("Could not preview: " + e);
                }
            }
            if(needsReset && GUILayout.Button("Reset Tween"))
            {
                //TweenModule.Tween.Rewind();
                //TweenModule.Tween.Kill();
                DOTweenEditorPreview.Stop(true);
                needsReset = false;
            }
        }
    } 
}
