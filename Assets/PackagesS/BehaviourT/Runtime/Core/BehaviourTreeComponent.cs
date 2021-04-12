using UnityEngine;

namespace BehaviourT
{
    public class BehaviourTreeComponent : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTree _behaviourTree;
        public BehaviourTree BehaviourTreeObjectReference
        {
            get => _behaviourTree; set
            {
                _behaviourTree = value;
                InitializeInstance();
            }
        }

        private BehaviourTree _behaviourTreeInstance;
        public BehaviourTree BehaviourTreeInstance { get => _behaviourTreeInstance; }

        [SerializeField]
        public bool Run = true;

        private bool isAwoken = false;

        protected virtual void Awake()
        {
            isAwoken = true;
            InitializeInstance();
        }

        private void InitializeInstance()
        {
            _behaviourTreeInstance?.Destroy();

            if (BehaviourTreeObjectReference != null && isAwoken)
            {
                _behaviourTreeInstance = Instantiate(BehaviourTreeObjectReference);
                _behaviourTreeInstance.Initialize(this);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (Run)
                BehaviourTreeInstance?.Update();

        }

        protected virtual void OnDestroy()
        {
            
            if(BehaviourTreeInstance != null)
            {
                BehaviourTreeInstance.Destroy();
                Destroy(BehaviourTreeInstance);
                _behaviourTreeInstance = null;
            }
        }

        public bool TrySetValue(string name, object value)
        {
            if (BehaviourTreeInstance == null)
                return false;

            return BehaviourTreeInstance.TrySetValue(name, value);
        }

        public T GetValue<T>(string name)
        {
            if (TryGetValue<T>(name, out T value))
                return value;
            return default;
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            if (BehaviourTreeInstance == null)
            {
                value = default;
                return false;
            }
            return BehaviourTreeInstance.TryGetValue<T>(name, out value);
        }

        public void Reset()
        {
            BehaviourTreeInstance?.Reset();
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            BehaviourTreeObjectReference?.DrawGizmos(this);
        }
#endif
    }
}
