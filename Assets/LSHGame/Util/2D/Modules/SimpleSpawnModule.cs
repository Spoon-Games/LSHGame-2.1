using UnityEngine;

namespace LSHGame.Util
{
    public class SimpleSpawnModule : MonoBehaviour
    {
        [SerializeField]
        public GameObject Prefab;

        [SerializeField]
        public Matrix4x4 trs = Matrix4x4.identity;

        public void Spawn()
        {
            Spawn(Matrix4x4.identity);
        }

        public void Spawn(Matrix4x4 trs)
        {
            trs = transform.localToWorldMatrix * this.trs * trs;
            SpawnGlobal(trs);
        }

        public void SpawnGlobal(Matrix4x4 trs)
        {
            Instantiate(Prefab, trs.MultiplyPoint(Vector3.zero), trs.rotation);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix * trs;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
        }

#endif
    }
}
