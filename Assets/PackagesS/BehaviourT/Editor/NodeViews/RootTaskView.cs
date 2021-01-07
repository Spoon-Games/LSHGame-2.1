using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourT.Editor
{
    public class RootTaskView : NodeView<RootTask>
    {
        FlowPort outputPort;

        public RootTaskView(RootTask nodeData, Vector2 position) : base(nodeData, position)
        {
            outputPort = base.CreateFlowPort(Direction.Output, Port.Capacity.Single);

            this.capabilities &= ~Capabilities.Deletable;

            RefreshExpandedState();
            RefreshPorts();
        }

        internal override Node Serialize()
        {
            if (outputPort.TrySerialize(out Task task))
            {
                NodeData.SerializeChild(task);
            }
            return base.Serialize();
        }

        internal override void Deserialize(List<NodeView> nodeViews,TreeGraphView graphView)
        {
            base.Deserialize(nodeViews,graphView);

            if (NodeData.Child != null)
            {
                NodeView connectToView = nodeViews.FirstOrDefault(nv => Equals(nv.GetNodeData(), NodeData.Child));
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
