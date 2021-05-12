using LSHGame.Util;
using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(RecreateManager))]
    public class RecreateManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Load"))
            {
                if(target is RecreateManager m)
                {
                    m.LoadPrefabs();
                }
            }
        }
    }
}
