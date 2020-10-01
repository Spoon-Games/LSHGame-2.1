using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/2D/Transform Controller 2D")]
    public class TransformController2D : Connection
    {
        public override string Title => "Transform Controller 2D";

        public enum Orientation { XY,XZ }

        [NodeEditorField(NodeEditorField.NodePlace.TitleContainer, hideLabel: true)]
        [SerializeField]
        private Orientation gateType = Orientation.XY;
        public Orientation GateType
        {
            get => gateType; set
            {
                gateType = value;
                UpdateConnection();
            }
        }

        private InputPort<Vector2> InputAPort = new InputPort<Vector2>("Position",PortCapacityMode.Single);
        private InputPort<Vector2> InputBPort = new InputPort<Vector2>("Direction", PortCapacityMode.Single);
        private InputPort<Vector2> InputCPort = new InputPort<Vector2>("Scale", PortCapacityMode.Single);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(InputAPort);
            ports.Add(InputBPort);
            ports.Add(InputCPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            if(InputAPort.Input.Length > 0)
            {
                Vector2 i = InputAPort.Input[0];
                Vector3 p = transform.position;
                switch (gateType)
                {
                    case Orientation.XY:
                        transform.localPosition = new Vector3(i.x, i.y, p.z);
                        break;
                    case Orientation.XZ:
                        transform.localPosition = new Vector3(i.x, p.z, i.y);
                        break;
                }
            }

            if(InputBPort.Input.Length > 0)
            {
                Vector2 i = InputBPort.Input[0];
                switch (gateType)
                {
                    case Orientation.XY:
                        transform.localRotation = Quaternion.LookRotation(i,Vector3.back);
                        break;
                    case Orientation.XZ:
                        transform.localRotation = Quaternion.LookRotation(new Vector3(i.x, 0, i.y), Vector3.up);
                        break;
                }

            }

            if(InputCPort.Input.Length > 0)
            {
                Vector2 i = InputCPort.GetFirst();
                switch (gateType)
                {
                    case Orientation.XY:
                        transform.localScale = new Vector3(i.x,1,i.y);
                        break;
                    case Orientation.XZ:
                        transform.localScale = new Vector3(i.x, i.y, 1);
                        break;
                }
            }
        }
    }
}