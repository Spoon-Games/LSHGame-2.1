using LSHGame.Util;
using UnityEditor;
using UnityEditor.U2D.IK;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(PlanetJointSolver))]
    [CanEditMultipleObjects]
    public class PlanetJointSolverEditor : Solver2DEditor
    {
        private static class Contents
        {
            public static readonly GUIContent effectorLabel = new GUIContent("Effector", "The last Transform of a hierarchy constrained by the target");
            public static readonly GUIContent targetLabel = new GUIContent("Target", "Transfrom which the effector will follow");
            public static readonly GUIContent circleTransformLabel = new GUIContent("Circle Transform");
            public static readonly GUIContent dampingLabel = new GUIContent("Damping");
            public static readonly GUIContent dampingThresholdLabel = new GUIContent("Damping Threashold");
        }

        private SerializedProperty m_ChainProperty;
        private SerializedProperty dampingProperty;
        private SerializedProperty circleTransformProperty;
        private SerializedProperty dampingThresholdProperty;

        private void OnEnable()
        {
            m_ChainProperty = serializedObject.FindProperty("chain");
            dampingProperty = serializedObject.FindProperty("damping");
            circleTransformProperty = serializedObject.FindProperty("circleTransform");
            dampingThresholdProperty = serializedObject.FindProperty("dampingThreshold");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ChainProperty.FindPropertyRelative("m_EffectorTransform"), Contents.effectorLabel);
            EditorGUILayout.PropertyField(m_ChainProperty.FindPropertyRelative("m_TargetTransform"), Contents.targetLabel);

            EditorGUILayout.PropertyField(circleTransformProperty, Contents.circleTransformLabel);
            EditorGUILayout.PropertyField(dampingProperty, Contents.dampingLabel);
            EditorGUILayout.PropertyField(dampingThresholdProperty, Contents.dampingThresholdLabel);

            DrawCommonSolverInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
