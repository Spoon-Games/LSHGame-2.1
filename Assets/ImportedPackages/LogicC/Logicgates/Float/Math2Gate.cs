using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Math2 Gate")]
    public class Math2Gate : Connection
    {
        public override string Title => "Math2 Gate";

        public enum Operation { MIN,MAX,ABS,SIGN, CLAMP, CLAMP01,FLOOR,CEIL,ROUND, LERP, INVERSLERP,LERPFREE,INVERSELERPFREE, PINGPONG,REPEAT,SMOOTHSTEP}

        [NodeEditorField(NodeEditorField.NodePlace.MainContainer, hideLabel: true)]
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

        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "B")]
        [SerializeField]
        private float defaultValueB;

        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "C")]
        [SerializeField]
        private float defaultValueC;

        private InputPort<float> InputAPort;
        private InputPort<float> InputBPort;
        private InputPort<float> InputCPort;
        private OutputPort<float> OutputPort = new OutputPort<float>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            InputAPort = new InputPort<float>("A", () => defaultValueA);
            InputBPort = new InputPort<float>("B", () => defaultValueB);
            InputCPort = new InputPort<float>("C", () => defaultValueC);
            ports.Add(InputAPort);
            ports.Add(InputBPort);
            ports.Add(InputCPort);
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            try
            {
                OutputPort.Output = GetOutput();
            }
            catch (Exception)
            {
                OutputPort.Output = 0;
            }
        }

        private float GetOutput()
        {
            float a = InputAPort.Input.Length > 0 ? InputAPort.Input[0] : 0;
            float b = InputBPort.Input.Length > 0 ? InputBPort.Input[0] : 0;
            float c = InputCPort.Input.Length > 0 ? InputCPort.Input[0] : 0;
            float res = 0;
            switch (gateType)
            {
                case Operation.LERP:
                    res = Mathf.Lerp(a, b, c);
                    break;
                case Operation.INVERSLERP:
                    res = Mathf.InverseLerp(a, b, c);
                    break;
                case Operation.CLAMP:
                    res = Mathf.Clamp(a, b, c);
                    break;
                case Operation.CLAMP01:
                    res = Mathf.Clamp01(a);
                    break;
                case Operation.FLOOR:
                    res = Mathf.Floor(a);
                    break;
                case Operation.CEIL:
                    res = Mathf.Ceil(a);
                    break;
                case Operation.ROUND:
                    res = Mathf.Round(a);
                    break;
                case Operation.MIN:
                    res = Mathf.Min(a);
                    break;
                case Operation.MAX:
                    res = Mathf.Max(a);
                    break;
                case Operation.ABS:
                    res = Mathf.Abs(a);
                    break;
                case Operation.SIGN:
                    res = Mathf.Sign(a);
                    break;
                case Operation.PINGPONG:
                    res = Mathf.PingPong(a, b);
                    break;
                case Operation.REPEAT:
                    res = Mathf.Repeat(a, b);
                    break;
                case Operation.SMOOTHSTEP:
                    res = Mathf.SmoothStep(a, b, c);
                    break;
                case Operation.LERPFREE:
                    res = Mathf.LerpUnclamped(a, b, c);
                    break;
                case Operation.INVERSELERPFREE:
                    res = (c - a) / (b - a) ; 
                    break;
            }
            return res;
        }
    }
}
