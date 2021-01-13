using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogicC
{
    public class LogicGraphView : GraphView
    {
        private List<LogicNode> logicNodes = new List<LogicNode>();
        internal SceneView SceneView { get; private set; }
        private LogicSelectionDragger selectionDragger;
        private GridBackground gridBackground;
        private LogicSearchWindow searchWindow;

        internal float CameraDistanceZoom { get; set; }
        internal GameObject CurrentSelected { get
            {
                foreach(var s in selection)
                {
                    if(s is LogicNode n)
                    {
                        return n.connection.gameObject;
                    }
                }
                return null;
            } }

        public LogicGraphView(SceneView sceneView,bool drawGrid = false)
        {
            this.SceneView = sceneView;
            
            StyleSheet styleSheet = Resources.Load<StyleSheet>("LogicGraphUSS");
            if (styleSheet == null)
                Debug.Log("Cant find styleSheet");
            styleSheets.Add(styleSheet);
            //this.AddManipulator(new ContentDragger());
            selectionDragger = new LogicSelectionDragger();
            selectionDragger.panSpeed = Vector2.one;

            this.AddManipulator(selectionDragger);

            gridBackground = new GridBackground();
            Insert(0, gridBackground);
            gridBackground.StretchToParentSize();
            gridBackground.visible = drawGrid;
            gridBackground.style.display = drawGrid ? DisplayStyle.Flex : DisplayStyle.None;

            viewTransform.position = new Vector3(100, 100, 0);

            AddSearchWindow();
            //this.AddManipulator(new RectangleSelector());



            this.StretchToParentSize();
            this.pickingMode = PickingMode.Ignore;
            sceneView.rootVisualElement.Add(this);
        }

        internal void UpdateGraphView(Connection[] connectionsArray)
        {
            //List<LogicNode> oldNodes = new List<LogicNode>(logicNodes.Where(l => !connections.Contains(l.connection)));
            //foreach (LogicNode l in oldNodes)
            //    DestroyNode(l);
            var connections = connectionsArray.Where(c => IsVisible(c.transform.position));

            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                connections = connections.Where(c => stage.IsPartOfPrefabContents(c.gameObject));
                
            }

            var removeLogicNodes = new List<LogicNode>(logicNodes.Where(l => l.Destroy || !connections.Any(c => c.Equals(l.connection))));
            foreach (LogicNode l in removeLogicNodes)
            {
                DestroyNode(l);
            }

            var newConnections = connections.Where(c => !logicNodes.Exists(l => l.connection.Equals(c)));
            foreach (Connection c in newConnections)
                GenerateNode(c);

            DeleteEdges();
            AddEdges();

            if (selectionDragger.IsActive)
            {

                foreach (LogicNode l in logicNodes)
                {
                    if (!l.selected)
                        l.UpdatePosition();
                    else
                        l.UpdatePositionDrag();
                }
            }
            else
            {
                foreach (LogicNode l in logicNodes)
                {
                    l.UpdatePosition();
                }
            }

            viewTransform.position = Vector3.zero;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(p =>
            {
                bool isCompatible = !p.Equals(startPort);
                isCompatible &= !p.node.Equals(startPort.node);
                isCompatible &= p.direction != startPort.direction;
                isCompatible &= p.direction == Direction.Input && BasePort.IsCompatible(p.portType, startPort.portType) ||
                    p.direction == Direction.Output && BasePort.IsCompatible(startPort.portType, p.portType);
                isCompatible &= !p.connections.Any(e => Equals(e.input, startPort) || Equals(e.output, startPort));
                if (isCompatible)
                {
                    compatiblePorts.Add(p);
                }
            });
            return compatiblePorts;
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            if(selectable is LogicNode logicNode)
            {
                Selection.activeGameObject = logicNode.connection.gameObject;
            }
        }

        private void GenerateNode(Connection connection)
        {
            LogicNode node = new LogicNode(connection, this);
            logicNodes.Add(node);
            AddElement(node);
        }

        private void DestroyNode(LogicNode node)
        {
            node.WasRemoved = true;
            RemoveElement(node);
            logicNodes.Remove(node);
        }

        internal void Destroy()
        {
            SceneView.rootVisualElement.Remove(this);
        }

        internal Vector2 GetPosition(Vector3 position, out float distance)
        {
            Vector2 pos = SceneView.camera.WorldToScreenPoint(position);
            pos.y = viewport.contentRect.size.y - pos.y;
            distance = GetDistanceScreen(position);
            return pos;
        }

        private float GetDistanceScreen(Vector3 position)
        {
            float distance = 0;
            if (!SceneView.camera.orthographic)
                distance = (position - SceneView.camera.transform.position).magnitude / 5;
            else
                distance = SceneView.camera.orthographicSize;
            distance *= Mathf.Pow(2, -CameraDistanceZoom) / 1.5f;
            //Debug.Log("Screenposition: " + pos);
            return distance;
        }

        internal void DrawGrid(bool visible)
        {
            gridBackground.visible = visible;
        }

        private void DeleteEdges()
        {

            List<Edge> removeEdges = new List<Edge>();
            edges.ForEach(e =>
           {
               LogicPort li = e.input as LogicPort;
               LogicPort lo = e.output as LogicPort;
               //BasePort input = (e.input is LogicPort li) ? li.basePort : null;
               //BasePort output = (e.output is LogicPort lo) ? lo.basePort : null;
                
               if (li == null || lo == null || !BasePort.Connected(li.basePort, lo.basePort) || (li.node as LogicNode).WasRemoved || (lo.node as LogicNode).WasRemoved)
               {
                   if (li != null && li.IsEdgeDragging(e))
                       return;
                   if (lo != null && lo.IsEdgeDragging(e))
                       return;
                   if (!removeEdges.Contains(e))
                       removeEdges.Add(e);
                   return;
               }
           });

            RemoveEdges(removeEdges);
        }
        private void RemoveEdges(List<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                e.input?.Disconnect(e);
                e.output?.Disconnect(e);
                RemoveElement(e);

            }
        }

        private void AddEdges()
        {
            List<KeyValuePair<BasePort, LogicPort>> connectables = new List<KeyValuePair<BasePort, LogicPort>>();
            ports.ForEach(p =>
           {
               LogicPort output = p as LogicPort;

               if (output.direction == Direction.Output)
                   foreach (BasePort basePort in output.basePort.GetReferences())
                   {
                       if (!output.connections.Any(e => Equals(basePort, (e.input as LogicPort).basePort)))
                       {
                           connectables.Add(new KeyValuePair<BasePort, LogicPort>(basePort, output));
                       }
                   }
           });

            //foreach (var pair in connectables)
            //{
            //    Debug.Log("Connect In: " + pair.Key.Name + " Out: " + pair.Value.basePort.Name);
            //}

            ports.ForEach(p =>
            {
                LogicPort inputPort = p as LogicPort;
                if (inputPort.direction == Direction.Input)
                {

                    for (int i = 0; i < connectables.Count; i++)
                    {
                        var pair = connectables[i];
                        if (pair.Key.Equals(inputPort.basePort))
                        {
                            Edge e = new Edge()
                            {
                                input = inputPort,
                                output = pair.Value
                            };

                            inputPort.Connect(e);
                            pair.Value.Connect(e);
                            Add(e);
                            connectables.RemoveAt(i);
                            i--;
                        }
                    }
                }
            });
        }

        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<LogicSearchWindow>();
            searchWindow.Init(this, SceneView);
            nodeCreationRequest = OnNodeCreateionRequest;
        }

        internal void RequestSearchWindow(Vector2 screenMousePosition)
        {
            NodeCreationContext context = new NodeCreationContext();
            context.screenMousePosition = screenMousePosition;
            this.nodeCreationRequest.Invoke(context);
        }

        internal void ShowOriginLines(bool visible)
        {
            logicNodes.ForEach(n => n.originLine.visible = visible);
        }

        private void OnNodeCreateionRequest(NodeCreationContext context)
        {
            if (Selection.activeGameObject != null)
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
            else
                EditorUtility.DisplayDialog("Error Creating Node", "Can not create a logic node, because no gameobject is selected.", "OK");
        }

        public override EventPropagation DeleteSelection()
        {
            //List<Edge> removeEdges = new List<Edge>();
            foreach(var selected in this.selection)
            {
                if(selected is Edge edge)
                {
                    DisconnectEdge(edge);
                    //removeEdges.Add(edge);
                }else if(selected is LogicNode logicNode)
                {
                    Object.DestroyImmediate(logicNode.connection);
                }
            }
            //RemoveEdges(removeEdges);
            return EventPropagation.Stop;
        }

        private void DisconnectEdge(Edge e)
        {
            LogicPort li = e.input as LogicPort;
            LogicPort lo = e.output as LogicPort;
            if (li == null || lo == null)
                return;
            BasePort.Disconnect(li.basePort,lo.basePort);
        }

        private bool IsVisible(Vector3 point)
        {
            Vector3 screenPoint = SceneView.camera.WorldToViewportPoint(point);
            float distance = 1 / GetDistanceScreen(point);
            return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 &&  distance > 0.25f;
        }
    }

    internal class LogicSelectionDragger : SelectionDragger
    {
        public bool IsActive => m_Active;
        public LogicSelectionDragger()
        {

        }
    }

}
