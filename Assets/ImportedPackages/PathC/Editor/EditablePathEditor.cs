using UnityEditor;
using UnityEngine;

namespace PathC
{
    public abstract class EditablePathEditor<T> : Editor where T : EditablePathBehaviour
    {
        private const float segmentSelectDistanceThreshold = .1f;
        protected int selectedSegementIndex = -1;

        protected T pathBehaviour;

        private bool drawVertex = false;

        protected virtual void OnSceneGUI()
        {
            Input();
            Draw();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Reset"))
            {
                Undo.RecordObject(pathBehaviour, "Reset path");
                pathBehaviour.Reset();
            }

            bool nIsClosed = GUILayout.Toggle(pathBehaviour.IsClosed, "Is Closed");
            if (nIsClosed != pathBehaviour.IsClosed)
            {
                Undo.RecordObject(pathBehaviour, "Toggle is closed");
                pathBehaviour.IsClosed = nIsClosed;
            }

            drawVertex = GUILayout.Toggle(drawVertex, "Draw vertecies");

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

        }

        protected virtual void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                if (selectedSegementIndex != -1)
                {
                    Undo.RecordObject(pathBehaviour, "Insert segment");
                    pathBehaviour.InsertSegment(selectedSegementIndex, mousePos);
                }
                else if (!pathBehaviour.IsClosed)
                {
                    Undo.RecordObject(pathBehaviour, "Add segment");
                    pathBehaviour.AddSegment(mousePos);
                }
            }

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 || guiEvent.keyCode == KeyCode.Delete)
            {
                float minDstToAnchor = .15f;
                int closestAnchorIndex = -1;

                for (int i = 0; i < pathBehaviour.PointCount; i++)
                {
                    float dst = Vector2.Distance(mousePos, pathBehaviour.GetPoint(i));
                    if (dst < minDstToAnchor)
                    {
                        minDstToAnchor = dst;
                        closestAnchorIndex = i;
                    }
                }

                if (closestAnchorIndex != -1)
                {
                    Undo.RecordObject(pathBehaviour, "Delete segment");
                    pathBehaviour.DeleteSegment(closestAnchorIndex);
                }
            }
            if (guiEvent.type == EventType.MouseMove)
            {
                float minDstTosegment = segmentSelectDistanceThreshold;
                int newSelectedSegementIndex = -1;

                for (int i = 0; i < pathBehaviour.SegmentCount; i++)
                {
                    float dst = GetDistanceOfPointToSegment(i, mousePos);
                    if (dst < minDstTosegment)
                    {
                        minDstTosegment = dst;
                        newSelectedSegementIndex = i;
                    }
                }

                if (newSelectedSegementIndex != selectedSegementIndex)
                {
                    selectedSegementIndex = newSelectedSegementIndex;
                    HandleUtility.Repaint();
                }
            }
        }

        protected virtual void Draw()
        {
            Handles.color = Color.red;
            for (int i = 0; i < pathBehaviour.PointCount; i++)
            {
                Vector2 newPos = Handles.FreeMoveHandle(pathBehaviour.GetPoint(i), Quaternion.identity, .15f, Vector2.zero, Handles.CylinderHandleCap);
                if (pathBehaviour.GetPoint(i) != newPos)
                {
                    Undo.RecordObject(pathBehaviour, "Move anchPoint");
                    pathBehaviour.MovePoint(i, newPos);
                }
            }

            if (drawVertex)
                for (int i = 0; i < pathBehaviour.VertexPath.Points.Length; i++)
                {
                    Vector2 pos = pathBehaviour.VertexPath.Points[i];
                    Handles.color = Color.grey;
                    Handles.CubeHandleCap(0, pos, Quaternion.identity, .05f, EventType.Repaint);
                }
        }

        

        protected abstract float GetDistanceOfPointToSegment(int segmentIndex, Vector2 point);

        protected virtual void OnEnable()
        {
            pathBehaviour = (T)target;
        }
    }
}
