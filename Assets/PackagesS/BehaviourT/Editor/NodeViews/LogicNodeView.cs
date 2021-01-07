using UnityEngine;

namespace BehaviourT.Editor
{
    public class LogicNodeView : NodeView<LogicNode>
    {
        public LogicNodeView(LogicNode nodeData, Vector2 position) : base(nodeData, position)
        {
        }
    }
}
