using LSHGame.Util;
using UnityEditor;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(MatBounceSubProp))]
    public class MatBounceSubPropEditor : UnityEditor.Editor
    {
        private MatBounceSubProp matBounceSubProp;

        private void Awake()
        {
            matBounceSubProp = target as MatBounceSubProp;
        }

        public override void OnInspectorGUI()
        {
            var s = matBounceSubProp.BounceSettings;

            s.IsRelativeSpeed = EditorGUILayout.Toggle("Is Relative Speed", s.IsRelativeSpeed);
            if (s.IsRelativeSpeed)
            {
                s.BounceDamping = EditorGUILayout.Slider("Bounce Damping", s.BounceDamping, 0, 3);
                s.MinMaxBounceSpeed.x = EditorGUILayout.FloatField("Min Speed", s.MinMaxBounceSpeed.x);
                if (s.MinMaxBounceSpeed.x < 0)
                    s.MinMaxBounceSpeed.x = -1;

                s.MinMaxBounceSpeed.y = EditorGUILayout.FloatField("Max Speed", s.MinMaxBounceSpeed.y);
                if (s.MinMaxBounceSpeed.y < 0)
                    s.MinMaxBounceSpeed.y = -1;
                else if (s.MinMaxBounceSpeed.x > s.MinMaxBounceSpeed.y)
                    s.MinMaxBounceSpeed.y = s.MinMaxBounceSpeed.x;
            }
            else
            {
                s.BounceSpeed = EditorGUILayout.FloatField("Bounce Speed", s.BounceSpeed);
            }

            s.FixedRotation = EditorGUILayout.Toggle("Is Fixed Rotation", s.FixedRotation);
            if (!s.FixedRotation)
            {
                s.MinMaxRotation.x = EditorGUILayout.FloatField("Min Rotation", s.MinMaxRotation.x);
                if (s.MinMaxRotation.x < -360)
                    s.MinMaxRotation.x = -1000;
                else if (s.MinMaxRotation.x > 360)
                    s.MinMaxRotation.x %= 360;

                s.MinMaxRotation.y = EditorGUILayout.FloatField("Max Rotation", s.MinMaxRotation.y);
                if (s.MinMaxRotation.y < -360)
                    s.MinMaxRotation.y = -1000;
                else if (s.MinMaxRotation.y > 360)
                    s.MinMaxRotation.y %= 360;
                else if (s.MinMaxRotation.x > s.MinMaxRotation.y)
                    s.MinMaxRotation.y = s.MinMaxRotation.x;
            }
            else
            {
                s.Rotation = EditorGUILayout.FloatField("Rotation", s.Rotation) % 360;
            }

            matBounceSubProp.AddGameObjectRotation = EditorGUILayout.Toggle("AddGameObjectRotation", matBounceSubProp.AddGameObjectRotation);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnBounceEvent"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
