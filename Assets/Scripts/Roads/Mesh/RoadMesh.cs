using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomHelper;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using CustomDebugger;
namespace RoadsMeshCreator
{
    public static class RoadMesh
    {
        public static Mesh CreateMesh()
        {
            return new Mesh();
        }

        //  The circle mesh
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

        public static RoadProperties CreateStraightMesh(Vector3 startPoint, Vector3 endPoint, float multiplier, float roadWidth, RoadProperties roadProperties)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 start = new Vector3(0, 0, 0);
            Vector3 end = new Vector3(Vector3.Distance(endPoint, startPoint), 0, 0);
            // Create the points, going from start to end

            // Multiplier, each 5 cubes place 1 point, that will have two vertices
            float distance = Vector3.Distance(startPoint, endPoint);
            float desiredCubes = 5;
            multiplier = Mathf.Clamp((1 / distance) * desiredCubes, 0.000001f, 0.2f);

            for (float t = 0; t < 1; t += multiplier)
            {
                points.Add(BezierCurves.Linear(t, start, end));
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

            // Create the uvs
            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < points.Count; i++)
            {
                float completionPercent = 1 / (points.Count - 1);
                uvs.Add(new Vector2(0, completionPercent));
                uvs.Add(new Vector2(1, completionPercent));
            }

            // Create the triangles
            List<int> triangles = new List<int>();
            for (int i = 0; i < 2 * (points.Count - 1); i += 2)
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

            roadProperties.mesh = mesh;
            roadProperties.points = points;
            return roadProperties;
        }

        public static Mesh CreateBezierMesh(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float multiplier, float roadWidth)
        {
            List<Vector2> pointsList = new List<Vector2>();
            Vector2 start = Vector3Extensions.ToVector2(startPoint);
            Vector2 mid = Vector3Extensions.ToVector2(midPoint);
            Vector2 end = Vector3Extensions.ToVector2(endPoint);
            mid -= start;
            end -= start;
            start = new Vector2(0, 0);

            for (float i = 0; i < 1; i += multiplier)
            {
                pointsList.Add(BezierCurves.Quadratic(i, start, mid, end));
            }

            // Iterate to get the distance of startPoint and endPoint traveled by the road
            float distance = 0;
            for (int i = pointsList.Count - 1; i > 1; i--)
            {
                distance += Vector3.Distance(pointsList[i], pointsList[i - 1]);
            }

            Vector2[] points = pointsList.ToArray();
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

                verts[vertIndex] = points[i] + left * roadWidth;
                verts[vertIndex + 1] = points[i] - left * roadWidth;

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
        }

        public static Mesh CreateBezierContinuation(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float multiplier, float roadWidth)
        {
            List<Vector2> pointsList = new List<Vector2>();
            Vector2 start = Vector3Extensions.ToVector2(startPoint);
            Vector2 mid = Vector3Extensions.ToVector2(midPoint);
            Vector2 end = Vector3Extensions.ToVector2(endPoint);
            mid -= start;
            end -= start;
            start = new Vector2(0, 0);

            for (float i = 0; i < 1; i += multiplier)
            {
                pointsList.Add(BezierCurves.Quadratic(i, start, mid, end));
            }

            Vector2[] points = pointsList.ToArray();
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
        }

        #region Junctions

        // Road Continuation

