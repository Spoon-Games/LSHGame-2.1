using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Float Source")]
    public class FloatSource : Connection
    {
        public override string Title => "Float Source";

        [NodeEditorField(NodeEditorField.NodePlace.MainContainer)]
        [SerializeField]
        private float value = 0;
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                UpdateConnection();
            }
        }
        private OutputPort<float> OutputPort = new OutputPort<float>("Output", PortCapacityMode.Multiple);

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
