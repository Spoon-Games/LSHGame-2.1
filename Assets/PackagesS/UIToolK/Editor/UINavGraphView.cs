using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UINavigation.Editor
{
    public class UINavGraphView : GraphView
    {
        private UINavSearchWindow searchWindow;
        private UINavEditor editor;

        public UINavGraphView(UINavEditor editor)
        {
            this.editor = editor;

            name = "UI Navigation Graph";

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.StretchToParentSize();

            //styleSheets.Add(Resources.Load<StyleSheet>("LSMGraphUSS"));
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow();

            new NavNestedEndNode(this, new Vector2(0, 200));
            new NavStartNode(this, Vector2.zero);
        }

        internal void CreateNavStateNode(Vector2 position)
        {
            new NavStateNode(this, position);
        }

        internal void CreateNavNestedNode(Vector2 position)
        {
            new NavNestedNode(this, position);
        }


        internal void RemoveEdge(Edge edge)
        {
            edge.input?.Disconnect(edge);
            edge.output?.Disconnect(edge);
            RemoveElement(edge);
        }

        internal void CreateEdge(Port inputPort, Port outputPort)
        {
            Edge edge = new Edge
            {
                input = inputPort,
                output = outputPort
            };

            inputPort.Connect(edge);
            outputPort.Connect(edge);

            Add(edge);
        }

        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<UINavSearchWindow>();
            searchWindow.Init(this,editor);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition),searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(p =>
            {
                if (!p.Equals(startPort) && !p.node.Equals(startPort.node) && p.direction != startPort.direction &&
                p.portType.Equals(startPort.portType) && !p.connections.Any(e => Equals(e.input, startPort) || Equals(e.output, startPort)))
                {
                    compatiblePorts.Add(p);
                }
            });
            return compatiblePorts;
        }
    }
}
