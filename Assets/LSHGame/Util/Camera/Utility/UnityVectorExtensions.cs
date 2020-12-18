using UnityEngine;

namespace MyCinemachine
{
    public static class UnityVectorExtensions
    {
        /// <summary>A useful Epsilon</summary>
        public const float Epsilon = 0.0001f;

        /// <summary>
        /// Checks if the Vector2 contains NaN for x or y.
        /// </summary>
        /// <param name="v">Vector2 to check for NaN</param>
        /// <returns>True, if any components of the vector are NaN</returns>
        public static bool IsNaN(this Vector2 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y);
        }

        /// <summary>
        /// Calculates the intersection point defined by line_1 [p1, p2], and line_2 [p3, p4).
        /// Meaning of brackets:
        /// <list type="bullet">
        /// <item><description> If line_1 intersects line_2 at exactly p4, then segments do not intersect, but lines do.</description></item>
        /// <item><description> If line_1 intersects line_2 at exactly p3, then segments intersect.</description></item>
        /// </list>
        /// <para>Returns intersection type (0 = no intersection, 1 = lines intersect, 2 = segments intersect).</para>
        /// </summary>
        /// <param name="p1">line_1 is defined by (p1, p2)</param>
        /// <param name="p2">line_1 is defined by (p1, p2)</param>
        /// <param name="p3">line_2 is defined by (p3, p4)</param>
        /// <param name="p4">line_2 is defined by (p3, p4)</param>
        /// <param name="intersection">If lines intersect, then this will hold the intersection point. Otherwise, it will be Vector2.positiveInfinity.</param>
        /// <returns>0 = no intersection, 1 = lines intersect, 2 = segments intersect.</returns>
        public static int FindIntersection(in Vector2 p1, in Vector2 p2, in Vector2 p3, in Vector2 p4,
            out Vector2 intersection)
        {
            // Get the segments' parameters.
            float dx12 = p2.x - p1.x;
            float dy12 = p2.y - p1.y;
            float dx34 = p4.x - p3.x;
            float dy34 = p4.y - p3.y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.x - p3.x) * dy34 + (p3.y - p1.y) * dx34)
                / denominator;
            if (float.IsInfinity(t1) || float.IsNaN(t1))
            {
                // The lines are parallel (or close enough to it).
                intersection = Vector2.positiveInfinity;
                if ((p1 - p3).sqrMagnitude < 0.01f || (p1 - p4).sqrMagnitude < 0.01f ||
                    (p2 - p3).sqrMagnitude < 0.01f || (p2 - p4).sqrMagnitude < 0.01f)
                {
                    return 2; // they are the same line, or very close parallels
                }
                return 0; // no intersection
            }

            // Find the point of intersection.
            intersection = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);

            float t2 = ((p3.x - p1.x) * dy12 + (p1.y - p3.y) * dx12) / -denominator;
            return (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 < 1) ? 2 : 1; // 2 = segments intersect, 1 = lines intersect
        }
    }
}
