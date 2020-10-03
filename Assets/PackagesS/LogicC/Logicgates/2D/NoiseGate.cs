using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Vector2/Noise Gate")]
    public class NoiseGate : Connection
    {
        public override string Title => "Noise Gate";

        
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "Position", minWidth: 150)]
        [SerializeField]
        private Vector2 defaultValueA;

        [NodeEditorField]
        [SerializeField]
        private Vector2 offset = Vector2.zero;

        [NodeEditorField]
        [SerializeField]
        private float scale = 1;


        private InputPort<Vector2> InputAPort;
        private OutputPort<float> OutputPort = new OutputPort<float>("Output", PortCapacityMode.Multiple);

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            InputAPort = new InputPort<Vector2>("Position", () => defaultValueA);
            ports.Add(InputAPort);
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            Vector2 pos = InputAPort.GetFirst();
            pos *= scale;
            pos += offset;

            OutputPort.Output = Mathf.PerlinNoise(pos.x, pos.y);
            
        }
    }
}
