using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Compare Gate")]
    public class CompareGate : Connection
    {
        public override string Title => "Compare Gate";

        public enum CompareType { EQUALS,UNEQUALS,GREATER,GREATEREQU,SMALLER,SMALEREQU }

        [NodeEditorField(NodeEditorField.NodePlace.MainContainer, hideLabel: true)]
        [SerializeField]
        private CompareType gateType;
        public CompareType GateType
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

        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "C")]
        [SerializeField]
        private float defaultValueB;

        private InputPort<float> InputAPort = new InputPort<float>("A", PortCapacityMode.Single);
        private InputPort<float> InputBPort = new InputPort<float>("B", PortCapacityMode.Single);
        private OutputPort<bool> OutputPort = new OutputPort<bool>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
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
                OutputPort.Output = false;
            }
        }

        private bool GetOutput()
        {
            float a = InputAPort.Input.Length > 0 ? InputAPort.Input[0] : 0;
            float b = InputBPort.Input.Length > 0 ? InputBPort.Input[0] : 0;
            bool res = false;
            switch (gateType)
            {
                case CompareType.EQUALS:
                    res = a == b;
                    break;
                case CompareType.UNEQUALS:
                    res = a != b;
                    break;
                case CompareType.GREATER:
                    res = a > b;
                    break;
                case CompareType.GREATEREQU:
                    res = a >= b;
                    break;
                case CompareType.SMALLER:
                    res = a < b;
                    break;
                case CompareType.SMALEREQU:
                    res = a <= b;
                    break;
            }
            return res;
        }
    }
}