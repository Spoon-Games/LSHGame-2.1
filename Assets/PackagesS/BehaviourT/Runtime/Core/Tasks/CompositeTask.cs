using System.Collections.Generic;
using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    public abstract class CompositeTask : Task
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.Hide)]
        private TaskArray _children;
        public TaskArray Children
        {
            get
            {
                if (_children == null)
                    _children = new TaskArray(new Task[0]);
                return _children;
            }
            private set => _children = value;
        }


        internal override bool PostUpdateEvaluate()
        {
            if (base.PostUpdateEvaluate())
            {
                return true;
            }

            bool childWasUpdated = false;
            int childPriority = 0;

            foreach (KeyValuePair<int, Task> child in Children.PriorEnumerable)
            {
                if (!childWasUpdated || (childWasUpdated && child.Key == childPriority))
                {
                    if (child.Value.PostUpdateEvaluate())
                    {
                        childWasUpdated = true;
                        childPriority = child.Key;
                    }
                }
                else
                {
                    child.Value.ResetState();
                }
            }

            if (childWasUpdated)
                ResetSelfState();

            return childWasUpdated;
        }

        internal override void ResetChildrenState()
        {
            base.ResetChildrenState();

            foreach (Task child in Children)
            {
                child.ResetState();
            }
        }

        public void SerializeTaskArray(TaskArray children)
        {
            Children = children;
        }
    }
}
