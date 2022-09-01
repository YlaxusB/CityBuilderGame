using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomHelper;

namespace RoadsMeshCreator
{
    public static class RoadMesh
    {
        public static Mesh CreateMesh()
        {
            return new Mesh();
        }

        public static Mesh CreatePrePreviewMesh(GameObject road, Vector2 initialPoint, float roadWidth)
        {
            // Make game objects, mid is mouse position, others are just a distance (road width) from cente
            roadWidth /= 2;
            GameObject midObject = new GameObject("Mid");
            midObject.transform.SetParent(road.transform);
            midObject.transform.localPosition = new Vector3(0, 0, 0);
            Vector3 mid = midObject.transform.position;
            Vector3 lMid = midObject.transform.localPosition;

            GameObject frontObject = new GameObject("Front");
            frontObject.transform.SetParent(road.transform);
            frontObject.transform.localPosition = new Vector3(roadWidth, 0, 0);
            Vector3 lfront = frontObject.transform.localPosition;

            GameObject backObject = new GameObject("Back");
            backObject.transform.SetParent(road.transform);
            backObject.transform.localPosition = new Vector3(-roadWidth, 0, 0);
            Vector3 lBack = backObject.transform.localPosition;

            GameObject RightObject = new GameObject("Right");
            RightObject.transform.SetParent(road.transform);
            RightObject.transform.localPosition = new Vector3(0, 0, -roadWidth);
            Vector3 lRight = RightObject.transform.localPosition;

            GameObject leftObject = new GameObject("Left");
            leftObject.transform.SetParent(road.transform);
            leftObject.transform.localPosition = new Vector3(0, 0, roadWidth);
            Vector3 lLeft = leftObject.transform.localPosition;

            // Gameobject with same x than front and z/y than right
            GameObject rightFront = new GameObject("Curve", typeof(MeshFilter), typeof(MeshRenderer));
            rightFront.transform.SetParent(road.transform);
            rightFront.transform.localPosition = lRight + lfront;


            /* Make a circle using bezier curves */
            List<Vector3> points = new List<Vector3>();
            // Right to front curve
            for (double t = 0; t < 1; t += 0.1)
            {
                points.Add(BezierCurves.Quadratic(((float)t), lRight, rightFront.transform.localPosition, lfront));
                Debug.Log(points[points.Count - 1]);
            }

            // Front to left curve
            for (double t = 0; t < 1; t += 0.1)
            {
                points.Add(BezierCurves.Quadratic(((float)t), lfront, lfront + lLeft, lLeft));
                Debug.Log(points[points.Count - 1]);
            }

            // Left to back curve
            for (double t = 0; t < 1; t += 0.1)
            {
                points.Add(BezierCurves.Quadratic(((float)t), -lRight, -rightFront.transform.localPosition, -lfront));
                Debug.Log(points[points.Count - 1]);
            }
            points.Add(lBack);

            // Back to Right curve
            for (double t = 0; t < 1; t += 0.1)
            {
                points.Add(BezierCurves.Quadratic(((float)t), -lfront, -lfront + -lLeft, -lLeft));
                Debug.Log(points[points.Count - 1]);
            }

            /* End of circle creation */



            List<Vector3> verts = new List<Vector3>();
            List<int> triangles = new List<int>();
            verts.Add(new Vector3(0, 0, 0));

            for (int i = 0; i < points.Count - 1; i++)
            {
                verts.Add(new Vector3(points[i].x, points[i].y, points[i].z));
            }

            //verts.Add(lLeft);
            verts.Add(lRight);

            // Make triangles, going from index, to 0, to index + 1
            for (int i = 1; i < points.Count; i++)
            {
                triangles.Add(i);
                triangles.Add(0);
                triangles.Add(i + 1);
            }
            /*
            triangles.Add(triangles.Count - 1);
            triangles.Add(0);
            triangles.Add(1);*/

            List<Vector2> uvs = new List<Vector2>();
            foreach (Vector3 point in verts)
            {
                uvs.Add(new Vector2(point.x, point.z));
            }
            /*
            Vector3[] verts = new Vector3[points.Length * 2];
            Vector2[] uvs = new Vector2[verts.Length];
            int[] tris = new int[2 * (points.Length - 1) * 3];

            Vector2 forward = Vector2.zero;
            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);
            BezierCurves.Quadratic
            */
            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            return mesh;
            /*
            // vertices = 2 * number of points
            // triangles = (2 * (number of points - 1) * 3) 
            Vector3[] verts = new Vector3[points.Length * 2];
            Vector2[] uvs = new Vector2[verts.Length];
            int[] tris = new int[2 * (points.Length - 1) * 3];
            int vertIndex = 0;
            int triIndex = 0;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 forward = Vector2.zero;
                if (i < points.Length - 1)
                {
                    forward += points[i + 1] - points[i];
                }
                if (i > 0)
                {
                    forward += points[i] - points[i - 1];
                }
                forward.Normalize();
                Vector2 left = new Vector2(-forward.y, forward.x);

                verts[vertIndex] = points[i] + left * roadWidth * .5f;
                verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;

                float completionPercent = i / (float)(points.Length - 1);
                uvs[vertIndex] = new Vector2(0, completionPercent);
                uvs[vertIndex + 1] = new Vector2(1, completionPercent);


                if (i < points.Length - 1)
                {
                    tris[triIndex] = vertIndex;
                    tris[triIndex + 1] = vertIndex + 2;
                    tris[triIndex + 2] = vertIndex + 1;

                    tris[triIndex + 3] = vertIndex + 1;
                    tris[triIndex + 4] = vertIndex + 2;
                    tris[triIndex + 5] = vertIndex + 3;
                }

                vertIndex += 2;
                triIndex += 6;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uvs;
            return mesh;
            */
        }
    }
}
