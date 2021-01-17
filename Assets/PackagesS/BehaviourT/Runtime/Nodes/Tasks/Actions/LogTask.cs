using System;
using UnityEngine;

namespace BehaviourT
{
    [Serializable]
    [AddComponentMenu("Tasks/Actions/Log Task")]
    public class LogTask : Task
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer,"",true,"Message")]
        private string message = "hello world";

        InputPort<string> MessagePort = null;

        protected internal override void GetPorts(PortList portList)
        {
            base.GetPorts(portList);
            MessagePort = new InputPort<string>("Message", () => message);
            portList.Add(MessagePort);
        }

        protected override TaskState OnEvaluate()
        {
            Debug.Log(MessagePort.Input);
            return TaskState.Success;
        }
    }
}
