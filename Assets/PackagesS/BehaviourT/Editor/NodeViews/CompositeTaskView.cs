using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviourT.Editor
{
    public class CompositeTaskView : NodeView<CompositeTask>
    {
        FlowPort outputPort;

        public CompositeTaskView(CompositeTask nodeData, Vector2 position) : base(nodeData, position)
        {
            base.CreateFlowPort(Direction.Input, Capacity.Single);
            outputPort = base.CreateFlowPort(Direction.Output, Capacity.Multi);

            RefreshExpandedState();
            RefreshPorts();
        }

        internal override Node Serialize()
        {
            if(outputPort.TrySerialize(out TaskArray taskArray))
            {
                NodeData.SerializeTaskArray(taskArray);
            }
            return base.Serialize();
        }

        internal override void Deserialize(List<NodeView> nodeViews, TreeGraphView graphView)
        {
            base.Deserialize(nodeViews,graphView);

            foreach(Task task in NodeData.Children)
            {
                NodeView connectToView = nodeViews.FirstOrDefault(nv => Equals(nv.GetNodeData(), task));
                if (connectToView != null)
                {
                    FlowPort input = GetFlowPort(Direction.Input, connectToView);
                    FlowPort output = GetFlowPort(Direction.Output, this);
                    graphView.CreateEdge(input, output);
                }

            }
        }
    }
}
