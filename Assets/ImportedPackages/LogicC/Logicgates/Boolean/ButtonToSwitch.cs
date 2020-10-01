using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LogicC
{
    [AddComponentMenu("LogicC/Boolean/ButtonToSwitch")]
    public class ButtonToSwitch : Connection
    {
        public override string Title => "ButtonToSwitch";

        private bool wasActivated = false;

        private InputPort<bool> InputPort = new InputPort<bool>("Input", PortCapacityMode.Multiple);
        private OutputPort<bool> OutputPort = new OutputPort<bool>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(InputPort);
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            bool i = false;
            foreach(var v in InputPort.Input)
            {
                i |= v;
            }
            if(!wasActivated && i)
            {
                OutputPort.Output = !OutputPort.Output;
            }
            wasActivated = i;
        }
    }
}
