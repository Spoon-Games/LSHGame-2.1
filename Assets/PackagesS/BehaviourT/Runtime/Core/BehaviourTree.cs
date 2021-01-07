using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Behaviour Tree",menuName = "Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        public float BTDeltaTime => Time.fixedDeltaTime;
        public float BTTime => Time.fixedTime;

        [SerializeReference]
        private Node[] nodes = new Node[] { new RootTask() };

        [SerializeReference]
        private DataExNode[] dataExNodes = new DataExNode[0];

        private RootTask rootTask;

        private Lookup<string, ExposedProperty> exposedProperties;
        public Lookup<string, ExposedProperty> ExposedProperties { get
            {
                if(exposedProperties == null)
                {
                    List<ExposedProperty> expProps = new List<ExposedProperty>();

                    foreach (Node node in nodes)
                    {
                        node.Initialize(this);

                        if (node is ExposedProperty e)
                            expProps.Add(e);
                    }

                    exposedProperties = (Lookup<string, ExposedProperty>)expProps.ToLookup(p => p.name);
                }
                return exposedProperties;
            } }

        public void Initialize()
        {
            List<ExposedProperty> expProps = new List<ExposedProperty>();

            foreach(Node node in nodes)
            {
                node.Initialize(this);

                if (node is RootTask r)
                    rootTask = r;
                else if (node is ExposedProperty e)
                    expProps.Add(e);
            }

            exposedProperties = (Lookup<string,ExposedProperty>)expProps.ToLookup(p => p.name);

            foreach(Node node in nodes)
            {
                node.InitializePorts();
            }

            Reset();

            foreach(Node node in nodes)
            {
                node.Awake();
            }
        }

        public void Update()
        {
            rootTask.Evaluate();
            rootTask.PostUpdateEvaluate();
        }

        public bool TryGetValue<T>(string key,out T value)
        {
            foreach(ExposedProperty e in exposedProperties[key])
            {
                if (e.TryGetValue<T>(out value))
                    return true;
            }
            value = default;
            return false;
        }

        public bool TrySetValue(string key,object value)
        {
            bool wasSet = false;
            foreach(var e in exposedProperties[key])
            {
                wasSet |= e.TrySetValue(value);
            }
            return wasSet;
        }

        public void Destroy()
        {
            foreach(Node node in nodes)
            {
                node.Destroy();
            }
        }

        public void Reset()
        {
            rootTask?.ResetState();
        }

        public void SerializeNodes(Node[] nodes)
        {
            this.nodes = nodes;
        }

        public Node[] DeserializeNodes()
        {
            return nodes;
        }

        public void SerializeDataExNodes(DataExNode[] dataExNodes)
        {
            this.dataExNodes = dataExNodes;
        }

        public DataExNode[] DeserializeDataExNodes()
        {
            return dataExNodes;
        }
    }
}
