using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    public class GroupDataExNode : DataExNode
    {
        [SerializeField]
        public string GroupName;

        [SerializeField]
        public Rect GroupPosition;

        [SerializeReference]
        public Node[] children;

        public GroupDataExNode(string groupName, Rect groupPosition, Node[] children)
        {
            GroupName = groupName;
            GroupPosition = groupPosition;
            this.children = children;
        }
    }
}
