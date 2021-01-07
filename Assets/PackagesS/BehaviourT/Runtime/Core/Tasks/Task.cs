using System;
using UnityEngine;

namespace BehaviourT
{
    public enum TaskState
    {
        NotEvaluated,
        Running,
        Failure,
        Success
    }

    [Serializable]
    public abstract class Task : Node
    {
        public TaskState State { get; private set; } = TaskState.NotEvaluated;

        [SerializeField]
        private bool EvaluatingWhenFailure;

        [SerializeField]
        private bool EvaluatingWhenSuccess;

        public TaskState Evaluate()
        {
            if (State == TaskState.Failure || State == TaskState.Success)
                return State;

            State = OnEvaluate();

            if (State == TaskState.NotEvaluated) // Fail-save when wrong implementing OnEvaluate
                State = TaskState.Running;

            return State;
        }

        protected abstract TaskState OnEvaluate();


        internal virtual bool PostUpdateEvaluate()
        {
            if(EvaluatingWhenFailure && State == TaskState.Failure ||
                EvaluatingWhenSuccess && State == TaskState.Success)
            {
                ResetState();
                TaskState potState = OnEvaluate();

                if (potState == TaskState.NotEvaluated)
                    potState = TaskState.Running;

                if(State != potState)
                {
                    State = potState;
                    return true;
                }
            }

            return false;
        }


        internal void ResetState()
        {
            ResetSelfState();
            ResetChildrenState();
        }

        internal void ResetSelfState()
        {
            State = TaskState.NotEvaluated;
        }

        internal virtual void ResetChildrenState() { }
    }
}
