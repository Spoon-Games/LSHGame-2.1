using UnityEditor;
using UnityEngine;

namespace PathC
{
    [CustomEditor(typeof(BezierPath),true)]
    public class BezierPathEditor : EditablePathEditor<BezierPath>
    {
        private bool displayControlPoints = true;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            pathBehaviour.VertexResolution = EditorGUILayout.Slider("Vertex Resolution",pathBehaviour.VertexResolution, 0, 5);
            pathBehaviour.VertexSpacing = EditorGUILayout.Slider("Vertex Spacing", pathBehaviour.VertexSpacing, 0.05f, 5);

            bool nIsAutoConP = GUILayout.Toggle(pathBehaviour.IsAutoSetcontrolPoints, "Automatic control points");
            if (nIsAutoConP != pathBehaviour.IsAutoSetcontrolPoints)
            {
                Undo.RecordObject(pathBehaviour, "Toggle automatic control points");
                pathBehaviour.IsAutoSetcontrolPoints = nIsAutoConP;
            }

            displayControlPoints = GUILayout.Toggle(displayControlPoints, "Display control points");

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }

        protected override void Draw()
        {
            base.Draw();

            for (int i = 0; i < pathBehaviour.SegmentCount; i++)
            {
                pathBehaviour.GetSegment(i, out Vector2 anchP1, out Vector2 contP1, out Vector2 contP2, out Vector2 anchP2);
                if (!pathBehaviour.IsAutoSetcontrolPoints && displayControlPoints)
                {
                    Handles.color = Color.black;
                    Handles.DrawLine(contP1, anchP1);
                    Handles.DrawLine(contP2, anchP2);
                }
                Color segmentColor = ((i == selectedSegementIndex) && Event.current.shift) ? Color.red : Color.green;
                Handles.DrawBezier(anchP1, anchP2, contP1, contP2, segmentColor, null, 2);
            }

            if (!pathBehaviour.IsAutoSetcontrolPoints && displayControlPoints)
            {
                Handles.color = Color.white;
                for (int i = 0; i < pathBehaviour.ContPointCount; i++)
                {
                    Vector2 newPos = Handles.FreeMoveHandle(pathBehaviour.GetContPoint(i), Quaternion.identity, .1f, Vector2.zero, Handles.CylinderHandleCap);
                    if (pathBehaviour.GetContPoint(i) != newPos)
                    {
                        Undo.RecordObject(pathBehaviour, "Move contPoint");
                        pathBehaviour.MoveContPoint(i, newPos);
                    }
                }
            }

        }

        protected override float GetDistanceOfPointToSegment(int segmentIndex, Vector2 point)
        {
            pathBehaviour.GetSegment(segmentIndex, out Vector2 anchP1, out Vector2 contP1, out Vector2 contP2, out Vector2 anchP2);
            return HandleUtility.DistancePointBezier(point, anchP1, anchP2, contP1, contP2);
        }
    }
}
