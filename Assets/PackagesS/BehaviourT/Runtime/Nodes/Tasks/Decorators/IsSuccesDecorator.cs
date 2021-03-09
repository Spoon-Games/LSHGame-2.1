using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Decorators/Is Success Decorator")]
    public class IsSuccesDecorator : ReturnDecorator
    {
        protected OutputPort<bool> isSuccessPort;

        private bool isSuccess = false;

        protected internal override void GetPorts(PortList portList)
        {
            isSuccessPort = new OutputPort<bool>("Is Success", () => isSuccess);
            portList.Add(isSuccessPort);
            base.GetPorts(portList);
        }

        protected override TaskState OnEvaluate()
        {
            if (Child == null)
                isSuccess = false;

            TaskState childState = Child.Evaluate();
            isSuccess = childState == TaskState.Success;
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
