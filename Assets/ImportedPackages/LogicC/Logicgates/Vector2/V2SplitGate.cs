using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Vector2/V2Split Gate")]
    public class V2SplitGate : Connection
    {
        public override string Title => "V2Split Gate";

        private InputPort<Vector2> InputAPort = new InputPort<Vector2>("Input", PortCapacityMode.Single);
        private OutputPort<float> Output1Port = new OutputPort<float>("X", PortCapacityMode.Multiple);
        private OutputPort<float> Output2Port = new OutputPort<float>("Y", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            
            ports.Add(InputAPort);
            ports.Add(Output1Port);
            ports.Add(Output2Port);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            Vector2 v = Vector2.zero;
            if (InputAPort.Input.Length > 0)
                v = InputAPort.Input[0];

            Output1Port.Output = v.x;
            Output2Port.Output = v.y;
        }
    }
}
