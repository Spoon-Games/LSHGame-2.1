using UnityEngine;

namespace BehaviourT
{
    [AddComponentMenu("Tasks/Decorators/Repeater")]
    [System.Serializable]
    public class Repeater : DecoratorTask
    {

        public enum AbortType
        {
            None,
            Failure,
            Success
        }

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.MainContainer, "Abort Type")]
        private AbortType abortType = AbortType.None;

        [SerializeField]
        [NodeEditorField(label: "Cicles")]
        private int cicles = -1;

        private int cicleIndex = 0;
        private TaskState lastChildState = TaskState.NotEvaluated;

        protected override TaskState OnEvaluate()
        {
            if(this.State == TaskState.NotEvaluated)
            {
                cicleIndex = 0;
                lastChildState = TaskState.NotEvaluated;
            }



            if ((lastChildState == TaskState.Success && abortType != AbortType.Success) ||
                (lastChildState == TaskState.Failure && abortType != AbortType.Failure))
                Child.ResetState();

            lastChildState = Child.Evaluate();

            if(lastChildState != TaskState.Running)
            {
                cicleIndex++;
            }

            if(cicles > 0 && cicleIndex >= cicles && lastChildState != TaskState.Running)
            {
                return TaskState.Success;
            }

            if(lastChildState == TaskState.Success && abortType == AbortType.Success ||
                lastChildState == TaskState.Failure && abortType == AbortType.Failure)
            {
                return lastChildState;
            }

            return TaskState.Running;

        }
    }
}
