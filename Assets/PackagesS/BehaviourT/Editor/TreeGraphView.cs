using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourT.Editor
{
    public class TreeGraphView : GraphView
    {
        private BehaviourTreeEditorr editor;
        private TreeSearchWindow searchWindow;
        private VisualElement inspectorContainer;

        public TreeGraphView(BehaviourTreeEditorr editor, VisualElement inspectorContainer)
        {
            this.editor = editor;
            this.inspectorContainer = inspectorContainer;
            
            name = "BehaviourTreeGraph";

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //styleSheets.Add(Resources.Load<StyleSheet>("LSMGraphUSS"));
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow();

            AddNodeView(new Vector2(100, 100), typeof(RootTask));

            this.StretchToParentSize();
            this.graphViewChanged += (e) => { editor.IsUnsaved = true; return e; };
            
        }

        internal void RuntimeUpdate()
        {
            foreach(var node in nodes.ToList().Cast<NodeView>())
            {
                node.RuntimeUpdate();
            }
        }

        internal void AddNodeView(Vector2 pos,Type type)
        {
            GraphElement nodeView = null;
            if (type.IsSubclassOf(typeof(RootTask)))
            {
                nodeView = new RootTaskView((RootTask)Activator.CreateInstance(type), pos);

            }else if (type.IsSubclassOf(typeof(CompositeTask)))
            {
                nodeView = new CompositeTaskView((CompositeTask)Activator.CreateInstance(type), pos);

            }else if (type.IsSubclassOf(typeof(DecoratorTask)))
            {
                nodeView = new DecoratorTaskView((DecoratorTask)Activator.CreateInstance(type), pos);
            }
            else if (type.IsSubclassOf(typeof(Task)))
            {
                nodeView = new TaskNodeView((Task)Activator.CreateInstance(type), pos);

            }else if (type.IsSubclassOf(typeof(LogicNode)))
            {
                nodeView = new LogicNodeView((LogicNode)Activator.CreateInstance(type), pos);
            }

            if (nodeView != null)
                this.AddElement(nodeView);
        }

        internal void AddNodeView(Vector2 pos, Node node)
        {
            GraphElement nodeView = null;
            if (node is RootTask rootTask)
            {
                nodeView = new RootTaskView(rootTask, pos);
            }
            else if (node is CompositeTask compositeTask)
            {
                nodeView = new CompositeTaskView(compositeTask, pos);
            }
            else if (node is DecoratorTask decoratorTask)
            {
                nodeView = new DecoratorTaskView(decoratorTask, pos);
            }
            else if (node is Task task)
            {
                nodeView = new TaskNodeView(task, pos);
            }
            else if (node is LogicNode logicNode)
            {
                nodeView = new LogicNodeView(logicNode, pos);
            }

            if (nodeView != null)
                this.AddElement(nodeView);
        }

        internal void AddGroup(Vector2 pos)
        {
            AddElement(new GroupView("New Group", pos));
        }

        internal GroupView AddGroup(string title, Rect position)
        {
            GroupView groupView = new GroupView(title, position);
            AddElement(groupView);
            return groupView;
        }

        

        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<TreeSearchWindow>();
            searchWindow.Init(this, editor);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
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

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(p =>
            {
                if (!p.Equals(startPort) && !p.node.Equals(startPort.node) && p.direction != startPort.direction &&
                p.portType.Equals(startPort.portType) && !p.connections.Any(e => Equals(e.input, startPort) || Equals(e.output, startPort))
                && !(startPort.capacity == Port.Capacity.Single && startPort.connected) && !(p.capacity == Port.Capacity.Single && p.connected))
                {
                    compatiblePorts.Add(p);
                }
            });
            return compatiblePorts;
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            UpdateInspector();
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            UpdateInspector();
        }

        public override EventPropagation DeleteSelection()
        {
            UpdateInspector();
            return base.DeleteSelection();
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            UpdateInspector();
        }

        private void UpdateInspector()
        {
            ISelectable selectable = selection.FirstOrDefault((s) => s is IInspectable);
            inspectorContainer.Clear();
            if(selectable is IInspectable inspectable)
            {
                inspectorContainer.Add(inspectable.GetInspectorView());
            }
        }
    }

    public interface IInspectable : ISelectable
    {
        VisualElement GetInspectorView();
    }
}
