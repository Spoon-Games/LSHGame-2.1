using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourT.Editor
{
    public class GroupView : Group
    {
        public GroupView(string title,Rect position)
        {
            this.title = title;
            SetPosition(position);
            capabilities |= Capabilities.Resizable;
        }

        public GroupView(string title, Vector2 position) : this(title,new Rect(position,new Vector2(200,200)))
        {
        }

        public GroupDataExNode Serialize()
        {
            var childNodes = from graphElement in containedElements
                             where graphElement is NodeView
                             select (graphElement as NodeView).GetNodeData();

            return new GroupDataExNode(title, GetPosition(), childNodes.ToArray());
        }

        public void Deserialize(GroupDataExNode data, List<NodeView> nodes)
        {
            var toAddNodes = from node in nodes
                             join  requestedNodes in data.children on node.GetNodeData() equals requestedNodes
                             select node;
            this.AddElements(toAddNodes);
        }
    }
}
