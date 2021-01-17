using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Decorators/Return Decorator")]
    public class ReturnDecorator : DecoratorTask
    {
        public enum ReturnType
        {
            Nothing,
            Failure,
            Success,
            Invert
        }

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.MainContainer,"Return Type")]
        private ReturnType returnType;

        protected override TaskState OnEvaluate()
        {
            if (Child == null)
                return TaskState.Success;

            TaskState childState = Child.Evaluate();
            if (childState == TaskState.Running)
                return TaskState.Running;

            switch (returnType)
            {
                case ReturnType.Nothing:
                    return Child.Evaluate();
                case ReturnType.Failure:
                    return TaskState.Failure;
                case ReturnType.Success:
                    return TaskState.Success;
                case ReturnType.Invert:
                    if (childState == TaskState.Success)
                        return TaskState.Failure;
                    else
                        return TaskState.Success;
            }

            return TaskState.Success;
        }
    }
}