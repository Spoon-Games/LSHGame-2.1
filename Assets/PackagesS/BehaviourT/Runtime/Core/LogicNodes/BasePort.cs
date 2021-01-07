using System;

namespace BehaviourT
{
    public class OutputPort<T> : Port<T>
    {
        public T Output { get => getOutput.Invoke();}
        private readonly Func<T> getOutput;

        public OutputPort(string name,Func<T> getOutput) : base(name)
        {
            this.getOutput = getOutput ?? throw new Exception("You have to specify a getOutput Function for the outputport " + name);
        }

        public override Direction GetDirection()
        {
            return Direction.output;
        }
    }

    public abstract class Port<T> : BasePort
    {
        public Port(string name) : base(name)
        {
        }

        public override Type GetPortType()
        {
            return typeof(T);
        }
    }

    public abstract class BasePort
    {
        public enum Direction { input,output}
        public readonly string Name;

        public BasePort(string name)
        {
            this.Name = name;
        }

        internal virtual void TryConnectOutputPort(SerInputPort serInputPort)
        {

        }

        public abstract Direction GetDirection();

        public abstract Type GetPortType();
    }
}
