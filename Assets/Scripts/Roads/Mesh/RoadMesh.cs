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
            List<Vector3> points = new List<Vector3>();

            // Create the points where
            float steps = roadWidth * 4;
            for (int currentStep = 0; currentStep <= steps; currentStep++)
            {
                float circumferenceProgress = currentStep / steps;

                float currentRadian = circumferenceProgress * 2 * Mathf.PI;

                float zScaled = Mathf.Cos(currentRadian);
                float xScaled = Mathf.Sin(currentRadian);
                points.Add(new Vector3(xScaled * roadWidth, 0, zScaled * roadWidth));
            }

            List<Vector3> verts = new List<Vector3>();
            List<int> triangles = new List<int>();


            // The first vertice is the middle and others are just the circle points
            verts.Add(new Vector3(0, 0, 0));
            for (int i = 0; i < points.Count - 1; i++)
            {
                verts.Add(new Vector3(points[i].x, points[i].y, points[i].z));
            }

            // Make triangles, going from index, to 0, to index + 1 /
            // point - middle point - next point 
            for (int i = 1; i < points.Count - 1; i++)
            {
                triangles.Add(i);
                triangles.Add(0);
                triangles.Add(i + 1);
            }
            triangles.Add(points.Count - 1);
            triangles.Add(0);
            triangles.Add(1);

            List<Vector2> uvs = new List<Vector2>();
            foreach (Vector3 vert in verts)
            {
                uvs.Add(new Vector2(vert.x, vert.z));
            }
            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            return mesh;
        }
    
        public static Mesh CreateStraightMesh(Vector3 startPoint, Vector3 endPoint, float multiplier, float roadWidth)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 start = new Vector3(0, 0, 0);
            Vector3 end = new Vector3(Vector3.Distance(endPoint, startPoint), 0, 0);
            // Create the points, going from start to end
            for (float t = 0; t < 1; t+= multiplier)
            {
                points.Add(BezierCurves.Linear(t, start, end));
                if(t < 1 && t > 1 - multiplier)
                {
                    points.Add(end);
                }
            }


            // Create the verts, they are on left and right of the points
            List<Vector3> verts = new List<Vector3>();
            for(int i = 0; i < points.Count; i++)
            {
                verts.Add(new Vector3(points[i].x, points[i].y, roadWidth));
                verts.Add(new Vector3(points[i].x, points[i].y, -roadWidth));
            }

            // Create the uvs
            List<Vector2> uvs = new List<Vector2>();
            for(int i = 0; i < points.Count; i++)
            {
                float completionPercent = 1 / (points.Count - 1);
                uvs.Add(new Vector2(0, completionPercent));
                uvs.Add(new Vector2(1, completionPercent));
            }

            // Create the triangles
            List<int> triangles = new List<int>();
            for(int i = 0; i < (points.Count * 1.5f) + 2; i+= 2)
            {
                triangles.Add(i);
                triangles.Add(i + 2);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i + 3);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            return mesh;
        }

        public static Mesh CreateCurvedMesh(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float multiplier, float roadWidth)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 start = new Vector3(0, 0, 0);
            Vector3 mid = midPoint - startPoint;
            Vector3 end = endPoint - startPoint;
            // Create the points, going from start to end
            for (float t = 0; t < 1; t += multiplier)
            {
                points.Add(BezierCurves.Quadratic(t, start, mid, end));
                if (t < 1 && t > 1 - multiplier)
                {
                    points.Add(end);
                }
            }


            // Create the verts, they are on left and right of the points
            List<Vector3> verts = new List<Vector3>();
            for (int i = 0; i < points.Count; i++)
            {
                verts.Add(new Vector3(points[i].x, points[i].y, roadWidth));
                verts.Add(new Vector3(points[i].x, points[i].y, -roadWidth));
            }

            // Create the triangles
            List<int> triangles = new List<int>();
            for (int i = 0; i < (points.Count * 1.5f) + 2; i += 2)
            {
                triangles.Add(i);
                triangles.Add(i + 2);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i + 3);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            return mesh;
        }
    }
}
