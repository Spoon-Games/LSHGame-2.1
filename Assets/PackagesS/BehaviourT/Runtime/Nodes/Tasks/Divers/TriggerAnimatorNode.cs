using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Divers/Trigger Animator Node")]
    [RequireComponent(typeof(Animator))]
    public class TriggerAnimatorNode : Task
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.MainContainer, "Parameter Name")]
        private string parameterName;

        private Animator animator;

        protected internal override void Awake()
        {
            base.Awake();
            animator = Component.GetComponent<Animator>();
        }


        protected override TaskState OnEvaluate()
        {
            animator.SetTrigger(parameterName);

            return TaskState.Success;
        }

    }
}
