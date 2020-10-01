using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolK.Editor
{
    public class NavBaseNode : Node
    {
        private readonly Vector2 defaultSize = new Vector2(100, 150);

        public string GUID { get; }

        protected UINavGraphView GraphView { get; }

        protected virtual string DefaultCoicePortName => "Choice ";

        public NavBaseNode(UINavGraphView graphView, string title, Vector2 position, string guid = null)
        {
            GraphView = graphView;
            this.title = title;

            GUID = guid == null? Guid.NewGuid().ToString() : guid;
            SetPosition(position);
            graphView.AddElement(this);
        }

        protected void SetPosition(Vector2 position)
        {
            SetPosition(new Rect(position, defaultSize));
        }

        internal Port AddChoicePort(string portName = "")
        {
            if (string.IsNullOrEmpty(portName))
                portName = DefaultCoicePortName +
                    (outputContainer.Query("connector").ToList().Count + 1);

            Port port = AddPort(portName, Direction.Output);

            Label oldLabel = port.contentContainer.Q<Label>("type");
            oldLabel.text = "";
            //port.contentContainer.Remove(oldLabel);

            TextField textField = new TextField
            {
                name = string.Empty,
                value = portName
            };
            textField.RegisterValueChangedCallback(evt =>
            {
                port.portName = evt.newValue;
                oldLabel.text = "";
            });
            port.contentContainer.Add(new Label("  "));
            port.contentContainer.Add(textField);

            Button removeButton = new Button(() => RemovePort(port)) { text = "-" };
            port.contentContainer.Add(removeButton);

            return port;
        }

        protected void AddChoicePortButton(string title = "Add Choice")
        {
            Button addChoiceButton = new Button(() => AddChoicePort());
            addChoiceButton.text = title;
            titleContainer.Add(addChoiceButton);
        }

        protected Port AddPort(string name, Direction direction, Port.Capacity capacity = Port.Capacity.Single, Type type = null)
        {
            Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, type ?? typeof(NavForwardPlaceholder));
            port.portName = name;
            if (direction == Direction.Input)
                inputContainer.Add(port);
            else
                outputContainer.Add(port);

            RefreshExpandedState();
            RefreshPorts();

            return port;
        }

        protected void RemovePort(Port port)
        {
            while (port.connected)
            {
                GraphView.RemoveEdge(port.connections.First());
            }

            if (port.direction == Direction.Input)
                inputContainer.Remove(port);
            else
                outputContainer.Remove(port);

            RefreshPorts();
            RefreshExpandedState();
        }
    }

    internal enum NavForwardPlaceholder { foo }
}
