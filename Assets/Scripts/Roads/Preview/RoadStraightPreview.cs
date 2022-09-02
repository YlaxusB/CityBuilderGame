using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomHelper;
using RoadsMeshCreator;


namespace Preview
{
    public static class StraightPreview
    {
        // Straight Preview (The preview when you choose a initial point)
        public static GameObject Create(RoadProperties roadProperties, List<Vector3> points)
        {
            // Create the initial plane
            GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
            previewRoad.transform.position = Raycasts.raycastLayer(roadProperties.camera, "Terrain") + new Vector3(0, roadProperties.height, 0);
            previewRoad.transform.rotation = Quaternion.Euler(180, 0, 0);
            previewRoad.name = "Straight Preview Road";

            // Material and textures
            MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
            roadMeshRenderer.material = roadProperties.previewMaterial;
            roadMeshRenderer.material.mainTexture = roadProperties.texture;

            // Mesh
            MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();
            roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
                Raycasts.raycastLayer(roadProperties.camera, "Terrain"), 0.1f, roadProperties.width);

            // Add this plane to preview roads "folder" and start the update preview that runs each game update
            previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

            Debug.Log("Created Straight Preview");
            return previewRoad;
        }

        public static IEnumerator Update(GameObject road, RoadProperties roadProperties, List<Vector3> points)
        {
            bool canRun = true;
            while (canRun)
            {
                MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
                Debug.Log("updating");
                Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);
                // Update Mesh
                roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
                    endPosition, 0.1f, roadProperties.width);
                float angle = -Mathf.Atan2(endPosition.z - points[0].z, endPosition.x - points[0].x) * (180 / Mathf.PI);
                road.transform.rotation = Quaternion.Euler(0, angle, 0);
                //road.transform.ro
                yield return null;
            }
        }
    }
}