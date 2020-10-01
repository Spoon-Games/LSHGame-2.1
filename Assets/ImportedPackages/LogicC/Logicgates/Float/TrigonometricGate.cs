using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Trigonometric Gate")]
    public class TrigonometricGate : Connection
    {
        public override string Title => "Trigonometric Gate";

        public enum Operation { SIN,COS,TAN,ASIN,ACOS,ATAN }

        [NodeEditorField(NodeEditorField.NodePlace.TitleContainer, hideLabel: true)]
        [SerializeField]
        private Operation gateType;
        public Operation GateType
        {
            get => gateType; set
            {
                gateType = value;
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
            try
            {
                OutputPort.Output = GetOutput();
            }
            catch (ArithmeticException)
            {
                OutputPort.Output = 0;
            }
        }

        private float GetOutput()
        {
            float a = InputAPort.Input.Length > 0 ? InputAPort.Input[0] : 0;
            float res = 0;
            switch (gateType)
            {
                case Operation.SIN:
                    res = Mathf.Sin(a);
                    break;
                case Operation.COS:
                    res = Mathf.Cos(a);
                    break;
                case Operation.TAN:
                    res = Mathf.Tan(a);
                    break;
                case Operation.ASIN:
                    res = Mathf.Asin(a);
                    break;
                case Operation.ACOS:
                    res = Mathf.Acos(a);
                    break;
                case Operation.ATAN:
                    res = Mathf.Atan(a);
                    break;
            }
            return res;
        }
    }
}
