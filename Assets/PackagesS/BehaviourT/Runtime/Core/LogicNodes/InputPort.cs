using System;

namespace BehaviourT
{
    public class InputPort<T> : Port<T>
    {
        internal OutputPort<T> connectedPort;

        public bool IsConnected => connectedPort != null;

        private Func<T> getDefaultValue = null;

        public T Input
        {
            get
            {
                if (IsConnected)
                {
                    return connectedPort.Output;
                }
                else if (getDefaultValue != null)
                {
                    return getDefaultValue.Invoke();
                }
                else
                {
                    return default;
                }
            }
        }

        public InputPort(string name, Func<T> getDefaultValue) : base(name)
        {
            this.getDefaultValue = getDefaultValue;
        }

        public InputPort(string name) : base(name)
        {
        }

        internal override void TryConnectOutputPort(SerInputPort serInputPort)
        {
            if (serInputPort.outputPortNode.PortList.TryGetValue(serInputPort.outputPortName, out BasePort outputPort))
            {
                if (outputPort is OutputPort<T> c)
                {
                    connectedPort = c;
                }
            }
        }

        public override Direction GetDirection()
        {
            return Direction.input;
        }
    }
}
