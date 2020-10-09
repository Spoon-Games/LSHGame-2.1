using UnityEditor;
using UnityEngine;

namespace PathC
{
    [CustomEditor(typeof(LinearPath),true)]
    public class LinearPathEditor : EditablePathEditor<LinearPath>
    {
        protected override float GetDistanceOfPointToSegment(int segmentIndex, Vector2 point)
        {
            pathBehaviour.GetSegment(segmentIndex, out Vector2 p1, out Vector2 p2);
            return HandleUtility.DistancePointLine(point, p1, p2);
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void Draw()
        {
            base.Draw();

            for (int i = 0; i < pathBehaviour.SegmentCount; i++)
            {
                pathBehaviour.GetSegment(i, out Vector2 p1, out Vector2 p2);

                Handles.color = ((i == selectedSegementIndex) && Event.current.shift) ? Color.red : Color.green;
                Handles.DrawLine(p1, p2);
            }
        }
    }
}
