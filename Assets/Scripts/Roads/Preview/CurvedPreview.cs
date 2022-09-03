using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomHelper;
using RoadsMeshCreator;

namespace Preview
{
    public class CurvedPreview
    {
        // Curved Preview (The preview when you choose a initial point and a mid point)

        // Bezier
        public static GameObject CreateBezier(RoadProperties roadProperties, List<Vector3> points)
        {
            // Create the initial plane
            GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
            previewRoad.transform.position = new Vector3(points[0].x, roadProperties.height, points[0].z);
            previewRoad.transform.rotation = Quaternion.Euler(90, 0, 0);
            previewRoad.name = "Bezier Preview Road";

            // Material and textures
            MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
            roadMeshRenderer.material = roadProperties.previewMaterial;
            roadMeshRenderer.material.mainTexture = roadProperties.texture;

            // Mesh
            MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();
            roadMeshFilter.mesh = RoadMesh.CreateBezierMesh(points[0], points[1],
                Raycasts.raycastLayer(roadProperties.camera, "Terrain"), 0.1f, roadProperties.width);

            // Add this plane to preview roads "folder" and start the update preview that runs each game update
            previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

            return previewRoad;
        }

        public static IEnumerator UpdateBezier(GameObject road, RoadProperties roadProperties, List<Vector3> points)
        {
            bool canRun = true;
            while (canRun)
            {
                MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
                Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);
                // Update Mesh
                roadMeshFilter.mesh = RoadMesh.CreateBezierMesh(points[0], points[1],
                        endPosition, 0.01f, roadProperties.width);

                //float angle = -Mathf.Atan2(points[1].z - points[0].z, points[1].x - points[0].x) * (180 / Mathf.PI);
                //road.transform.rotation = Quaternion.Euler(0, angle, 0);
                //road.transform.ro
                yield return null;
            }
        }


        ///

        public static GameObject CreateBezierContinuation(RoadProperties roadProperties, List<Vector3> points)
        {
            // Create the initial plane
            GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
            previewRoad.transform.position = new Vector3(points[0].x, roadProperties.height, points[0].z);
            previewRoad.transform.rotation = Quaternion.Euler(90, 0, 0);
            previewRoad.name = "Bezier Preview Road";

            // Material and textures
            MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
            roadMeshRenderer.material = roadProperties.previewMaterial;
            roadMeshRenderer.material.mainTexture = roadProperties.texture;

            // Mesh
            MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();
            roadMeshFilter.mesh = RoadMesh.CreateBezierContinuation(points[0], points[1],
                Raycasts.raycastLayer(roadProperties.camera, "Terrain"), 0.1f, roadProperties.width);

            // Add this plane to preview roads "folder" and start the update preview that runs each game update
            previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

            return previewRoad;
        }

        public static IEnumerator UpdateBezierContinuation(GameObject road, RoadProperties roadProperties, List<Vector3> points)
        {
            bool canRun = true;
            while (canRun)
            {
                MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
                Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);
                // Update Mesh
                roadMeshFilter.mesh = RoadMesh.CreateBezierContinuation(points[0], points[1],
                        endPosition, 0.01f, roadProperties.width);

                //float angle = -Mathf.Atan2(points[1].z - points[0].z, points[1].x - points[0].x) * (180 / Mathf.PI);
                //road.transform.rotation = Quaternion.Euler(0, angle, 0);
                //road.transform.ro
                yield return null;
            }
        }

        // Biarc
    }
}