using UnityEngine;

namespace PathC
{
    [ExecuteInEditMode]
    public abstract class PathBehaviour : MonoBehaviour
    {
        private VertexPath vertexPath;
        public VertexPath VertexPath
        {
            get
            {
                if (vertexPath == null)
                {
                    vertexPath = new VertexPath();
                    UpdateVertexPath();
                }
                return vertexPath;
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool isInit = false;

        [SerializeField]
        private bool updateTransformChange = false;

        private Matrix4x4 worldToLocalMatrix;
        private Matrix4x4 localToWorldMatrix;

        protected void Awake()
        {
            if (!isInit)
            {
                Initialize();
                isInit = true;
            }
            worldToLocalMatrix = transform.worldToLocalMatrix;
            localToWorldMatrix = transform.localToWorldMatrix;
        }

        public void Reset()
        {
            Initialize();
        }

        private void Update()
        {
            if (updateTransformChange && transform.hasChanged)
            {
                UpdateVertexPath();
            }
        }

        protected virtual void Initialize() { }

        protected abstract void UpdateVertexPath();

        protected Vector2 WorldToLocal(Vector2 pos)
        {
            if (!updateTransformChange && Application.isPlaying)
            {
                return worldToLocalMatrix.MultiplyPoint(pos);
            }
            return transform.worldToLocalMatrix.MultiplyPoint(pos);
        }

        protected Vector2 LocalToWorld(Vector2 pos)
        {
            if (!updateTransformChange && Application.isPlaying)
            {
                return localToWorldMatrix.MultiplyPoint(pos);
            }
            return transform.localToWorldMatrix.MultiplyPoint(pos);
        }
    }
}
