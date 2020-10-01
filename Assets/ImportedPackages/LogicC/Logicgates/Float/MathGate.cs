using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Math Gate")]
    public class MathGate : Connection
    {
        public override string Title => "Math Gate";

        public enum MathOperation { PLUS, MINUS, MULTIPLY, DIVIDE, POWER,EXP, ROOT,SQRT, LOGARYTHM, LOGE,}

        [NodeEditorField(NodeEditorField.NodePlace.TitleContainer,hideLabel:true)]
        [SerializeField]
        private MathOperation gateType;
        public MathOperation GateType
        {
            get => gateType; set
            {
                gateType = value;
                UpdateConnection();
            }
        }
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer,hideLabel:true, portContainer: "A")]
        [SerializeField]
        private float defaultValueA;

        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "B")]
        [SerializeField]
        private float defaultValueB;


        private InputPort<float> InputAPort;
        private InputPort<float> InputBPort;
        private OutputPort<float> OutputPort = new OutputPort<float>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            InputAPort = new InputPort<float>("A", () => defaultValueA);
            InputBPort = new InputPort<float>("B", () => defaultValueB);
            ports.Add(InputAPort);
            ports.Add(InputBPort);
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
            float b = InputBPort.Input.Length > 0 ? InputBPort.Input[0] : 0;
            float res = 0;
            switch (gateType)
            {
                case MathOperation.PLUS:
                    res = a + b;
                    break;
                case MathOperation.MINUS:
                    res = a - b;
                    break;
                case MathOperation.MULTIPLY:
                    res = a * b;
                    break;
                case MathOperation.DIVIDE:
                    res = a / b;
                    break;
                case MathOperation.POWER:
                    res = Mathf.Pow(a, b);
                    break;
                case MathOperation.ROOT:
                    res = Mathf.Pow(a, 1 / b);
                    break;
                case MathOperation.LOGARYTHM:
                    res = Mathf.Log(a, b);
                    break;
                case MathOperation.SQRT:
                    res = Mathf.Sqrt(a);
                    break;
                case MathOperation.LOGE:
                    res = Mathf.Log(a);
                    break;
                case MathOperation.EXP:
                    res = Mathf.Exp(a);
                    break;
            }
            return res;
        }
    }
}