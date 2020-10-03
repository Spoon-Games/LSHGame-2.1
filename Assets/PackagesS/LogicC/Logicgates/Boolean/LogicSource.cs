using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Boolean/Logic Source")]
    public class LogicSource : Connection
    {
        public override string Title => "Source";

        [NodeEditorField(NodeEditorField.NodePlace.MainContainer)]
        [SerializeField]
        private bool fired = false;
        public bool Fired
        {
            get => fired;
            set{
                fired = value;
                UpdateConnection();
            }
        }
        private OutputPort<bool> OutputPort = new OutputPort<bool>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            OutputPort.Output = fired;
        }
    }
}
