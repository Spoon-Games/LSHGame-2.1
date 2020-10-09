using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathC
{
    public class LinearPath : EditablePathBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private List<Vector2> points = new List<Vector2>();

        [SerializeField]
        private float durration = 1;

        public override int PointCount => points.Count;

        public override int SegmentCount => points.Count - (isClosed?0:1);

        protected override void Initialize()
        {
            base.Initialize();

            points.Add(-Vector2.one*0.5f);
            points.Add(Vector2.one*0.5f);
        }

        public override void AddSegment(Vector2 point)
        {
            points.Add(WorldToLocal(point));
            UpdateVertexPath();
        }

        public void GetSegment(int index,out Vector2 p1,out Vector2 p2)
        {
            p1 = LocalToWorld(points[LoopIndex(index)]);
            p2 = LocalToWorld(points[LoopIndex(index+1)]);
        }

        public override void DeleteSegment(int index)
        {
            if (points.Count <= 2 )
                return;
            points.RemoveAt(index);
            UpdateVertexPath();
        }

        public override Vector2 GetPoint(int index)
        {
            return LocalToWorld(points[index]);
        }

        public override void InsertSegment(int index, Vector2 point)
        {
            points.Insert(LoopIndex(index+1), WorldToLocal(point));
            UpdateVertexPath();
        }

        public override void MovePoint(int index, Vector2 point)
        {
            points[index] = WorldToLocal(point);
            UpdateVertexPath();
        }

        public override void SetIsClosed(bool value)
        {
            if (value != isClosed)
            {
                isClosed = value;
                UpdateVertexPath();
            }
        }

        protected override void UpdateVertexPath()
        {
            List<Vector2> points = new List<Vector2>(this.points);
            if (isClosed)
                points.Add(points[0]);
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = LocalToWorld(points[i]);
            }
            float[] timeStamps = new float[points.Count];

            float pathLength = 0;
            timeStamps[0] = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                pathLength += Vector2.Distance(points[i], points[i + 1]);
                timeStamps[i + 1] = pathLength;
            }

            string debugs = "";

            for (int i = 0; i < points.Count; i++)
            {
                timeStamps[i] *= durration / pathLength;
                debugs += timeStamps[i] + "\n";
            }

            //Debug.Log(debugs);
            VertexPath.Update(points.ToArray(), timeStamps, pathLength);
            //VertexPath.Update();
        }

        private int LoopIndex(int i) => (i + points.Count) % points.Count;
    }
}