        // Update previous road (remove the ending points)
        public static Mesh UpdatePreviousMesh(RoadProperties roadProperties, int firstPointsToExclude)
        {
            Debug.Log(roadProperties.points.Count);
            Debug.Log(firstPointsToExclude);

            Mesh oldMesh = roadProperties.mesh;
            List<Vector3> oldVertices = oldMesh.vertices.ToList();
            List<int> oldTriangles = oldMesh.triangles.ToList();
            List<Vector2> oldUvs = oldMesh.uv.ToList();

            oldVertices.RemoveRange(oldVertices.Count - (firstPointsToExclude * 2), firstPointsToExclude * 2);
            List<Vector3> newVertices = oldVertices;

            RoadProperties newRoadProperties = roadProperties;
            //newRoadProperties.points.remove(newRoadProperties.points.Count - 1 - firstPointsToExclude, firstPointsToExclude - 1);
            RemoveFrom(newRoadProperties.points, newRoadProperties.points.Count - firstPointsToExclude);
            Debug.Log(newRoadProperties.points.Count);


            List<int> newTriangles = new List<int>();//oldMesh.triangles.ToList();
            for (int i = 0; i < 2 * (roadProperties.points.Count - 1); i += 2)
            {
                newTriangles.Add(i);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 1);

                newTriangles.Add(i + 1);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 3);
            }

            List<Vector2> newUvs = new List<Vector2>();//oldMesh.uv.ToList();
            for (int i = 0; i < roadProperties.points.Count; i++)
            {
                float completionPercent = 1 / (roadProperties.points.Count - 1);
                newUvs.Add(new Vector2(0, completionPercent));
                newUvs.Add(new Vector2(1, completionPercent));
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = newVertices.ToArray();
            newMesh.triangles = newTriangles.ToArray();
            newMesh.uv = newUvs.ToArray();
            return newMesh;
        }

        // Update the preview continuation (remove the starting points)
        public static Mesh UpdatePreviewMesh(RoadProperties roadProperties, int previewPointsToExclude)
        {
            if (roadProperties.points.Count <= previewPointsToExclude)
            {
                return roadProperties.mesh;
            }

            Mesh oldMesh = roadProperties.mesh;
            List<Vector3> oldVertices = oldMesh.vertices.ToList();
            List<int> oldTriangles = oldMesh.triangles.ToList();
            List<Vector2> oldUvs = oldMesh.uv.ToList();

            // Remove the initials vertices
            oldVertices.RemoveRange(0, previewPointsToExclude * 2);
            List<Vector3> newVertices = oldVertices;

            // Remove the initials points
            RoadProperties newRoadProperties = roadProperties;
            newRoadProperties.points.RemoveRange(0, previewPointsToExclude);


            List<int> newTriangles = new List<int>();//oldMesh.triangles.ToList();
            for (int i = 0; i < 2 * (roadProperties.points.Count - 1); i += 2)
            {
                newTriangles.Add(i);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 1);

                newTriangles.Add(i + 1);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 3);
            }

            List<Vector2> newUvs = new List<Vector2>();//oldMesh.uv.ToList();
            for (int i = 0; i < roadProperties.points.Count; i++)
            {
                float completionPercent = 1 / (roadProperties.points.Count - 1);
                newUvs.Add(new Vector2(0, completionPercent));
                newUvs.Add(new Vector2(1, completionPercent));
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = newVertices.ToArray();
            newMesh.triangles = newTriangles.ToArray();
            newMesh.uv = newUvs.ToArray();
            return newMesh;
        }

        // Create straight junction continuation
        public static Mesh CreateContinuationMesh(RoadProperties roadProperties, int previewPointsToExclude)
        {

            Mesh oldMesh = roadProperties.mesh;
            List<Vector3> oldVertices = oldMesh.vertices.ToList();
            List<int> oldTriangles = oldMesh.triangles.ToList();
            List<Vector2> oldUvs = oldMesh.uv.ToList();

            // Remove the initials vertices
            oldVertices.RemoveRange(0, previewPointsToExclude * 2);
            List<Vector3> newVertices = oldVertices;

            // Remove the initials points
            RoadProperties newRoadProperties = roadProperties;
            newRoadProperties.points.RemoveRange(0, previewPointsToExclude);
            Debug.Log(newRoadProperties.points.Count);


            List<int> newTriangles = new List<int>();//oldMesh.triangles.ToList();
            for (int i = 0; i < 2 * (roadProperties.points.Count - 1); i += 2)
            {
                newTriangles.Add(i);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 1);

                newTriangles.Add(i + 1);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 3);
            }

            List<Vector2> newUvs = new List<Vector2>();//oldMesh.uv.ToList();
            for (int i = 0; i < roadProperties.points.Count; i++)
            {
                float completionPercent = 1 / (roadProperties.points.Count - 1);
                newUvs.Add(new Vector2(0, completionPercent));
                newUvs.Add(new Vector2(1, completionPercent));
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = newVertices.ToArray();
            newMesh.triangles = newTriangles.ToArray();
            newMesh.uv = newUvs.ToArray();
            return newMesh;
        }




        public static void RemoveFrom<T>(this List<T> lst, int from)
        {
            lst.RemoveRange(from, lst.Count - from);
        }

        // Combine Meshes
        public static Mesh CombineMeshes(List<Mesh> meshes)
        {
            Mesh newMesh = new Mesh();
            CombineInstance[] combine = new CombineInstance[meshes.Count - 1];
            for (int i = 0; i < meshes.Count - 1; i++)
            {
                combine[i].mesh = meshes[i];
            }
            newMesh.CombineMeshes(combine);
            return newMesh;
        }

        // Create straight continued mesh
        public static Mesh CreateStraightContinuationMesh(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float multiplier, float roadWidth, float startAngle)
        {
            List<Vector2> pointsList = new List<Vector2>();
            //pointsList.Add(new Vector2(startPoint.x - 0.01f, startPoint.z));
            Vector2 start = Vector3Extensions.ToVector2(startPoint);
            Vector2 mid = Vector3Extensions.ToVector2(midPoint);
            Vector2 end = Vector3Extensions.ToVector2(endPoint);
            mid -= start;
            end -= start;
            start = new Vector2(0, 0);

            /*
            pointsList.Add(start);
            for (float i = 0; i < 1; i += multiplier)
            {
                Vector2 p0 = Vector2.Lerp(start, mid, i);
                Vector2 p1 = Vector2.Lerp(mid, end, i);
                
                pointsList.Add(Vector2.Lerp(p0, p1, i));
            }
            pointsList.Add(end);
            */

            multiplier = 0.01f;
            //float startAngle = 0;
            float angle = -Mathf.Atan2(end.y - start.y, end.x - start.x) * (180 / Mathf.PI);
            Debug.Log(angle);
            float ang = MathF.Abs(startAngle) - MathF.Abs(angle);
            float desiredAngle = (Mathf.Abs(ang) * (Mathf.PI/180));
            Debug.Log(desiredAngle * (180/MathF.PI));

            //mid += (end / 2);

            int iter = 0;

            for (float i = 0; i < desiredAngle; i += multiplier)
            {
                iter++;
                if (iter > 10000)
                {
                    Debug.Log("porra");
                    break;
                }
                Debug.Log("a");
                float tAngle = Mathf.Lerp(0, desiredAngle, i);
                Debug.Log("b");
                pointsList.Add(mid + (new Vector2(Mathf.Cos(tAngle),
                    MathF.Sin(tAngle)) * (roadWidth)));
                Debug.Log("c");
                //CustomDebugger.Debugger.Primitive(PrimitiveType.Cube, "Aqui", startPoint + Vector3Extensions.ToVector3(pointsList.Last()), Quaternion.Euler(0, 0, 0));
                Debug.Log("d");
            }
            Debug.Log("e");
            CustomDebugger.Debugger.Primitive(PrimitiveType.Cube, "Mid", startPoint + new Vector3(mid.x, 0.0f, mid.y), Quaternion.Euler(0, 0, 0));
            // Iterate to get the distance of startPoint and endPoint traveled by the road
            float distance = 0;
            for (int i = pointsList.Count - 1; i > 1; i--)
            {
                distance += Vector3.Distance(pointsList[i], pointsList[i - 1]);
            }

            Vector2[] points = pointsList.ToArray();
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

                verts[vertIndex] = points[i] + left * roadWidth;
                verts[vertIndex + 1] = points[i] - left * roadWidth;

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
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            return mesh;
        }
        // Internet
        public static Vector2 DrawArcBetweenTwoPoints(Vector2 a, Vector2 b, float radius, bool flip = false)
        {
            if (flip)
            {
                Vector2 temp = b;
                b = a;
                a = temp;
            }

            // get distance components
            double x = b.x - a.x, y = b.y - a.y;
            // get orientation angle
            var θ = Math.Atan2(y, x);
            // length between A and B
            var l = Math.Sqrt(x * x + y * y);
            if (2 * radius >= l)
            {
                // find the sweep angle (actually half the sweep angle)
                var φ = Math.Asin(l / (2 * radius));
                // triangle height from the chord to the center
                var h = radius * Math.Cos(φ);
                // get center point. 
                // Use sin(θ)=y/l and cos(θ)=x/l
                Vector2 C = new Vector2(
                    (float)(a.x + x / 2 - h * (y / l)),
                    (float)(a.y + y / 2 + h * (x / l)));


                // Conversion factor between radians and degrees
                const double to_deg = 180 / Math.PI;

                return new Vector2(C.x - radius, C.y - radius);
                // Draw arc based on square around center and start/sweep angles
                //g.DrawArc(pen, C.x - radius, C.y - radius, 2 * radius, 2 * radius,
                //    (float)((θ - φ) * to_deg) - 90, (float)(2 * φ * to_deg));
            }
            else
            {
                Debug.Log("VÁ SE FODER");
                return new Vector2();
            }
        }
        #endregion

        // sla 

        public static void eita()
        {
            Handles.DrawWireArc(new Vector3(0, 0.2f, 0), new Vector3(5, 0.2f, 5), new Vector3(10, 0.2f, 10), 47, 79);
        }
    }
}
