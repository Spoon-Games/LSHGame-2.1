using UnityEditor;
using UnityEngine;
using UnityEditor.Audio;

namespace AudioP
{
    [CustomEditor(typeof(SoundInfo),true)]
    public class SoundInfoEditor : Editor
    {
        private SoundInfo SoundInfo => target as SoundInfo;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if (GUILayout.Button("Preview"))
            {
                
            }
        }
    } 
}
