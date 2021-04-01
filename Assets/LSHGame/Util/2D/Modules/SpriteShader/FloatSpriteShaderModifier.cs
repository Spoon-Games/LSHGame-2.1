using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteInEditMode]
    public class FloatSpriteShaderModifier : MonoBehaviour
    {
        public string ParameterName;

        [SerializeField]
        private float value;
        private float privateValue;

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                if (Application.isEditor)
                {
                    spriteRenderer.sharedMaterial.SetFloat(ParameterName, value);
                }
                else if (spriteRenderer.material != null)
                        spriteRenderer.material.SetFloat(ParameterName, value);
                privateValue = value;
                this.value = value;
            }
        }

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if(privateValue != value)
            {
                Value = value;
            }
        }
    }
}
