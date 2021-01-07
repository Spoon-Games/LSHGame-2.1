using UnityEngine;

namespace BehaviourT
{
    [AddComponentMenu("Tasks/Composites/Paralell")]
    [System.Serializable]
    public class Paralell : CompositeTask
    {
        public enum ParalellAbortType { OnFailure, OnSuccess, DoAll}

        [SerializeField]
        [NodeEditorField(label: "Abort Type")]
        private ParalellAbortType abortType = ParalellAbortType.OnFailure;

        protected internal override void Awake()
        {
            base.Awake();
            Children.TaskPriorities = new int[Children.TasksCount];
        }

        protected override TaskState OnEvaluate()
        {
            bool allFinished = true;
            foreach(Task task in Children)
            {
                switch (task.Evaluate())
                {
                    case TaskState.Running:
                        allFinished = false;
                        break;
                    case TaskState.Failure:
                        if (abortType == ParalellAbortType.OnFailure)
                            return TaskState.Failure;
                        break;
                    case TaskState.Success:
                        if (abortType == ParalellAbortType.OnSuccess)
                            return TaskState.Success;
                        break;
                }
            }
            if (allFinished)
                return TaskState.Success;

            return TaskState.Running;
        }
    }
}
