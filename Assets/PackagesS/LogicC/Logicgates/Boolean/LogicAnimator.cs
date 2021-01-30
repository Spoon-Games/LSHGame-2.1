using UnityEngine;

namespace LogicC
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("LogicC/Boolean/Logic Animator")]
    public class LogicAnimator : LogicDestination
    {
        public override string Title => "Boolean Animator";

        [NodeEditorField]
        [SerializeField]
        private string[] triggerBools;

        private Animator animator;

        protected override void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected override void OnSetValue(bool value)
        {
            base.OnSetValue(value);

            foreach(var trig in triggerBools)
            {
                animator.SetBool(trig, value);
            }
        }
    }
}
