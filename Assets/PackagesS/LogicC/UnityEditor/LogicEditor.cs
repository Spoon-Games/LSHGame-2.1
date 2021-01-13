using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityToolbarExtender;

namespace LogicC
{
    public class LogicEditor : EditorWindow
    {
        private static List<LogicGraphView> graphViews = new List<LogicGraphView>();

        private static HashSet<Connection> connections = new HashSet<Connection>();

        private static bool graphViewEnabled = false;
        private static bool autoUpdate = true;
        private static bool showOriginLines = true;
        private static bool drawGrid = false;
        private static float zoom = 0;
        private static bool wasOnEnabled = false;

        [MenuItem("Window/Util/LogicEditor")]
        public static LogicEditor GetLogicEditor()
        {
            return GetWindow<LogicEditor>("LogicEditor");
        }

        private void OnGUI()
        {
            if (wasOnEnabled)
            {
                wasOnEnabled = false;
                EnableGraphView();
            }

            bool e = EditorGUILayout.Toggle("Nodes enabled", graphViewEnabled);
            if (e && !graphViewEnabled)
                EnableGraphView();
            if (!e && graphViewEnabled)
                DisableGraphView();

            e = EditorGUILayout.Toggle("GridBackground", drawGrid);
            if(e != drawGrid)
            {
                graphViews.ForEach(g => g.DrawGrid(e));
                drawGrid = e;
            }

            e = EditorGUILayout.Toggle("Draw lines to origins", showOriginLines);
            if(e != showOriginLines)
            {
                graphViews.ForEach(g => g.ShowOriginLines(e));
                showOriginLines = e;
            }

            zoom = EditorGUILayout.Slider("Node size zoom",zoom, -4f, 4f);
            graphViews.ForEach(g => g.CameraDistanceZoom = zoom);
           
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {

                LogicGraphView graphView = graphViews.FirstOrDefault(g => g.SceneView == sceneView);
                if(graphView != null)
                {
                    graphView.RequestSearchWindow(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
                }
            }

            if (Event.current.type == EventType.Repaint)
            {

                LogicGraphView graphView = graphViews.FirstOrDefault(g => g.SceneView == sceneView);
                if (graphView != null)
                {
                    graphView.UpdateGraphView(connections.ToArray());
                }
            }
        }

        private static void EnableGraphView()
        {
            DisableGraphView();

            foreach (SceneView sceneView in SceneView.sceneViews)
            {
                graphViews.Add(new LogicGraphView(sceneView, drawGrid));
                //VisualElement visualElement = new VisualElement();
                //visualElement.StretchToParentSize();

                //visualElement.Add(new Button() { text = "testButton" });
                //VisualElement testElement = new VisualElement();
                //testElement.style.backgroundColor = new Color(255, 0, 0, 64);
                //testElement.style.opacity = 0.3f;
                //testElement.StretchToParentSize();
                //testElement.pickingMode = PickingMode.Ignore;
               
                //visualElement.Add(testElement);

                //sceneView.rootVisualElement.Add(visualElement);
            }
            SceneView.duringSceneGui += OnSceneGUI;
            graphViewEnabled = true;
        }

        private void OnEnable()
        {
            wasOnEnabled = true;
        }

        private static void DisableGraphView()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            foreach (LogicGraphView graphView in graphViews)
                graphView.Destroy();

            graphViews.Clear();
            
            graphViewEnabled = false;
        }

        internal void SetSelected(GameObject g)
        {
            Selection.activeGameObject = g;
        }

        //private static List<T> GetComponentsInScene<T>() where T: MonoBehaviour
        //{
        //    //Debug.Log("GetComponentsInScene");
        //    List<T> result = new List<T>();
        //    foreach(GameObject g in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        //    {
        //        if (g == null)
        //            continue;
        //        AddComponentsAndChildren<T>(result, g.transform);
        //    }
        //    return result;
        //}

        //private static void AddComponentsAndChildren<T>(List<T> components, Transform t) where T : MonoBehaviour
        //{
        //    components.AddRange(t.GetComponents<T>());
        //    foreach (Transform child in t)
        //        AddComponentsAndChildren<T>(components, child);
        //}

        //private static IEnumerable<GameObject> SceneRoots()
        //{
        //    HierarchyProperty prop = new HierarchyProperty(HierarchyType.GameObjects);
        //    int[] expanded = new int[] { };
        //    while (prop.Next(expanded))
        //    {
        //        yield return prop.pptrValue as GameObject;
        //    }
        //}

        [InitializeOnLoadMethod]
        private static void InitConnectionRegistry()
        {
            Connection.OnConnectionEnabled += RegisterConnection;
            Connection.OnConnectionDisabled += UnregisterConnection;

            //PrefabStage.prefabStageClosing += OnLeavePrefabStage;
            //PrefabStage.prefabSaving += SavePrefabStage;

            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
                
        }

        private static void OnToolbarGUI()
        {
            GUILayout.Space(20);

            GUILayout.Space(8);

            var tex = EditorGUIUtility.IconContent(@"UnityEditor.SceneView").image;
            bool e = GUILayout.Toggle(graphViewEnabled, new GUIContent("LogicView",tex, "Toggle LogicView"),ToolbarStyles.commandButtonStyle);
            if (e && !graphViewEnabled)
                EnableGraphView();
            if (!e && graphViewEnabled)
                DisableGraphView();

            //GUILayout.FlexibleSpace();
        }

        private static void RegisterConnection(Connection connection)
        {
            if(!connections.Contains(connection))
                connections.Add(connection);
        }

        private static void UnregisterConnection(Connection connection)
        {
            connections.Remove(connection);

            //PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            //if(stage != null)
            //{
            //    if (stage.IsPartOfPrefabContents(connection.gameObject))
            //    {
            //        connection.enabled = false;
            //        Debug.Log("Leaving prefag stage connection");
            //    }
            //}
        }

    }

    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageLeft,
                fixedWidth = 90,
            };
        }
    }
}
