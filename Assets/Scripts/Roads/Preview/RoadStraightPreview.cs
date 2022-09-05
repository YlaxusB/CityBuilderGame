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
            previewRoad.transform.position = new Vector3(Mathf.Round(points[0].x), roadProperties.height, Mathf.Round(points[0].z)) ;//Raycasts.raycastLayer(roadProperties.camera, "Terrain") + new Vector3(0, roadProperties.height, 0);
            previewRoad.transform.rotation = Quaternion.Euler(180, 0, 0);
            previewRoad.name = "Straight Preview Road";

            // Material and textures
            MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
            roadMeshRenderer.material = roadProperties.previewMaterial;
            roadMeshRenderer.material.mainTexture = roadProperties.texture;

            // Mesh
            MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();
            roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
                Raycasts.raycastLayer(roadProperties.camera, "Terrain"), 0.1f, roadProperties.width, roadProperties).mesh;

            // Collision System
            previewRoad.AddComponent<PreviewColliderScript>();
            Rigidbody rigidbody = previewRoad.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            // Add this plane to preview roads "folder" and start the update preview that runs each game update
            previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

            // Add properties to preview
            RoadProperties previewProperties = previewRoad.AddComponent<RoadProperties>();
            previewProperties.ChangeProperties(roadProperties);

            return previewRoad;
        }

        public static IEnumerator Update(GameObject road, RoadProperties roadProperties, List<Vector3> points, bool continuation)
        {
            bool canRun = true;
            while (canRun && points.Count > 0)
            {
                MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
                Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);
                // Update Mesh
               Mesh newMesh = RoadMesh.CreateStraightMesh(points[0],
                    endPosition, 0.1f, roadProperties.width, roadProperties).mesh;
                roadProperties.mesh = newMesh;
                if (continuation)
                {
                    roadMeshFilter.mesh = RoadMesh.UpdatePreviewMesh(roadProperties, ((int)(Mathf.Ceil(roadProperties.width / 5) + 1)));
                }
                else
                {
                    roadMeshFilter.mesh = newMesh;
                }
                float angle = -Mathf.Atan2(Mathf.Round(endPosition.z) - Mathf.Round(points[0].z),
                    Mathf.Round(endPosition.x) - Mathf.Round(points[0].x)) * (180 / Mathf.PI);
                road.transform.rotation = Quaternion.Euler(0, Mathf.Round(angle), 0);
                //road.transform.ro

                // Check colliding and create intersections
                MeshCollider roadMeshCollider = road.GetComponent<MeshCollider>();
                if(roadMeshFilter.mesh.vertexCount > 5)
                {
                    roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
                }
                roadMeshCollider.convex = true;

                // Update properties from preview
                RoadProperties previewProperties = road.GetComponent<RoadProperties>();
                previewProperties.ChangeProperties(roadProperties);

                yield return null;
            }
        }
    }
}