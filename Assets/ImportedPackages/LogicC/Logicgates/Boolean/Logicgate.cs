using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Boolean/Logicgate")]
    public class Logicgate : Connection
    {
        public override string Title => "Logicgate";

        public enum LogicGateType { AND, OR, XOR, NAND, NOR, NXOR }

        [NodeEditorField(NodeEditorField.NodePlace.MainContainer, hideLabel: true)]
        [SerializeField]
        private LogicGateType gateType; 
        public LogicGateType GateType { get => gateType; set
            {
                gateType = value;
                UpdateConnection();
            } }

        private InputPort<bool> InputPort = new InputPort<bool>("Input",PortCapacityMode.Multiple);
        private OutputPort<bool> OutputPort = new OutputPort<bool>("Output",PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(InputPort);
            ports.Add(OutputPort); 
            return ports;
        }

        protected override void InputPortUpdate()
        {
            OutputPort.Output = GetFired();
        }

        #region Old
        private bool GetFired()
        {
            bool f = false;
            switch (gateType)
            {
                case LogicGateType.AND:
                    f = GetAND();
                    break;
                case LogicGateType.OR:
                    f = GetOR();
                    break;
                case LogicGateType.XOR:
                    f = GetXOR();
                    break;
                case LogicGateType.NAND:
                    f = GetNAND();
                    break;
                case LogicGateType.NOR:
                    f = GetNOR();
                    break;
                case LogicGateType.NXOR:
                    f = GetNXOR();
                    break;
            }
            return f;
        }

        private bool GetAND()
        {
            bool f = true;
            foreach (bool i in InputPort.Input)
            {
                f &= i;
            }
            if (InputPort.Input.Length == 0)
                f = false;
            return f;
        }

        private bool GetOR()
        {
            bool f = false;
            foreach (bool i in InputPort.Input)
            {
                f |= i;
            }
            return f;
        }

        private bool GetXOR()
        {
            bool f = false;
            foreach (bool i in InputPort.Input)
            {
                f ^= i;
            }
            return f;
        }

        private bool GetNAND()
        {
            return !GetAND();
        }

        private bool GetNOR()
        {
            return !GetOR();
        }

        private bool GetNXOR()
        {
            return !GetXOR();
        }
        #endregion
    } 
}