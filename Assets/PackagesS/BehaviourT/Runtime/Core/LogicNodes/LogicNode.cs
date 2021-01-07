namespace BehaviourT
{
    [System.Serializable]
    public abstract class LogicNode : Node
    {
        internal protected abstract override void GetPorts(PortList portList);
    }
}
