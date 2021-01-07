using System;
using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    public abstract class ExposedProperty<T> : ExposedProperty
    {
        private T value = default;
        private bool wasSet = false;

        protected internal override void GetPorts(PortList portList)
        {
            portList.Add(new OutputPort<T>("value", GetOutput));
        }

        private T GetOutput()
        {
            if (!wasSet)
                return GetDefault();
            else
                return value;
        }

        protected abstract T GetDefault();

        public override bool TrySetValue(object value)
        {
            if(value is T v)
            {
                this.value = v;
                wasSet = true;
                return true;
            }
            return false;
        }

        public override bool TryGetValue<V>(out V value)
        {
            if(this.value is V v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public override Type PropertyType => typeof(T);

    }

    [System.Serializable]
    public abstract class ExposedProperty : LogicNode
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.MainContainer,"Name")]
        internal protected string name = "Property";
        public string PropertyName => name;

        public abstract Type PropertyType { get; }

        public abstract bool TrySetValue(object value);

        public abstract bool TryGetValue<T>(out T value);
    }
}
