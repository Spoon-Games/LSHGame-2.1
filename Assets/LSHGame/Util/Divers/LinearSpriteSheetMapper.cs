using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public class LinearSpriteSheetMapper : StateMachineBehaviour
    {
        [SerializeField]
        private Sprite[] sprites;

        [SerializeField]
        private string parameterName;

        [SerializeField]
        private float SpritesPerUnit = 1;

        [SerializeField]
        private float offset = 0;

        [SerializeField]
        private bool looping = true;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private int parameterHash = -1;

        private void Awake()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if(spriteRenderer == null)
            {
                spriteRenderer = animator.GetComponent<SpriteRenderer>();
            }
            this.animator = animator;
            TimeSystem.OnLateUpdate += UpdateSprite;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            TimeSystem.OnLateUpdate -= UpdateSprite;
        }

        private void OnDisable()
        {
            TimeSystem.OnLateUpdate -= UpdateSprite;
        }


        private void UpdateSprite()
        {
            if (spriteRenderer == null || sprites == null || sprites.Length == 0 || animator == null)
                return;

            float v = animator.GetFloat(parameterHash);
            int index = (int) ((v - offset) * SpritesPerUnit );
            if(looping)
                index = mod(index, sprites.Length);
            else
            {
                index = Mathf.Clamp(index, 0, sprites.Length -1);
            }
            spriteRenderer.sprite = sprites[index];

            //Debug.Log("UpdateSprite: " + index + " Value: " + v);
        }

        int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
