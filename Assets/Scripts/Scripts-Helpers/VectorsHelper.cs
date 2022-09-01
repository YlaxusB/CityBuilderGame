using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CustomHelper
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Convert Vector 3 to 2
        /// <para>2.X = 3.X</para>
        /// <para>2.Y = 3.Z</para>
        /// </summary>
        public static Vector2 ToVector2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        /// Convert Vector 2 to 3
        /// <para>2.X = 3.X</para>
        /// <para>2.Y = 3.Z</para>
        /// </summary>
        public static Vector3 ToVector3(Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }
    }

}


