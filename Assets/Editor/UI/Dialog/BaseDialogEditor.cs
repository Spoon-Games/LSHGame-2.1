using LSHGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(BaseDialog),true)]
    public class BaseDialogEditor : UnityEditor.Editor
    {
        private BaseDialog DialogHeader => target as BaseDialog;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Parse"))
            {
                Debug.Log(DialogHeader.Parse());
            }
        }
    } 
}
