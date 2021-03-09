using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Divers/Integer Animator Node")]
    [RequireComponent(typeof(Animator))]
    public class IntAnimatorNode : Task
    {
        private InputPort<int> paramPort;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "Value")]
        private int defaultValue = 0;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.MainContainer, "Parameter Name")]
        private string parameterName;

        private Animator animator;

        protected internal override void Awake()
        {
            base.Awake();
            animator = Component.GetComponent<Animator>();
        }

        protected internal override void GetPorts(PortList portList)
        {
            paramPort = new InputPort<int>("Value", () => defaultValue);
            portList.Add(paramPort);
            base.GetPorts(portList);
        }

        protected override TaskState OnEvaluate()
        {
            animator.SetInteger(parameterName, paramPort.Input);

            return TaskState.Success;
        }

    }
}
