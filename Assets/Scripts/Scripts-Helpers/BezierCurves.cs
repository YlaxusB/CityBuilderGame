//Assets/Editor/KeywordReplace.cs
using UnityEngine;

namespace CustomHelper
{
    public static class BezierCurves
    {
        // Cubic Bezier Curves
        /// <summary>
        /// Calculates the Cubic bezier of 4 points in given t
        /// <para>These points can be Vector3 or Vector2</para>
        /// <para>t needs to be less than 1 and more than 0</para>
        /// </summary>
        /// <returns>Vector3</returns>
        public static Vector3 Cubic(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Mathf.Pow(1-t, 3) * p0 + 3 * Mathf.Pow(1-t, 2) * t * p1 + 3*(1-t) * Mathf.Pow(t,2) * p2 + Mathf.Pow(t,3) * p3;
        }

        public static Vector2 Cubic(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return Mathf.Pow(1 - t, 3) * p0 + 3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;
        }

        // Quadratic Bezier Curves
        /// <summary>
        /// Calculates the Quadratic bezier of 3 points (start, mid, end) in given t
        /// <para>These points can be Vector3 or Vector2</para>
        /// <para>t needs to be less than 1 and more than 0</para>
        /// </summary>
        /// <returns>Vector3</returns>
        public static Vector3 Quadratic(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
        }

        /// <summary>
        /// Calculates the Quadratic bezier of 3 points (start, mid, end) in given t
        /// These points can be Vector3 or Vector2
        /// </summary>
        /// <param name="p0">Value from 0 to 1</param>
        /// <returns>Vector3</returns>
        public static Vector2 Quadratic(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
        }

        // Linear Bezier Curves
        /// <summary>
        /// Calculates the Linear bezier of 2 points in given t
        /// <para>These points can be Vector3 or Vector2</para>
        /// <para>t needs to be less than 1 and more than 0</para>
        /// </summary>
        /// <returns>Vector3</returns>
        public static Vector3 Linear(float t, Vector3 p0, Vector3 p1)
        {
            return p0 + t * (p1 - p0);
        }

        public static Vector2 Linear(float t, Vector2 p0, Vector2 p1)
        {
            return p0 + t * (p1 - p0);
        }
    }
}
