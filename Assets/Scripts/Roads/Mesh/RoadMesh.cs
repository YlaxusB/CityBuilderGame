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

        /*--- Junctions ---*/

        public static Mesh CreateMeshAlongPoints(List<Vector2> pointsList, float roadWidth)
        {
            roadWidth *= 2;
            List<Vector3> points = new List<Vector3>();
            foreach (Vector2 vector in pointsList.ToArray())
            {
                points.Add(new Vector3(vector.x, 0, vector.y));
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
            /*
            for (int i = 1; i < points.Count - 1; i++)
            {
                triangles.Add(i);
                triangles.Add(0);
                triangles.Add(i + 1);
            }
            */
            for (int i = points.Count - 1; i > 1; i--)
            {
                triangles.Add(i);
                triangles.Add(0);
                triangles.Add(i - 1);
            }
            // Create the uvs
            List<Vector2> uvs = new List<Vector2>();
            //uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 0));
            for(int i = 0; i < points.Count / 2; i++)
            {
                float completionPercent = i / (float)(points.Count - 1);
                uvs.Add(new Vector2(1, completionPercent));
                uvs.Add(new Vector2(completionPercent, completionPercent));
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            Vector3[] normals = mesh.normals;
            for(int i = 0; i < normals.Length; i++)
            {
                normals[i] = -1 * normals[i];
            }
            mesh.normals = normals;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        #region Junctions
        // Removes the specified amount of vertices from start or end of a mesh
        public static Mesh RemoveMeshPoints(RoadProperties roadProperties, int pointsToExclude, bool isFromStart)
        {
            Mesh oldMesh = roadProperties.mesh;
            List<Vector3> oldVertices = oldMesh.vertices.ToList();

            // Removes the vertices of start or end
            if (isFromStart)
            {
                oldVertices.RemoveRange(0, pointsToExclude * 2);
            }
            else
            {
                oldVertices.RemoveRange(oldVertices.Count - (pointsToExclude * 2), pointsToExclude * 2);
            }
            List<Vector3> newVertices = oldVertices;

            RoadProperties newRoadProperties = roadProperties;
            //newRoadProperties.points.remove(newRoadProperties.points.Count - 1 - pointsToExclude, pointsToExclude - 1);

            RemoveFrom(newRoadProperties.points, newRoadProperties.points.Count - pointsToExclude);


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
        #endregion


        #region Old Junctions


        // Road Continuation

        // Update previous road (remove the ending points)
        public static Mesh UpdatePreviousMesh(RoadProperties roadProperties, int firstPointsToExclude)
        {

            Mesh oldMesh = roadProperties.mesh;
            List<Vector3> oldVertices = oldMesh.vertices.ToList();
            List<int> oldTriangles = oldMesh.triangles.ToList();
            List<Vector2> oldUvs = oldMesh.uv.ToList();

            oldVertices.RemoveRange(oldVertices.Count - (firstPointsToExclude * 2), firstPointsToExclude * 2);
            List<Vector3> newVertices = oldVertices;

            RoadProperties newRoadProperties = roadProperties;
            //newRoadProperties.points.remove(newRoadProperties.points.Count - 1 - firstPointsToExclude, firstPointsToExclude - 1);
            RemoveFrom(newRoadProperties.points, newRoadProperties.points.Count - firstPointsToExclude);


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
                float completionPercent = 0;
                if (roadProperties.points.Count - 1 != 0)
                {
                    completionPercent = 1 / (roadProperties.points.Count - 1);
                }
                newUvs.Add(new Vector2(0, completionPercent));
                newUvs.Add(new Vector2(1, completionPercent));
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = newVertices.ToArray();
            newMesh.triangles = newTriangles.ToArray();
            //newMesh.uv = newUvs.ToArray();
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
        public static Mesh CreateStraightContinuationMesh(List<Vector2> anchorPoints, List<Vector2> controlPoints, float multiplier, float roadWidth)
        {
            List<Vector2> pointsList = new List<Vector2>();
            //pointsList.Add(new Vector2(startPoint.x - 0.01f, startPoint.z));
            Vector2 start = anchorPoints[0];
            Vector2 end = anchorPoints[1];
            end -= start;
            anchorPoints[0] -= start;
            anchorPoints[1] -= start;
            controlPoints[0] -= start;
            controlPoints[1] -= start;
            start = new Vector2(0, 0);
            pointsList.Add(start);

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

            //mid += (end / 2);
            for (float i = 0; i < 1; i += multiplier)
            {
                /*
                float tAngle = Mathf.Lerp(0, angle * (MathF.PI / 180) / 2, i);
                pointsList.Add(mid + (new Vector2(Mathf.Cos(tAngle),
                    MathF.Sin(tAngle)) * (roadWidth * 2)));
                */
                Vector2 bezierPoint = BezierCurves.Cubic(i, anchorPoints[0], controlPoints[0], controlPoints[1], anchorPoints[1]);
                pointsList.Add(bezierPoint);

                //pointsList.Add(Vector3Extensions.ToVector2(BezierCurves.Cubic(i, startPoint, mid2 - startPoint, mid2 - startPoint, endPoint)));
                //Vector3 a = Vector3.Lerp(Vector3Extensions.ToVector3(start), Vector3Extensions.ToVector3(mid), i);
                //Vector3 b = Vector3.Lerp(Vector3Extensions.ToVector3(mid), Vector3Extensions.ToVector3(end), i);
                //pointsList.Add(Vector3Extensions.ToVector2(Vector3.Slerp(a, b, i)));
                //CustomDebugger.Debugger.Primitive(PrimitiveType.Cube, "Aqui", startPoint + Vector3Extensions.ToVector3(pointsList.Last()), Quaternion.Euler(0, 0, 0));
            }
            pointsList.Add(anchorPoints[1]);
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
        public static Vector3 CenterOfVectors(this List<Vector3> vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Count == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Count;
        }
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
                return new Vector2();
            }
        }
        #endregion


    }
}
