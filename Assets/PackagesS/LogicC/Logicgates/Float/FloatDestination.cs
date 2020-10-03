using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Float Destination")]
    public class FloatDestination : Connection
    {
        public UnityEvent<float> OnSetValueEvent = default;

        public override string Title => "Float Destination";

        [NodeEditorField]
        [SerializeField]
        [ReadOnly]
        private float value = 0;

        protected InputPort<float> inputPort = new InputPort<float>("Input", PortCapacityMode.Single);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(inputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            float value = 0;
            if (inputPort.Input.Length > 0)
                value = inputPort.Input[0];

            OnSetValue(value);
            this.value = value;
        }

        protected virtual void OnSetValue(float value)
        {
            if(OnSetValueEvent != null)
                OnSetValueEvent?.Invoke(value);
        }

    }
}

