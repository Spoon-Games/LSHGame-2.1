using UnityEngine;

namespace LSHGame.Util
{

    public class VFXTriggerSMB : StateMachineBehaviour
    {
        [SerializeField]
        private string vFXName;
        [SerializeField]
        private float startDelay = 0;
        [SerializeField]
        private bool OnEnter = true;
        [SerializeField]
        private bool OnExit = false;
        [SerializeField]
        private float TriggerAgainThreshold = 0.1f;
        private float triggerAgainTimer = float.NegativeInfinity;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnEnter)
            {
                Trigger(animator.transform);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnExit)
            {
                Trigger(animator.transform);
            }
        }

        private void Trigger(Transform transform)
        {
            if (Time.time < triggerAgainTimer)
                return;

            triggerAgainTimer = Time.time + TriggerAgainThreshold;

            Bundle bundle = new Bundle();
            bundle.Put("startDelay", startDelay);

            var spriteRender = transform.GetComponent<SpriteRenderer>();
            if (spriteRender)
                bundle.Put("flip",spriteRender.flipX);

            if (string.IsNullOrEmpty(vFXName))
                return;
            transform.GetComponent<EffectsController>().TriggerEffectParams(vFXName, bundle);
        }
    } 
}
