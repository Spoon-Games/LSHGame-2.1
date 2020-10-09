using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Curve Gate")]
    public class CurvecGate : Connection
    {
        public override string Title => "Curve Gate";


        [NodeEditorField(minWidth: 300)]
        [SerializeField]
        private AnimationCurve curve = new AnimationCurve();
        public AnimationCurve Curve
        {
            get => curve; set
            {
                curve = value;
                UpdateConnection();
            }
        }

        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "A")]
        [SerializeField]
        private float defaultValueA;

        private InputPort<float> InputAPort;
        private OutputPort<float> OutputPort = new OutputPort<float>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            InputAPort = new InputPort<float>("A", () => defaultValueA);
            ports.Add(InputAPort);
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            float t = InputAPort.GetFirst();
            //t = Mathf.Clamp01(t);

            OutputPort.Output = curve.Evaluate(t);
        }

        
    }
}
