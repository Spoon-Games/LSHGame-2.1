using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LogicC
{
    public class InputPort<T> : Port<T>
    {
        private T[] input = new T[0];
        public T[] Input
        {
            get {
                if (getDefaultValue != null && input.Length == 0)
                    return new T[] { getDefaultValue.Invoke() };
                return input; }
            set => input = value;
        }

        public bool IsConnected => input.Length > 0;

        public T GetFirst()
        {
            if (Input.Length == 0)
                return default;
            else
                return Input[0];
        }

        private Func<T> getDefaultValue = null;

        public InputPort(string name,Func<T> getDefaultValue, PortCapacityMode capacityMode = PortCapacityMode.Single) : base(name, capacityMode, PortDirection.Input)
        {
            this.getDefaultValue = getDefaultValue;
        }

        public InputPort(string name, PortCapacityMode capacityMode = PortCapacityMode.Single) : base(name, capacityMode, PortDirection.Input)
        {
        }

        internal override void UpdateInputPortValues()
        {
            List<T> inputValues = new List<T>();
            foreach (BasePort reference in references)
            {
                if(reference.Direction == PortDirection.Output &&
                    LogicConverter.TryCast<T>(reference.GetOutputValue(),out T value))
                {
                    inputValues.Add(value);
                }
            }
            Input = inputValues.ToArray();
        }
    }

    public class OutputPort<T> : Port<T>
    {
        public T Output { get; set; }

        public OutputPort(string name, PortCapacityMode capacityMode = PortCapacityMode.Multiple) : base(name, capacityMode, PortDirection.Output)
        {
        }

        internal override object GetOutputValue()
        {
            return Output;
        }
    }

    public abstract class Port<T> : BasePort
    {
        public Port(string name, PortCapacityMode capacityMode, PortDirection direction) : base(name, capacityMode, direction, typeof(T))
        {
        }
    }

    public abstract class BasePort
    {
        public string Name { get; }

        public PortCapacityMode CapacityMode { get; }

        public PortDirection Direction { get; }

        public Type PortType { get; }

        public Connection Parent { get; internal set; }

        protected List<BasePort> references = new List<BasePort>();

        public BasePort(string name, PortCapacityMode capacityMode, PortDirection direction,Type portType)
        {
            Name = name;
            CapacityMode = capacityMode;
            Direction = direction;
            PortType = portType;
        }

        internal void AddReference(BasePort reference)
        {
            if(!references.Contains(reference))
                references.Add(reference);
        }

        internal void RemoveReference(BasePort reference)
        {
            references.Remove(reference);
        }

        private void ClearReferences()
        {
            references.Clear();
        }

        public IEnumerable<BasePort> GetReferences()
        {
            return references;
        }

        internal bool IsReferencesUpdateDepending()
        {
            foreach(BasePort reference in references)
            {
                if (reference.Parent.IsUpdateDepending || !reference.Parent.wasInitCon)
                    return true;
            }
            return false;
        }

        internal void UpdateReferences()
        {
            foreach (BasePort reference in references)
            {
                reference.Parent.UpdateConnection();
            }
        }

        internal virtual void UpdateInputPortValues()
        {
        }

        internal virtual object GetOutputValue() { return null; }

        public static void Connect(BasePort input,BasePort output)
        {
            if (input.Direction == output.Direction || !IsCompatible(input.PortType,output.PortType) ||
                Equals(input.Parent, output.Parent))
                return;
            if (Connected(input, output))
                return;

            if(input.CapacityMode == PortCapacityMode.Single)
            {
                input.references.AsParallel().ForAll(r => Disconnect(input, r));
            }
            if(output.CapacityMode == PortCapacityMode.Single)
            {
                output.references.AsParallel().ForAll(r => Disconnect(r, output));
            }

            input.AddReference(output);
            output.AddReference(input);

            input.Parent.UpdateConnection();
        }

        public static bool IsCompatible(Type inputType,Type outputType)
        {
            return LogicConverter.IsCastable(outputType,inputType);
        }

        public static void DisconnectAll(BasePort port)
        {
            foreach (BasePort p in port.references)
                p.RemoveReference(port);
            port.ClearReferences();
        }

        public static void Disconnect(BasePort input,BasePort output)
        {
            if (!input.references.Contains(output)&& output.references.Contains(input))
                return;

            input.RemoveReference(output);
            output.RemoveReference(input);

            input.Parent.UpdateConnection();
        }

        public static bool Connected(BasePort input, BasePort output)
        {
            if (input.Parent == null || output.Parent == null)
                return false;
            return input.references.Contains(output) && output.references.Contains(input);
        }
    }
}
