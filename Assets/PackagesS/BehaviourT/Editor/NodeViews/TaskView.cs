using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourT.Editor
{
    public class TaskNodeView : NodeView<Task>
    {
        public TaskNodeView(Task nodeData, Vector2 position) : base(nodeData, position)
        {
            base.CreateFlowPort(Direction.Input, Port.Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
