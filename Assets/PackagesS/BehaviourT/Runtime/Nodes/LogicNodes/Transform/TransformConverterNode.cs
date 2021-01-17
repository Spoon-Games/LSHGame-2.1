using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("LogicNodes/Transform/Transform Converter")]
    public class TransformConverterNode : LogicNode
    {
        InputPort<Transform> transformPort = new InputPort<Transform>("Transform");


        protected internal override void GetPorts(PortList portList)
        {
            portList.Add(transformPort);
            portList.Add(new OutputPort<Vector3>("Position", () => transformPort.IsConnected ? transformPort.Input.position : Vector3.zero));
            portList.Add(new OutputPort<Quaternion>("Rotation", () => transformPort.IsConnected ? transformPort.Input.rotation : Quaternion.identity));
            portList.Add(new OutputPort<Vector3>("Scale", () => transformPort.IsConnected ? transformPort.Input.localScale : Vector3.one));
            
        }
    }
}
