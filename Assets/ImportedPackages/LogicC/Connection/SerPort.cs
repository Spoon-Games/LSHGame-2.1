using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicC
{
    public enum PortCapacityMode { Single, Multiple }

    public enum PortDirection { Input,Output}

    [System.Serializable]
    public class SerPort
    {
        [SerializeField]
        private string name;
        public string Name => name;

        [SerializeField]
        private PortCapacityMode capacityMode;
        public PortCapacityMode CapacityMode => capacityMode;

        [SerializeField]
        private PortDirection direction;
        public PortDirection Direction => direction;

        [SerializeField]
        private string portType;
        public string PortType => portType;

        [SerializeField]
        private List<Connection> refParents = new List<Connection>();
        [SerializeField]
        private List<string> refNames = new List<string>();

        public SerPort() { }

        public SerPort(BasePort basePort)
        {
            this.name = basePort.Name;
            this.capacityMode = basePort.CapacityMode;
            this.direction = basePort.Direction;
            this.portType = basePort.PortType.AssemblyQualifiedName;

            var refs = new List<BasePort>();

            foreach(BasePort port in basePort.GetReferences())
            {
                if (!refs.Contains(port))
                {
                    refs.Add(port);
                    refParents.Add(port.Parent);
                    refNames.Add(port.Name);
                }
            }
        }

        internal void Deserialize(BasePort port)
        {
            for(int i = 0; i < refParents.Count; i++)
            {
                if(refParents[i] != null && refParents[i].TryGetPort(refNames[i],out BasePort reference)){
                    port.AddReference(reference);
                } 
            }
        }
    }

    public class SerPortsAttribute : PropertyAttribute
    {

    }
}
