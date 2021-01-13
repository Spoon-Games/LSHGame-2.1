using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    public abstract class Node
    {
        public BehaviourTree Parent { get; private set; }

        public BehaviourTreeComponent Component => Parent?.BehaviourTreeComponent;
        public Transform Transform => Component.transform;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.Hide)]
        private SerInputPort[] serInputPorts = new SerInputPort[0];

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.Hide)]
        public Vector2 editorPos;

        private PortList _portList;
        public PortList PortList { get
            {
                if(_portList == null)
                {
                    _portList = new PortList();
                    GetPorts(_portList);
                }
                return _portList;
            }
        }

        internal void Initialize(BehaviourTree parent)
        {
            Parent = parent;
            _portList.Clear();
            GetPorts(_portList);
        }

        internal void InitializePorts()
        {
            foreach (var serInputPort in serInputPorts)
            {
                if (serInputPort.outputPortNode == null || string.IsNullOrEmpty(serInputPort.outputPortName))
                    continue;

                if (PortList.TryGetValue(serInputPort.portName, out BasePort inputPort))
                {
                    inputPort.TryConnectOutputPort(serInputPort);
                }
            }
        }

        public void SetSerializedInputPorts(SerInputPort[] serInputPorts)
        {
            this.serInputPorts = serInputPorts;
        }

        public SerInputPort[] GetSerializedInputPorts()
        {
            return serInputPorts;
        }

        internal protected virtual void GetPorts(PortList portList) { }

        internal protected virtual void Awake() { }
        internal protected virtual void Destroy() { }

#if UNITY_EDITOR
        internal void OnDrawGizmos(BehaviourTree parent)
        {
            this.Parent = parent;
            OnDrawGizmos();
        }

        public virtual void OnDrawGizmos() { }
#endif
    } 

    public class PortList : IEnumerable<BasePort>
    {
        private Dictionary<string, BasePort> ports = new Dictionary<string, BasePort>();

        public void Add(BasePort basePort)
        {
            if (ports.ContainsKey(basePort.Name))
                throw new System.Exception("The name of the ports has to be unique");
            ports.Add(basePort.Name, basePort);
        }

        public IEnumerator<BasePort> GetEnumerator()
        {
            return ports.Values.GetEnumerator();
        }

        public bool TryGetValue(string key,out BasePort value)
        {
            return ports.TryGetValue(key, out value);
        }

        internal void Clear()
        {
            ports.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ports.Values.GetEnumerator();
        }
    }
}
