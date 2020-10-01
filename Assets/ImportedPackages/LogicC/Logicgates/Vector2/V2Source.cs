using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Vector2/V2 Source")]
    public class V2Source : Connection
    {
        public override string Title => "V2 Source";

        [NodeEditorField(NodeEditorField.NodePlace.MainContainer)]
        [SerializeField]
        private Vector2 value = Vector2.zero;
        public Vector2 Value
        {
            get => value;
            set
            {
                this.value = value;
                UpdateConnection();
            }
        }
        private OutputPort<Vector2> OutputPort = new OutputPort<Vector2>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            OutputPort.Output = value;
        }
    }
}
