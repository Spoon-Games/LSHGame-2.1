using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogicC
{
    public class LogicNode : Node
    {
        private Vector2 defaultSize = new Vector2(100, 150);

        public string GUID;

        internal Connection connection;

        private LogicGraphView nodeParent;

        internal bool Destroy => connection.IsDestroied;

        internal bool WasRemoved { get; set; } = false;

        private StyleSheet hideLabelOfPropertyFieldSheet;

        internal VisualElement originLine;

        public LogicNode(Connection connection, LogicGraphView nodeParent)
        {
            hideLabelOfPropertyFieldSheet = Resources.Load<StyleSheet>("PropertyField2USS");

            GUID = Guid.NewGuid().ToString();

            this.connection = connection;
            this.nodeParent = nodeParent;
            title = connection.Title + " - " + connection.gameObject.name;

            //capabilities &= ~Capabilities.Deletable;

            foreach (BasePort basePort in connection.BasePorts)
            {
                GeneratePort(basePort);
            }

            RefreshExpandedState();
            RefreshPorts();

            DrawNodeFields();
            CreateOriginLine();

            UpdatePosition();

        }



        internal void UpdatePosition()
        {
            if (connection.IsDestroied)
                return;
            Vector2 pos = nodeParent.GetPosition(connection.transform.position, out float distance);
            distance = Mathf.Clamp(1 / distance, 0.3f, 1);
            SetPosition(new Rect(pos + connection.nodeOffset * distance, defaultSize));

            Vector3 oldScale = transform.scale;
            transform.scale = Vector3.one * distance;

            if (oldScale != transform.scale)
                this.MarkDirtyRepaint();
        }

        internal void UpdatePositionDrag()
        {
            if (connection.IsDestroied)
                return;
            Vector2 pos = nodeParent.GetPosition(connection.transform.position, out float distance);
            distance = Mathf.Clamp(1 / distance, 0.3f, 1);
            connection.nodeOffset = GetPosition().position - pos;
            connection.nodeOffset /= distance;
            Vector3 oldScale = transform.scale;
            transform.scale = Vector3.one * distance;
            if (oldScale != transform.scale)
                this.MarkDirtyRepaint();

            SetOriginLine(connection.nodeOffset);
        }

        private void GeneratePort(BasePort basePort)
        {
            LogicPort port = new LogicPort(basePort);
            
            if (basePort.Direction == PortDirection.Input)
                inputContainer.Add(port);
            else
                outputContainer.Add(port);
        }

        private void DrawNodeFields()
        {
            Dictionary<string, NodeEditorField> markedFields = GetAttributes<NodeEditorField>(connection.GetType(), typeof(Connection));

            SerializedObject serializedObject = new SerializedObject(connection);

            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true);

            do
            {
                if (markedFields.TryGetValue(property.name, out NodeEditorField attribute))
                {
                    VisualElement container = null;
                    switch (attribute.nodePlace)
                    {
                        case NodeEditorField.NodePlace.TitleContainer:
                            container = titleContainer;
                            break;
                        case NodeEditorField.NodePlace.MainContainer:
                            container = mainContainer;
                            break;
                        case NodeEditorField.NodePlace.PortContainer:
                            container = Q<LogicPort>(inputContainer, p => Equals(p.basePort.Name,attribute.portContainer));
                            if (container == null)
                                container = Q<LogicPort>(outputContainer, p => Equals(p.basePort.Name, attribute.portContainer));
                            if(container == null)
                            {
                                Debug.LogWarning("Port " + attribute.portContainer + " was not found for default value");
                                container = mainContainer;
                            }
                            break;
                    }
                    PropertyField propertyField = new PropertyField(property,attribute.label);
                    propertyField.style.alignSelf = Align.Stretch;
                    if (attribute.hideLabel)
                    {
                        propertyField.styleSheets.Add(hideLabelOfPropertyFieldSheet);
                    }
                    if (attribute.minWidth != -1)
                    {
                        propertyField.style.minWidth = attribute.minWidth;
                    }
                    if (!attribute.editable)
                        propertyField.focusable = false;
                    container.Add(propertyField);
                }
            } while (property.NextVisible(false));

            this.Bind(serializedObject);
        }

        private void CreateOriginLine()
        {
            originLine = new VisualElement();
            originLine.style.backgroundColor = new Color(0.11f,0.1f,0.1f,0.5f);
            //originLine.style.display = DisplayStyle.None;
            originLine.style.alignSelf = Align.FlexStart;
            originLine.style.flexWrap = Wrap.NoWrap;
            originLine.style.height = 1;
            originLine.style.marginBottom = -1;
            originLine.style.position = Position.Absolute;

            this.Insert(0, originLine);
            SetOriginLine(connection.nodeOffset);
        }

        private void SetOriginLine(Vector2 offset)
        {
            originLine.style.width = offset.magnitude;
            originLine.style.marginRight = -offset.magnitude;
            originLine.transform.rotation = Quaternion.EulerAngles(0,0, Mathf.Atan2(-offset.y,-offset.x));
        }

        //internal static void MakeNonInteractable(VisualElement visualElement)
        //{
        //    VisualElement overlay = new VisualElement();
        //    overlay.AddManipulator(new Clickable(() => { }, 0, 0));
        //    overlay.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0f);
        //    visualElement.Add(overlay);
        //    overlay.StretchToParentSize();
        //}

        private Dictionary<string,T> GetAttributes<T>(Type type, Type baseType) where T : Attribute
        {
            Dictionary<string, T> markedFields = new Dictionary<string, T>();

            FieldInfo[] objectFields = GetAllFields(type, baseType);
            // search all fields and find the attribute [Position]
            for (int i = 0; i < objectFields.Length; i++)
            {

                // if we detect any attribute print out the data.
                if (Attribute.GetCustomAttribute(objectFields[i], typeof(T), true) is T attribute)
                {
                    markedFields[objectFields[i].Name] = attribute;
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

        private static T Q<T>(VisualElement container,Func<T,bool> func) where T:VisualElement
        {
            foreach(var ve in container.Children())
            {
                if (ve is T res && func.Invoke(res))
                    return res;
            }
            return null;
        }
    }

    internal class LogicPort : Port
    {
        internal readonly BasePort basePort;
        private EdgeConnector<Edge> myEdgeConnector;

        public LogicPort(BasePort basePort) : base(Orientation.Horizontal, GetDirection(basePort.Direction),
                GetCapacity(basePort.CapacityMode), basePort.PortType)
        {
            
            portName = basePort.Name;
            this.basePort = basePort;

            myEdgeConnector = new EdgeConnector<Edge>(new EC());
            this.AddManipulator(myEdgeConnector);

        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);

            if (basePort.Direction == PortDirection.Output)
            {
                BasePort.Connect((edge.input as LogicPort).basePort, basePort);
            }
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);

            //if (basePort.Direction == PortDirection.Output)
            //{
            //    if (edge.input != null)
            //        BasePort.Disconnect((edge.input as LogicPort).basePort, basePort);
            //}
        }

        internal bool IsEdgeDragging(Edge e)
        {
            if (e == null || myEdgeConnector == null || myEdgeConnector.edgeDragHelper.edgeCandidate == null)
                return false;
            return e == myEdgeConnector.edgeDragHelper.edgeCandidate;
        }

        private static Direction GetDirection(PortDirection dir)
        {
            return dir == PortDirection.Input ? Direction.Input : Direction.Output;
        }

        private static Port.Capacity GetCapacity(PortCapacityMode mode)
        {
            return mode == PortCapacityMode.Single ? Port.Capacity.Single : Port.Capacity.Multi;
        }
    }

    internal class EC : IEdgeConnectorListener
    {
        public void OnDrop(GraphView graphView, Edge edge)
        {
            edge.output.Connect(edge);
            edge.input.Connect(edge);
            graphView.Add(edge);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {

        }
    }
}
