using System;
using UnityEngine;

namespace PathC
{
    public class VertexPath
    {
        public Vector2[] Points { get; private set; }
        public float[] Ts { get; private set; }

        public float Length { get; private set; }
        public float Durration { get; private set; }

        public Action OnPathUpdated;

        bool isInit = false;

        public VertexPath()
        {

        }

        public void Update(Vector2[] points, float[] timeStamps,float pathLength = -1)
        {
            if (points.Length != timeStamps.Length)
                throw new System.Exception("Points and times have to have the same length");

            this.Points = points;

            if (pathLength == -1)
            {
                Length = 0;
                for (int i = 0; i < points.Length - 1; i++)
                {
                    Length += Vector2.Distance(points[i], points[i + 1]);
                }
            }
            else
                Length = pathLength;

            Ts = new float[timeStamps.Length];
            Durration = timeStamps[timeStamps.Length - 1];

            for (int i = 0; i < timeStamps.Length; i++)
            {
                Ts[i] = Mathf.Min(1,timeStamps[i] / Durration);
            }

            isInit = true;
            OnPathUpdated?.Invoke();
        }

        public void GetAtDistance(float distance, out Vector2 pos, out Vector2 direction, EndOfPathInstruction instruction = EndOfPathInstruction.Loop)
        {
            GetAtPercent(distance / Length, out pos, out direction, instruction);
        }

        public void GetAtTime(float time, out Vector2 pos, out Vector2 direction, EndOfPathInstruction instruction = EndOfPathInstruction.Loop)
        {
            GetAtPercent(time / Durration, out pos, out direction, instruction);
        }

        public void GetAtPercent(float t, out Vector2 pos, out Vector2 direction,EndOfPathInstruction instruction = EndOfPathInstruction.Loop)
        {
            if (!isInit)
            {
                pos = Vector2.zero;
                direction = Vector2.zero;
                return;
            }

            t = ClampT(t, instruction);

            int prevIndex = 0;
            int nextIndex = Points.Length - 1;
            int i = Mathf.RoundToInt(t * (Points.Length - 1)); // starting guess

            // Starts by looking at middle vertex and determines if t lies to the left or to the right of that vertex.
            // Continues dividing in half until closest surrounding vertices have been found.
            while (true)
            {
                // t lies to left
                if (t <= Ts[i])
                {
                    nextIndex = i;
                }
                // t lies to right
                else
                {
                    prevIndex = i;
                }
                i = (nextIndex + prevIndex) / 2;

                if (nextIndex - prevIndex <= 1)
                {
                    break;
                }
            }   

            float abPercent = Mathf.InverseLerp(Ts[prevIndex], Ts[nextIndex], t);

            if(nextIndex == prevIndex)
            {
                nextIndex = (nextIndex + 1) % Points.Length;
            }
            direction = Points[nextIndex] - Points[prevIndex];
            if(direction == Vector2.zero) 
            {
                direction = Points[(nextIndex + 1) % Points.Length] - Points[prevIndex];
            }
            pos = Vector2.Lerp(Points[prevIndex], Points[nextIndex], abPercent);
        }

        private float ClampT(float t,EndOfPathInstruction instruction)
        {
            switch (instruction)
            {
                case EndOfPathInstruction.Loop:
                    // If t is negative, make it the equivalent value between 0 and 1
                    if (t < 0)
                    {
                        t += Mathf.CeilToInt(Mathf.Abs(t));
                    }
                    t %= 1;
                    break;
                case EndOfPathInstruction.PingPong:
                    t = Mathf.PingPong(t, 1);
                    break;
                case EndOfPathInstruction.Stop:
                    t = Mathf.Clamp01(t);
                    break;
                case EndOfPathInstruction.SmoothStep:
                    t = Mathf.SmoothStep(0,1,Mathf.Clamp01(t));
                    break;
            }
            return t;
        }
    }

    public enum EndOfPathInstruction
    {
        Loop,PingPong,Stop,SmoothStep
    }
}
