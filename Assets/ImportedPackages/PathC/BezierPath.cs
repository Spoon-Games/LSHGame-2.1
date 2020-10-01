using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathC
{
    public class BezierPath : EditablePathBehaviour
    {
        [SerializeField]
        private List<Vector2> anchPoints;

        [SerializeField]
        private List<Vector2> contPoints;

        [SerializeField]
        [HideInInspector]
        private bool isAutoSetControlPoints;

        [SerializeField]
        [HideInInspector]
        private float vertexResolution = 1;

        public float VertexResolution { get => vertexResolution;
            set {
                if (value == vertexResolution)
                    return;
                vertexResolution = value;
                UpdateVertexPath();
            } }

        [SerializeField]
        [HideInInspector]
        private float vertexSpacing = 0.05f;

        public float VertexSpacing
        {
            get => vertexSpacing;
            set
            {
                if (value == vertexSpacing)
                    return;
                vertexSpacing = value;
                UpdateVertexPath();
            }
        }

        public bool IsAutoSetcontrolPoints { get => isAutoSetControlPoints; set => SetIsAutoSetControlPoints(value); }

        public override int SegmentCount => PointCount - (!isClosed ? 1 : 0);

        public override int PointCount => anchPoints.Count;

        public int ContPointCount => contPoints.Count;

        protected override void Initialize()
        {
            anchPoints = new List<Vector2>()
            {
                Vector2.left,
                Vector2.right
            };
            contPoints = new List<Vector2>()
            {
                (Vector2.left + Vector2.up) * 0.5f,
                (Vector2.right + Vector2.down) * 0.5f
            };
        }

        public override void AddSegment(Vector2 anchorPos) => AddSegmentLocal(WorldToLocal(anchorPos));

        private void AddSegmentLocal(Vector2 localAnchorPos)
        {
            Vector2 contPoint1 = anchPoints.Last() * 2 - contPoints.Last();
            Vector2 contPoint2 = (contPoint1 + localAnchorPos) * 0.5f;

            contPoints.Add(contPoint1);
            contPoints.Add(contPoint2);
            anchPoints.Add(localAnchorPos);

            if (isAutoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(anchPoints.Count - 1);
            }

            UpdateVertexPath();
        }

        public void GetSegment(int i, out Vector2 anchP1, out Vector2 contP1, out Vector2 contP2, out Vector2 anchP2)
        {
            GetSegmentLocal(i, out Vector2 anchLocalP1, out Vector2 contLocalP1, out Vector2 contLocalP2, out Vector2 anchLocalP2);
            anchP1 = LocalToWorld(anchLocalP1);
            anchP2 = LocalToWorld(anchLocalP2);
            contP1 = LocalToWorld(contLocalP1);
            contP2 = LocalToWorld(contLocalP2);
        }

        private void GetSegmentLocal(int i, out Vector2 anchP1, out Vector2 contP1, out Vector2 contP2, out Vector2 anchP2)
        {
            anchP1 = anchPoints[LoopAnchIndex(i)];
            anchP2 = anchPoints[LoopAnchIndex(i + 1)];

            contP1 = contPoints[LoopContIndex(i * 2)];
            contP2 = contPoints[LoopContIndex(i * 2 + 1)];
        }

        public override void InsertSegment(int anchI, Vector2 anchPos) => InsertSegmentLocal(anchI, WorldToLocal(anchPos));

        private void InsertSegmentLocal(int anchI, Vector2 localAnchPos)
        {
            anchI += 1;
            anchPoints.Insert(anchI, localAnchPos);
            contPoints.InsertRange(anchI * 2 - 1, new Vector2[2]);

            if (isAutoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(anchI);
            }
            else
            {
                AutoSetContPoints(anchI);
            }
            UpdateVertexPath();
        }

        public override void DeleteSegment(int anchI)
        {
            if (anchPoints.Count <= 2)
                return;
            if (anchI == 0)
            {
                if (isClosed)
                {
                    contPoints[contPoints.Count - 1] = contPoints[2];
                }
                contPoints.RemoveRange(0, 2);
            }
            else if (anchI >= anchPoints.Count - 1 && !isClosed)
            {
                contPoints.RemoveRange(anchI * 2 - 2, 2);
            }
            else
            {
                contPoints.RemoveRange(anchI * 2 - 1, 2);
            }
            anchPoints.RemoveAt(anchI);

            AutoSetAllAffectedControlPoints(LoopAnchIndex(anchI - 1));

            UpdateVertexPath();
        }

        public override void MovePoint(int i, Vector2 pos) => MoveAnchLocalPoint(i, WorldToLocal(pos));

        private void MoveAnchLocalPoint(int i, Vector2 localPos)
        {
            Vector2 deltaMove = localPos - anchPoints[i];
            anchPoints[i] = localPos;

            if (isAutoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {
                if (i * 2 < contPoints.Count || isClosed)
                    contPoints[LoopContIndex(i * 2)] += deltaMove;
                if (i * 2 - 1 >= 0 || isClosed)
                    contPoints[LoopContIndex(i * 2 - 1)] += deltaMove;
            }

            UpdateVertexPath();
        }

        public void MoveContPoint(int i, Vector2 pos) => MoveContLocalPoint(i, WorldToLocal(pos));

        private void MoveContLocalPoint(int i, Vector2 localPos)
        {
            if (isAutoSetControlPoints)
                return;

            contPoints[i] = localPos;

            bool beforeAnchor = i % 2 == 1;
            int coContI = beforeAnchor ? i + 1 : i - 1;
            int anchI = beforeAnchor ? i / 2 + 1 : i / 2;

            if (coContI >= 0 && coContI < contPoints.Count || isClosed)
            {
                Vector2 anchPoint = anchPoints[LoopAnchIndex(anchI)];
                coContI = LoopContIndex(coContI);
                float dst = (anchPoint - contPoints[coContI]).magnitude;
                Vector2 dir = (anchPoint - localPos).normalized;
                contPoints[coContI] = anchPoint + dir * dst;
            }

            UpdateVertexPath();
        }

        public List<Vector2> CalculateEvenelySpacedPoints(float spacing, float resolution = 1)
        {
            spacing = Mathf.Max(0.05f, spacing);
            resolution = Mathf.Max(0, resolution);
            List<Vector2> evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(LocalToWorld(anchPoints.First()));

            Vector2 previousPoint = LocalToWorld(anchPoints.First());
            float dstSinceLastEvenPoint = 0;

            for (int segmentI = 0; segmentI < SegmentCount; segmentI++)
            {
                GetSegment(segmentI, out Vector2 anchP1, out Vector2 contP1, out Vector2 contP2, out Vector2 anchP2);
                float controlNetLength = Vector2.Distance(anchP1, contP1) + Vector2.Distance(contP1, contP2) + Vector2.Distance(contP2, anchP2);
                float estimatedCurveLength = Vector2.Distance(anchP1, anchP2) + controlNetLength / 2;
                float division = Mathf.Ceil(estimatedCurveLength * resolution * 10f);
                float t = 0;
                while (t <= 1)
                {
                    t += 1 / division;
                    Vector2 pointOnCurve = BezierUtil.EvaluateCubic(anchP1, contP1, contP2, anchP2, t);
                    dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                    int check = 0;
                    while (dstSinceLastEvenPoint >= spacing && check < 200)
                    {
                        float overshootDst = dstSinceLastEvenPoint - spacing;
                        Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                        evenlySpacedPoints.Add(newEvenlySpacedPoint);
                        dstSinceLastEvenPoint = overshootDst;
                        previousPoint = newEvenlySpacedPoint;
                        check++;
                    }

                    previousPoint = pointOnCurve;
                }
            }

            return evenlySpacedPoints;
        }

        public override void SetIsClosed(bool isClosed)
        {
            if (isClosed == this.isClosed)
                return;
            this.isClosed = isClosed;
            if (isClosed)
            {
                Vector2 contPoint1 = anchPoints.Last() * 2 - contPoints.Last();
                Vector2 contPoint2 = anchPoints[0] * 2 - contPoints[0];

                contPoints.Add(contPoint1);
                contPoints.Add(contPoint2);

                if (isAutoSetControlPoints)
                {
                    AutoSetContPoints(0);
                    AutoSetContPoints(anchPoints.Count - 1);
                }
            }
            else
            {
                contPoints.RemoveRange(contPoints.Count - 2, 2);
                if (isAutoSetControlPoints)
                {
                    AutoSetStartAndEndControls();
                }
            }

            UpdateVertexPath();
        }

        private void SetIsAutoSetControlPoints(bool value)
        {
            if (isAutoSetControlPoints == value)
                return;

            isAutoSetControlPoints = value;

            if (isAutoSetControlPoints)
            {
                AutoSetAllContPoints();
            }

            UpdateVertexPath();
        }

        private void AutoSetAllAffectedControlPoints(int updatedAnchI)
        {
            for (int i = updatedAnchI - 1; i <= updatedAnchI + 1; i++)
            {
                if (i >= 0 && i < anchPoints.Count || isClosed)
                {
                    AutoSetContPoints(LoopAnchIndex(i));
                }
            }

            AutoSetStartAndEndControls();
        }

        private void AutoSetAllContPoints()
        {
            for (int i = 0; i < anchPoints.Count; i++)
            {
                AutoSetContPoints(i);
            }

            AutoSetStartAndEndControls();
        }

        private void AutoSetContPoints(int anchI)
        {
            Vector2 anchPoint = anchPoints[anchI];
            Vector2 dir = Vector2.zero;
            float[] neighbourDst = new float[2];

            if (anchI - 1 >= 0 || isClosed)
            {
                Vector2 offset = anchPoints[LoopAnchIndex(anchI - 1)] - anchPoint;
                dir += offset.normalized;
                neighbourDst[0] = offset.magnitude;
            }
            if (anchI + 1 >= 0 || isClosed)
            {
                Vector2 offset = anchPoints[LoopAnchIndex(anchI + 1)] - anchPoint;
                dir -= offset.normalized;
                neighbourDst[1] = -offset.magnitude;
            }
            dir.Normalize();

            for (int i = 0; i < 2; i++)
            {
                int contI = anchI * 2 + i - 1;
                if (contI >= 0 && contI < contPoints.Count || isClosed)
                {
                    contPoints[LoopContIndex(contI)] = anchPoint + dir * neighbourDst[i] * .5f;
                }
            }
        }

        private void AutoSetStartAndEndControls()
        {
            if (isClosed)
                return;

            contPoints[0] = (anchPoints[0] + contPoints[1]) * .5f;
            contPoints[contPoints.Count - 1] = (anchPoints.Last() + contPoints[contPoints.Count - 2]) * .5f;
        }

        public override Vector2 GetPoint(int i) => LocalToWorld(anchPoints[i]);

        public Vector2 GetContPoint(int i) => LocalToWorld(contPoints[i]);

        private int LoopContIndex(int i) => (i + contPoints.Count) % contPoints.Count;
        private int LoopAnchIndex(int i) => (i + anchPoints.Count) % anchPoints.Count;

        protected override void UpdateVertexPath()
        {
            List<Vector2> points = CalculateEvenelySpacedPoints(vertexSpacing, vertexResolution);

            if (isClosed)
                points.Add(points.First());

            float[] timeStamps = new float[points.Count];

            int f = 1 / points.Count - 1;

            for (int i = 0; i < points.Count; i++)
            {
                timeStamps[i] = i * f;
            }

            VertexPath.Update(points.ToArray(), timeStamps);
        }
    }
}

