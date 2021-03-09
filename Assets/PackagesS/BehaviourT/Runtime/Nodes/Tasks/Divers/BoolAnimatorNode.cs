using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Divers/Boolean Animator Node")]
    [RequireComponent(typeof(Animator))]
    public class BoolAnimatorNode : Task
    {
        private InputPort<bool> paramPort;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer, hideLabel: true, portContainer: "Is Active")]
        private bool defaultIsActive = false;

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
            paramPort = new InputPort<bool>("Is Active", () => defaultIsActive);
            portList.Add(paramPort);
            base.GetPorts(portList);
        }

        protected override TaskState OnEvaluate()
        {
            animator.SetBool(parameterName, paramPort.Input);

            return TaskState.Success;
        }

    }
}
