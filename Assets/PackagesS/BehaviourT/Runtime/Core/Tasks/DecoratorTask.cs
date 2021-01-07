using UnityEngine;

namespace BehaviourT
{
    public abstract class DecoratorTask : Task
    {
        [SerializeReference]
        [NodeEditorField(NodeEditorField.NodePlace.Hide)]
        private Task child;

        public Task Child { get => child; private set => child = value; }

        internal override bool PostUpdateEvaluate()
        {
            if (base.PostUpdateEvaluate())
            {
                return true;
            }

            if (Child == null)
                return false;

            if (Child.PostUpdateEvaluate())
            {
                ResetSelfState();
                return true;
            }
            return false;
        }

        internal override void ResetChildrenState()
        {
            base.ResetChildrenState();

            child?.ResetState();
        }

        public void SerializeChild(Task child)
        {
            Child = child;
        }
    }
}
