using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LogicC
{
    [AddComponentMenu("LogicC/Vector2/V2 Destination")]
    public class V2Destination : Connection
    {
        public UnityEvent<Vector2> OnSetValueEvent = default;

        public override string Title => "V2 Destination";

        [NodeEditorField]
        [SerializeField]
        [ReadOnly]
        private Vector2 value = Vector2.zero;

        protected InputPort<Vector2> inputPort = new InputPort<Vector2>("Input", PortCapacityMode.Single);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(inputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            Vector2 value = Vector2.zero;
            if (inputPort.Input.Length > 0)
                value = inputPort.Input[0];

            OnSetValue(value);
            this.value = value;
        }

        protected virtual void OnSetValue(Vector2 value)
        {
            if (OnSetValueEvent != null)
                OnSetValueEvent?.Invoke(value);
        }

    }
}
