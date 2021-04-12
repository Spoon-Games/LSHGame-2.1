using FMODUnity;
using UnityEngine;

namespace LSHGame.Util
{
    public class FModParameterSetter : MonoBehaviour
    {
        [SerializeField]
        private StudioEventEmitter emitter;

        [SerializeField]
        private string parameter;

        [SerializeField]
        public float Value = 0;

        private float _value = 0;

        private void Awake()
        {
            if(emitter == null)
                emitter = GetComponent<StudioEventEmitter>();

            if (emitter == null)
                Debug.LogError("No StudioEventEmitter found");
        }

        private void Update()
        {
            if(Value != _value)
            {
                _value = Value;
                emitter.SetParameter(parameter, Value);
            }
        }

    }
}
