namespace BehaviourT
{
    [System.Serializable]
    public sealed class RootTask : DecoratorTask
    {
        protected override TaskState OnEvaluate()
        {
            if (Child == null)
                return TaskState.Failure;

            return Child.Evaluate();
        }
    }
}
