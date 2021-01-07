using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    public class SerInputPort
    {
        [SerializeReference]
        public Node outputPortNode;

        [SerializeField]
        public string outputPortName;

        [SerializeField]
        public string portName;

        public SerInputPort(Node outputPortNode, string outputPortName, string portName)
        {
            this.outputPortNode = outputPortNode;
            this.outputPortName = outputPortName;
            this.portName = portName;
        }
    }
}
