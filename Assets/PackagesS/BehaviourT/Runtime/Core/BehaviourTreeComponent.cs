using UnityEngine;

namespace BehaviourT
{
    public class BehaviourTreeComponent : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTree _behaviourTree;
        public BehaviourTree BehaviourTree { get => _behaviourTree; set
            {
                _behaviourTree.Destroy();
                _behaviourTree = value;
                _behaviourTree.Initialize();
            }
        }

        [SerializeField]
        public bool Run = true;

        protected virtual void Awake()
        {
            BehaviourTree?.Initialize();
        }

        protected virtual void FixedUpdate()
        {
            if (Run)
                BehaviourTree?.Update();

        }

        protected virtual void OnDestroy()
        {
            BehaviourTree?.Destroy();
        }

        public bool TrySetValue(string name,object value)
        {
            if (BehaviourTree == null)
                return false;

            return BehaviourTree.TrySetValue(name, value);
        }

        public T GetValue<T>(string name)
        {
            if (TryGetValue<T>(name, out T value))
                return value;
            return default;
        }

        public bool TryGetValue<T>(string name,out T value)
        {
            if(BehaviourTree == null)
            {
                value = default;
                return false;
            }
            return BehaviourTree.TryGetValue<T>(name,out value);
        }

        public void Reset()
        {
            BehaviourTree?.Reset();
        }
    }
}
