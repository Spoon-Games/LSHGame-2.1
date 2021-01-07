using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviourT.Editor
{
    internal enum DummyVerticalPortType { foo }

    public abstract class NodeView<T> : NodeView,IInspectable where T : BehaviourT.Node
    {
        private readonly Vector2 defaultSize = new Vector2(100, 150);

        public readonly T NodeData;

        private VisualElement inspectorView = new VisualElement();

        public NodeView(T nodeData, Vector2 position)
        {
            this.style.backgroundColor = new Color(0, 0, 0, 40);
            NodeData = nodeData;

            SetPosition(new Rect(position, defaultSize));

            foreach (BasePort basePort in NodeData.PortList)
            {
                GenerateLogicPort(basePort);
            }

            DrawNodeFields();

            RefreshExpandedState();
            RefreshPorts();
        }

        internal override Node Serialize()
        {
            List<SerInputPort> serInputPorts = new List<SerInputPort>();
            foreach (LogicPort p in inputContainer.Query<LogicPort>().ToList())
            {
                if (p.TrySerialize(out SerInputPort serPort))
                {
                    serInputPorts.Add(serPort);
                }
            }
            NodeData.SetSerializedInputPorts(serInputPorts.ToArray());
            NodeData.editorPos = GetPosition().position;
            return NodeData;
        }

        internal override void Deserialize(List<NodeView> nodeViews, TreeGraphView graphView)
        {
            foreach (var serInputPort in NodeData.GetSerializedInputPorts())
            {
                if (serInputPort.outputPortNode == null || string.IsNullOrEmpty(serInputPort.outputPortName))
                    continue;

                NodeView connectToView = nodeViews.FirstOrDefault(nv => Equals(nv.GetNodeData(), serInputPort.outputPortNode));
                if(connectToView != null)
                {
                    LogicPort input = GetPort(serInputPort.portName,this.inputContainer);
                    LogicPort output = GetPort(serInputPort.outputPortName, connectToView.outputContainer);
                    graphView.CreateEdge(input, output);
                }
            }
        }

        internal static LogicPort GetPort(string portName, VisualElement container)
        {
            List<LogicPort> ports = container.Query<LogicPort>().ToList();
            return ports.FirstOrDefault(p => Equals(p.basePort.Name, portName));
        }

        internal static FlowPort GetFlowPort(Direction direction, NodeView nodeView)
        {
            List<FlowPort> ports = nodeView.Query<FlowPort>().ToList();
            return ports.FirstOrDefault(p => p.direction == direction);
        }

        internal override Node GetNodeData()
        {
            return NodeData;
        }

        internal override void RuntimeUpdate()
        {
            base.RuntimeUpdate();
            if(NodeData is Task task)
            {
                switch (task.State)
                {
                    case TaskState.NotEvaluated:
                        this.style.backgroundColor = new Color(0, 0, 0, 40);
                        break;
                    case TaskState.Running:
                        this.style.backgroundColor = Color.blue;
                        break;
                    case TaskState.Failure:
                        this.style.backgroundColor = Color.red;
                        break;
                    case TaskState.Success:
                        this.style.backgroundColor = Color.green;
                        break;
                }
            }
        }

        #region Logic Ports

        internal FlowPort CreateFlowPort(Direction direction, Capacity capacity)
        {
            FlowPort portView = new FlowPort(NodeData, direction, capacity);

            if (direction == Direction.Input)
                this.Insert(0, portView);
            else
                mainContainer.Add(portView);

            return portView;
        }

        private void GenerateLogicPort(BasePort basePort)
        {
            LogicPort portView = new LogicPort(basePort, NodeData);

            if (basePort.GetDirection() == BasePort.Direction.input)
                inputContainer.Add(portView);
            else
                outputContainer.Add(portView);
        }

        private void DrawNodeFields()
        {
            inspectorView.StretchToParentSize();

            string title = NodeData.GetType().Name;
            AddComponentMenu titleAttribute = NodeData.GetType().GetCustomAttribute<AddComponentMenu>();
            if(titleAttribute != null)
            {
                title = titleAttribute.componentMenu;
                int splitI = title.LastIndexOf('/');
                if (splitI != -1 && splitI < title.Length-1)
                {
                    title = title.Substring(splitI+1);
                }
            }

            this.title = title;
            Label inspectorTitle = new Label(title);
            inspectorTitle.AddToClassList("inspector-title");
            inspectorView.Add(inspectorTitle);

            Dictionary<string, NodeEditorField> markedFields = GetAttributes<NodeEditorField>(NodeData.GetType(), typeof(BehaviourT.Node));
            List<FieldInfo> serializedFields = GetAttributesList<SerializeField>(NodeData.GetType(), typeof(Node));

            foreach (FieldInfo fieldInfo in serializedFields)
            {
                VisualElement container = null;
                string elementName = fieldInfo.Name;

                if (markedFields.TryGetValue(fieldInfo.Name, out NodeEditorField attribute))
                {
                    switch (attribute.nodePlace)
                    {
                        case NodeEditorField.NodePlace.TitleContainer:
                            container = titleContainer;
                            break;
                        case NodeEditorField.NodePlace.MainContainer:
                            container = mainContainer;
                            break;
                        case NodeEditorField.NodePlace.PortContainer:
                            container = Q<LogicPort>(inputContainer, p => Equals(p.basePort.Name, attribute.portContainer));
                            if (container == null)
                                container = Q<LogicPort>(outputContainer, p => Equals(p.basePort.Name, attribute.portContainer));
                            if (container == null)
                            {
                                Debug.LogWarning("Port " + attribute.portContainer + " was not found for default value");
                                container = mainContainer;
                            }
                            break;
                        case NodeEditorField.NodePlace.Hide:
                            continue;
                    }

                    if(attribute.label != null)
                        elementName = attribute.label;
                }

                Action<object> valueChanged = (o) => fieldInfo.SetValue(NodeData, o);
                

                VisualElement viewElement = null;
                if (container != null)
                {
                    viewElement = ElementFieldConverter.GetVisualElement(fieldInfo.FieldType,
                        fieldInfo.GetValue(NodeData), elementName, valueChanged,ref valueChanged);

                    //valueChanged += (o) => getValue?.Invoke(o);
                }


                VisualElement inspectorElement = ElementFieldConverter.GetVisualElement(fieldInfo.FieldType,
                    fieldInfo.GetValue(NodeData), elementName, valueChanged,ref valueChanged);
                //Debug.Log("GetValue 2: " + (getValue2 != null));
                //getValue2?.Invoke("Hallo");
                //if (getValue2 != null)
                //{
                //    valueChanged += (o) => { getValue2?.Invoke(o); Debug.Log("Invoke GetValue 2"); };
                //}

                if(viewElement != null)
                    viewElement.style.alignSelf = Align.Stretch;
                inspectorElement.style.alignSelf = Align.Stretch;

                if (attribute != null)
                {
                    if (viewElement != null)
                    {
                        if (attribute.hideLabel)
                        {
                            viewElement.AddToClassList("hide-label");
                        }
                        if (attribute.minWidth != -1)
                        {
                            viewElement.style.minWidth = attribute.minWidth;
                        }
                        if(attribute.nodePlace == NodeEditorField.NodePlace.TitleContainer)
                        {
                            viewElement.AddToClassList("simple-text-field");
                        }
                        if (attribute.nodePlace == NodeEditorField.NodePlace.PortContainer)
                            viewElement.style.flexGrow = 2;
                        if (!attribute.editable)
                            viewElement.focusable = false;
                    }
                    if (!attribute.editable)
                        inspectorElement.focusable = false;
                }

                container?.Add(viewElement);
                inspectorView.Add(inspectorElement);
            }
        }

        private Dictionary<string, C> GetAttributes<C>(Type type, Type baseType) where C : Attribute
        {
            Dictionary<string, C> markedFields = new Dictionary<string, C>();

            FieldInfo[] objectFields = GetAllFields(type, baseType);
            // search all fields and find the attribute [Position]
            for (int i = 0; i < objectFields.Length; i++)
            {

                // if we detect any attribute print out the data.
                if (Attribute.GetCustomAttribute(objectFields[i], typeof(C), true) is C attribute)
                {
                    markedFields[objectFields[i].Name] = attribute;
                }
            }

            return markedFields;
        }

        private List<FieldInfo> GetAttributesList<C>(Type type, Type baseType) where C : Attribute
        {
            List<FieldInfo> markedFields = new List<FieldInfo>();

            FieldInfo[] objectFields = GetAllFields(type, baseType);
            // search all fields and find the attribute [Position]
            for (int i = 0; i < objectFields.Length; i++)
            {

                // if we detect any attribute print out the data.
                if (Attribute.GetCustomAttribute(objectFields[i], typeof(C), true) is C attribute)
                {
                    if (!markedFields.Any(f => f.Name == objectFields[i].Name))
                        markedFields.Add(objectFields[i]);
                }
            }

            return markedFields;
        }

        //private Dictionary<O, T> GetAttributes<O,T>(Type type, Type baseType) where T : Attribute
        //{
        //    Dictionary<O, T> markedFields = new Dictionary<O, T>();

        //    FieldInfo[] objectFields = GetAllFields(type, baseType);
        //    // search all fields and find the attribute [Position]
        //    for (int i = 0; i < objectFields.Length; i++)
        //    {
        //        if (Attribute.GetCustomAttribute(objectFields[i], typeof(T), true) is T attribute)
        //        {
        //            if (objectFields[i] is O field)
        //                markedFields.Add(field, attribute);
        //        }
        //    }
        //    return markedFields;
        //}

        private static FieldInfo[] GetAllFields(Type type, Type baseType)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>(
                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            while (type.BaseType != typeof(object) && type.BaseType != baseType)
            {
                type = type.BaseType;
                FieldInfo[] f = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var fi in f)
                {
                    if (!fieldInfos.Contains(fi))
                        fieldInfos.Add(fi);
                }
            }
            return fieldInfos.ToArray();
        }

        private static C Q<C>(VisualElement container, Func<C, bool> func) where C : VisualElement
        {
            foreach (var ve in container.Children())
            {
                if (ve is C res && func.Invoke(res))
                    return res;
            }
            return null;
        }

        public VisualElement GetInspectorView()
        {
            return inspectorView;
        }



        #endregion
    }

    public abstract class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        internal abstract Node GetNodeData();

        internal abstract Node Serialize();

        internal abstract void Deserialize(List<NodeView> nodeViews,TreeGraphView graphView);

        internal virtual void RuntimeUpdate() { }
    }

    #region Ports
    internal class LogicPort : Port
    {
        internal readonly BasePort basePort;
        internal readonly Node nodeParent;

        public LogicPort(BasePort basePort, Node nodeParent) : base(Orientation.Horizontal,
                basePort.GetDirection() == BasePort.Direction.input ? Direction.Input : Direction.Output,
                basePort.GetDirection() == BasePort.Direction.input ? Capacity.Single : Capacity.Multi,
                basePort.GetPortType())
        {

            portName = basePort.Name;
            this.basePort = basePort;
            this.nodeParent = nodeParent;

            this.AddManipulator(new EdgeConnector<Edge>(new EC()));

        }

        public bool TrySerialize(out SerInputPort serInputPort)
        {
            if (direction == Direction.Output)
            {
                serInputPort = null;
                return false;
            }
            else
            {
                BehaviourT.Node outputNode = null;
                string outputPortName = null;

                if (connected)
                {
                    if (connections.First().output is LogicPort output)
                    {
                        outputNode = output.nodeParent;
                        outputPortName = output.basePort.Name;
                    }
                }
                serInputPort = new SerInputPort(outputNode, outputPortName, basePort.Name);
                return true;
            }
        }
    }

    internal class FlowPort : Port
    {
        internal readonly Node nodeParent;

        public FlowPort(Node nodeParent, Direction direction, Capacity capacity) : base(Orientation.Vertical, direction, capacity, typeof(DummyVerticalPortType))
        {
            this.nodeParent = nodeParent;
            name = "";
            Label label = this.Q<Label>();
            label.visible = false;
            label.style.display = DisplayStyle.None;

            this.style.alignSelf = Align.Center;
            this.style.alignContent = Align.Center;
            this.style.alignItems = Align.Center;
            this.style.justifyContent = Justify.Center;

            this.AddManipulator(new EdgeConnector<Edge>(new EC()));
        }

        public bool TrySerialize(out TaskArray taskArray)
        {
            if (direction == Direction.Input)
            {
                taskArray = null;
                return false;
            }
            else
            {
                SortedList<float, Task> tasks = new SortedList<float, Task>();

                foreach (var c in connections)
                {
                    if (c.input is FlowPort input && input.nodeParent is Task task)
                    {

                        float pos = c.input.GetGlobalCenter().x;
                        while (tasks.ContainsKey(pos))
                            pos++;

                        tasks.Add(pos, task);
                    }
                }
                taskArray = new TaskArray(tasks.Values.ToArray());
                return true;
            }
        }

        public bool TrySerialize(out Task task)
        {
            if (direction == Direction.Input)
            {
                task = null;
                return false;
            }
            else
            {
                foreach (var c in connections)
                {
                    if (c.input is FlowPort input && input.nodeParent is Task t)
                    {

                        task = t;
                        return true;

                    }
                }

                task = null;
                return false;
            }
        }
    }

    internal class EC : IEdgeConnectorListener
    {
        public void OnDrop(GraphView graphView, Edge edge)
        {
            edge.output.Connect(edge);
            edge.input.Connect(edge);
            graphView.AddElement(edge);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {

        }
    }
}

#endregion