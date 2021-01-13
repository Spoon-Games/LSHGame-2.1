using UnityEditor;
using UnityEngine;

namespace LogicC
{
    public class LogicSpawner2D : LogicDestination
    {
        [SerializeField]
        public GameObject prefab;

        [Header("Spawn")]
        [SerializeField]
        private Rect spawnArea;

        [SerializeField]
        private float rotation;

        [SerializeField]
        private float angle = 0;

        [Header("Burst")]
        [SerializeField]
        private int cicles = -1;
        private int cicleIndex = 0;

        [SerializeField]
        private float intervall = 1;

        [SerializeField]
        private int count = 1;

        [SerializeField]
        private float timeOffset = 0;
        private float spawnTimer = float.PositiveInfinity;

        [SerializeField]
        private float animationOffset = 0;

        private Animator animator;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (base.Active)
            {
                spawnTimer = Time.fixedTime + timeOffset + animationOffset;
                cicleIndex = 0;
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            spawnTimer = Time.fixedTime + timeOffset + animationOffset;
            cicleIndex = 0;
            Spawn();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            animator?.Play("Idle");
        }

        private void FixedUpdate()
        {
            Spawn();
        }

        private void Spawn()
        {
            if (base.Active && spawnTimer - animationOffset <= Time.fixedTime && (cicleIndex < cicles || cicles <= 0) && count > 0 && prefab != null)
            {
                animator?.SetBool("Fired", true);
            }

            if (base.Active && spawnTimer <= Time.fixedTime && (cicleIndex < cicles || cicles <= 0) && count > 0 && prefab != null)
            {
                for (int i = 0; i < count; i++)
                {
                    SpawnSingle();
                }
                spawnTimer = Time.fixedTime + intervall;
                cicleIndex++;
                animator?.SetBool("Fired", false);
            }

            if (!base.Active)
            {
                animator?.SetBool("Fired", false);
            }
        }

        private void SpawnSingle()
        {
            Matrix4x4 trs = GetRandomTRS();
            GameObject.Instantiate(prefab, trs.MultiplyPoint(Vector3.zero), trs.rotation);
        }

        private Matrix4x4 GetRandomTRS()
        {
            Vector3 position = new Vector3(Random.Range(spawnArea.xMin, spawnArea.xMax), Random.Range(spawnArea.yMin, spawnArea.yMax));
            float r = Random.Range(rotation - angle / 2, rotation + angle / 2);
            return transform.localToWorldMatrix * Matrix4x4.TRS(position, Quaternion.Euler(0, 0, r), Vector3.one);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Handles.color = Color.blue;

            Gizmos.matrix = transform.localToWorldMatrix;
            Handles.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(spawnArea.center, spawnArea.size);
            Handles.DrawSolidArc(spawnArea.position, Vector3.forward, Quaternion.Euler(0, 0, rotation - angle / 2) * Vector3.up, angle, 1);

            Gizmos.matrix = Matrix4x4.identity;
            Handles.matrix = Matrix4x4.identity;
        }
#endif
    }
}
