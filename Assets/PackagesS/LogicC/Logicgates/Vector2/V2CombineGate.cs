using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Vector2/V2Combine Gate")]
    public class V2CombineGate : Connection
    {
        public override string Title => "V2Split Gate";

        private InputPort<float> InputAPort = new InputPort<float>("X", PortCapacityMode.Single);
        private InputPort<float> InputBPort = new InputPort<float>("Y", PortCapacityMode.Single);
        private OutputPort<Vector2> Output1Port = new OutputPort<Vector2>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(InputAPort);
            ports.Add(InputBPort);
            ports.Add(Output1Port);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            float x = InputAPort.GetFirst();
            float y = InputBPort.GetFirst();

            Output1Port.Output = new Vector2(x,y);
        }
    }
}
