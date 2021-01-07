using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviourT.Editor
{
    public class DecoratorTaskView : NodeView<DecoratorTask>
    {
        FlowPort outputPort;

        public DecoratorTaskView(DecoratorTask nodeData, Vector2 position) : base(nodeData, position)
        {
            base.CreateFlowPort(Direction.Input, Capacity.Single);
            outputPort = base.CreateFlowPort(Direction.Output, Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();
        }

        internal override Node Serialize()
        {
            if(outputPort.TrySerialize(out Task task))
            {
                NodeData.SerializeChild(task);
            }
            return base.Serialize();
        }

        internal override void Deserialize(List<NodeView> nodeViews, TreeGraphView graphView)
        {
            base.Deserialize(nodeViews,graphView);

            if(NodeData.Child != null)
            {
                NodeView connectToView = nodeViews.FirstOrDefault(nv => Equals(nv.GetNodeData(), NodeData.Child));
                if (connectToView != null)
                {
                    FlowPort input = GetFlowPort(Direction.Input, connectToView);
                    FlowPort output = GetFlowPort(Direction.Output,this);
                    graphView.CreateEdge(input, output);
                }

            }
        }
    }
}
