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
            GameObject RightObject = new GameObject();
            RightObject.transform.SetParent(road.transform);
            RightObject.transform.localPosition = new Vector3(0, 0, roadWidth);

            GameObject leftObject = new GameObject();
            leftObject.transform.SetParent(road.transform);
            leftObject.transform.localPosition = new Vector3(0, 0, -roadWidth);

            GameObject frontObject = new GameObject();
            frontObject.transform.SetParent(road.transform);
            frontObject.transform.localPosition = new Vector3(roadWidth, 0, 0);

            GameObject backObject = new GameObject();
            backObject.transform.SetParent(road.transform);
            backObject.transform.localPosition = new Vector3(-roadWidth, 0, 0);

            List<Vector2> points = new List<Vector2>();
            for(float t = 0; t < 1; t += 0.1f)
            {
                points.Add(BezierCurves.Quadratic(t, RightObject.transform.localPosition,RightObject.transform.localPosition + ((frontObject.transform.localPosition - RightObject.transform.localPosition)/2), frontObject.transform.localPosition));
            }
            List<Vector3> verts = new List<Vector3>();
            List<int> triangles = new List<int>();

            for(int i = 0; i < points.Count - 1; i++)
            {
                verts.Add(Vector3Extensions.ToVector3(points[i]));
            }

            for(int i = 0; i < points.Count - 1; i++)
            {
                if(i != triangles.Count)
                {
                    triangles.Add(i);
                    triangles.Add(0);
                    triangles.Add(i + 1);
                }
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
            mesh.uv = points.ToArray();

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
