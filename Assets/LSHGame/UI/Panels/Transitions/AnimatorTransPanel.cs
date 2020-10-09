

using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class AnimatorTransPanel : TransitionPanel
    {
        [SerializeField]
        protected Slider slider;

        protected Animator Animator { get; private set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        protected override void OnSetProgress(float progress)
        {
            Animator.SetFloat("Progress", progress);
            slider?.SetValueWithoutNotify(progress);

            base.OnSetProgress(progress);
        }

        protected override void OnSwitchState(State state)
        {
            switch (state)
            {
                case State.Idle:
                    Animator.SetTrigger("TriggerIdle");
                    break;
                case State.Start:
                    Animator.SetTrigger("TriggerStart");
                    break;
                case State.Middle:
                    Animator.SetTrigger("TriggerMiddle");
                    break;
                case State.End:
                    Animator.SetTrigger("TriggerEnd");
                    break;
            }
        }
    }
}