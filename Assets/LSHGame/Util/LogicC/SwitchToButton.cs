using LogicC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    [AddComponentMenu("LogicC/Boolean/SwitchToButton")]
    public class SwitchToButton : Connection
    {
        public override string Title => "SwitchToButton";

        [SerializeField]
        [NodeEditorField]
        private float extendTime = 1;

        [SerializeField]
        [NodeEditorField]
        private bool extendAtStart = true;

        [SerializeField]
        [NodeEditorField]
        private bool extendSignal = false;

        private InputPort<bool> inputPort = new InputPort<bool>("Input");
        private OutputPort<bool> outputPort = new OutputPort<bool>("Output");

        private bool wasPortActivated;

        private float extendTimer = float.NegativeInfinity;

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(inputPort);
            ports.Add(outputPort);

            return ports;
        }

        protected override void InputPortUpdate()
        {
            if ((wasPortActivated ^ extendAtStart) && (!inputPort.GetFirst() ^ extendAtStart))
                extendTimer = Time.fixedTime + extendTime;

            if (Time.fixedTime < extendTimer)
                outputPort.Output = true;
            else
            {
                extendTimer = float.NegativeInfinity;
                outputPort.Output = false;
            }

            outputPort.Output |= extendSignal && inputPort.GetFirst();

            wasPortActivated = inputPort.GetFirst();
        }

        private void FixedUpdate()
        {
            if (extendTimer != float.NegativeInfinity && Time.fixedTime >= extendTime)
                UpdateConnection();
        }
    } 
}
