using UnityEngine;

namespace PathC
{
    public abstract class EditablePathBehaviour : PathBehaviour
    {
        [SerializeField]
        [HideInInspector]
        protected bool isClosed;

        public bool IsClosed
        {
            get => isClosed;
            set => SetIsClosed(value);
        }

        public abstract int PointCount { get; }

        public abstract int SegmentCount { get; }

        public abstract void AddSegment(Vector2 point);

        public abstract void InsertSegment(int index, Vector2 point);

        public abstract void DeleteSegment(int index);

        public abstract void MovePoint(int index, Vector2 point);

        public abstract Vector2 GetPoint(int index);

        public abstract void SetIsClosed(bool value);
    }
}
