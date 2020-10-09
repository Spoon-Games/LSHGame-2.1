using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Vector2/V2Math Gate")]
    public class V2MathGate : Connection
    {
        public override string Title => "V2Math Gate";

        public enum MathOperation { PLUS, MINUS, MULTIPLY, DIVIDE,DISTANCE,DOT,MAGNITUDE,NORMALIZE,PERPENDICULAR,ANGLE }

        [NodeEditorField(NodeEditorField.NodePlace.TitleContainer, hideLabel: true)]
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
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "A",minWidth:150)]
        [SerializeField]
        private Vector2 defaultValueA;

        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "B",minWidth:150)]
        [SerializeField]
        private Vector2 defaultValueB;


        private InputPort<Vector2> InputAPort;
        private InputPort<Vector2> InputBPort;
        private OutputPort<Vector2> OutputPort = new OutputPort<Vector2>("Output", PortCapacityMode.Multiple);
        private OutputPort<float> OutputFloatPort = new OutputPort<float>("O Float", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            InputAPort = new InputPort<Vector2>("A", () => defaultValueA);
            InputBPort = new InputPort<Vector2>("B", () => defaultValueB);
            ports.Add(InputAPort);
            ports.Add(InputBPort);
            ports.Add(OutputPort);
            ports.Add(OutputFloatPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            try
            {
                OutputPort.Output = GetOutput(out float floatRes);
                OutputFloatPort.Output = floatRes;
            }
            catch (ArithmeticException)
            {
                OutputPort.Output = Vector2.zero;
                OutputFloatPort.Output = 0;
            }
        }

        private Vector2 GetOutput(out float floatRes)
        {
            Vector2 a = InputAPort.Input.Length > 0 ? InputAPort.Input[0] : Vector2.zero;
            Vector2 b = InputBPort.Input.Length > 0 ? InputBPort.Input[0] : Vector2.zero;
            Vector2 res = Vector2.zero;
            floatRes = 0;

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
                case MathOperation.DISTANCE:
                    floatRes = Vector2.Distance(a, b);
                    break;
                case MathOperation.DOT:
                    floatRes = Vector2.Dot(a, b);
                    break;
                case MathOperation.MAGNITUDE:
                    floatRes = a.magnitude;
                    break;
                case MathOperation.PERPENDICULAR:
                    res = Vector2.Perpendicular(a);
                    break;
                case MathOperation.NORMALIZE:
                    res = a.normalized;
                    break;
                case MathOperation.ANGLE:
                    floatRes = Vector2.Angle(a, b);
                    break;
            }
            return res;
        }
    }
}
