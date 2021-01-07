using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Composite/Sequence")]
    public class Sequence : CompositeTask
    {
        protected override TaskState OnEvaluate()
        {
            foreach(Task child in Children)
            {
                switch (child.Evaluate())
                {
                    case TaskState.Running:
                        return TaskState.Running;
                    case TaskState.Failure:
                        return TaskState.Failure;
                    case TaskState.Success:
                        break;
                }
            }

            return TaskState.Success;
        }
    }
}
