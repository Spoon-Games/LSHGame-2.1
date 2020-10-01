using System;
using UnityEngine;

namespace LogicC
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("LogicC/Float/Float Animator")]
    public class FloatAnimator : FloatDestination
    {
        public override string Title => "Float Animator";

        [NodeEditorField]
        [SerializeField]
        private string[] triggerFloats;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected override void OnSetValue(float value)
        {
            base.OnSetValue(value);

            foreach (var trig in triggerFloats)
            {
                animator.SetFloat(trig, value);
            }
        }

    }
}
