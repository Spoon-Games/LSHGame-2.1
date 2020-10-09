using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LogicC
{
    [AddComponentMenu("LogicC/Boolean/LogicDestiantion")]
    public class LogicDestination : Connection
    {
        public UnityEvent OnActivatedEvent;
        public UnityEvent OnDeactivatedEvent;

        public override string Title => "Destination";

        protected InputPort<bool> inputPort = new InputPort<bool>("Input", PortCapacityMode.Single);


        [NodeEditorField]
        [ReadOnly]
        [SerializeField]
        private bool active;

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(inputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            bool activated = false;
            if (inputPort.Input.Length > 0)
                activated = inputPort.Input[0];

            if (!active && activated)
                OnActivated();
            else if (active && !activated)
                OnDeactivated();

            active = activated;

            OnSetValue(activated);
        }

        protected virtual void OnActivated()
        {
            OnActivatedEvent.Invoke();
        }

        protected virtual void OnDeactivated()
        {
            OnDeactivatedEvent.Invoke();
        }

        protected virtual void OnSetValue(bool value) { }
    }
}
