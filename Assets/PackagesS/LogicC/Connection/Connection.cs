using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LogicC
{
    [ExecuteInEditMode]
    public abstract class Connection : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Attributes
        public static Action<Connection> OnConnectionEnabled;

        public static Action<Connection> OnConnectionDisabled;

        [HideInInspector]
        [SerializeField]    
        public Vector2 nodeOffset = Vector2.zero;

        public abstract string Title { get; }

        public bool IsDestroied { get; private set; } = false;

        [NonSerialized]
        internal bool wasInitCon = false;

        [SerializeField]
        [ReadOnly]
        private SerPort[] serializedPorts = new SerPort[0];

        private Dictionary<string, BasePort> ports = new Dictionary<string, BasePort>();
        public IEnumerable<BasePort> BasePorts => ports.Values;

        internal bool IsUpdateDepending { get; private set; } = true;

        private bool updateAfterDeserializedFlag = false;
        #endregion

        #region Init

        public void Init()
        {
            if (wasInitCon)
                return; 
            wasInitCon = true;
            ports.Clear(); 
            List<BasePort> portList = GetPorts(new List<BasePort>());

            foreach (BasePort port in portList)
            {
                if (ports.ContainsKey(port.Name))
                    throw new LogicException("Port name has to be unique");
                port.Parent = this;
                ports.Add(port.Name, port);
            }
        }

        public bool TryGetPort(string name, out BasePort port)
        {
            Init();
            return ports.TryGetValue(name, out port);
        }

        #endregion

        #region Update

        public void UpdateConnection()
        {
            IsUpdateDepending = true;

            if (!CanUpdateInputports())
                return;
            if (IsDestroied)
                return;

            UpdateInputs();
            try
            {
                InputPortUpdate();
            }catch(Exception e)
            {
                Debug.LogError(e);
            }
            IsUpdateDepending = false;

            UpdateOutputs();
        }

        protected abstract void InputPortUpdate();

        private bool CanUpdateInputports()
        {
            if (wasInitCon == false)
                return false; 
            foreach (BasePort p in BasePorts)
            {
                if (p.Direction == PortDirection.Input)
                {
                    if (p.IsReferencesUpdateDepending())
                        return false;
                }
            }
            return true;
        }

        private void UpdateInputs()
        {
            foreach (BasePort p in BasePorts)
            {
                if (p.Direction == PortDirection.Input)
                {
                    p.UpdateInputPortValues();
                }
            }
        }

        private void UpdateOutputs()
        {
            foreach (BasePort p in BasePorts)
            {
                if (p.Direction == PortDirection.Output)
                {
                    p.UpdateReferences();
                }
            }
        }

        #endregion

        #region Serialization
        public void OnAfterDeserialize()
        {
            //Debug.Log("OnAfterDeserialize");
            Init();
            foreach (BasePort port in ports.Values)
            {
                SerPort serPort = serializedPorts.FirstOrDefault(sp => Equals(sp.Name, port.Name));
                //if(serPort != null)
                //print("SerPort: " + serPort.Name + " "+serPort.CapacityMode + " " + serPort.Direction 
                //+ " " +serPort.PortType.ToString());

                if (serPort != null && serPort.CapacityMode == port.CapacityMode
                    && serPort.Direction == port.Direction && Equals(serPort.PortType, port.PortType.AssemblyQualifiedName))
                {
                    serPort.Deserialize(port);
                    //print("Deserialize");
                }
            }
            serializedPorts = new SerPort[0];

            updateAfterDeserializedFlag = true;
        }

        public void OnBeforeSerialize()
        {
            serializedPorts = new SerPort[ports.Count];
            int i = 0;
            foreach (BasePort port in ports.Values)
            {
                serializedPorts[i] = new SerPort(port);
                i++;
            }

        }

        protected abstract List<BasePort> GetPorts(List<BasePort> ports);
        #endregion

        #region Unity Callbacks
        protected virtual void Awake() 
        {
            //Debug.Log("Awake");
            Init();

            //UpdateConnection();
        }

        protected virtual void OnEnable()
        {
            OnConnectionEnabled?.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            OnConnectionDisabled?.Invoke(this);
        }

        private void OnDestroy()
        {
            IsDestroied = true;
        }

        private void Update()
        {
            if (updateAfterDeserializedFlag)
            {
                updateAfterDeserializedFlag = false;
                UpdateConnection();
            }
        }
        #endregion

        private void DisconnectAllPorts()
        {
            foreach(var p in ports)
            {
                BasePort.DisconnectAll(p.Value);
            }
        }
    }

    #region External
    public class LogicException : Exception
    {
        public LogicException(string message) : base(message)
        {
        }
    }
    #endregion
}
