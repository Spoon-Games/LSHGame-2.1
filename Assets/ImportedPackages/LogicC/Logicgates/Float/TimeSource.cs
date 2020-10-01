using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    [AddComponentMenu("LogicC/Float/Time Source")]
    public class TimeSource : Connection
    {
        public enum UpdateMode { Update,FixedUpdate}

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.TitleContainer, hideLabel: true)]
        [Tooltip("Will the graph be UpdateConnection updated on Update or FixedUpdate")]
        private UpdateMode updateMode = UpdateMode.FixedUpdate;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "Output",editable:false)]
        private float timeValue = 0;

        private OutputPort<float> OutputPort = new OutputPort<float>("Output", PortCapacityMode.Multiple);

        public override string Title => "Time Source";

        protected override List<BasePort> GetPorts(List<BasePort> ports)
        {
            ports.Add(OutputPort);
            return ports;
        }

        protected override void InputPortUpdate()
        {
            if (updateMode == UpdateMode.Update)
            {
                timeValue = Time.time;
                OutputPort.Output = Time.time;
            }
            else
            {
                timeValue = Time.fixedTime;
                OutputPort.Output = Time.fixedTime;
            }
        }

        private void Update()
        {
            if (updateMode == UpdateMode.Update)
                UpdateConnection();
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate)
                UpdateConnection();
        }
    }
}
