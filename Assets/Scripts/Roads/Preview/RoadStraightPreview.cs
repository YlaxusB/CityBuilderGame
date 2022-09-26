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
        public static GameObject Create(RoadProperties roadProperties, List<Vector3> points, bool continuation)
        {
            // Create the initial plane
            GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
            previewRoad.transform.position = new Vector3(Mathf.Round(points[0].x), roadProperties.height, Mathf.Round(points[0].z));//Raycasts.raycastLayer(roadProperties.camera, "Terrain") + new Vector3(0, roadProperties.height, 0);
            previewRoad.transform.rotation = Quaternion.Euler(180, 0, 0);
            if (continuation)
            {
                //previewRoad.transform.position += previewRoad.transform.TransformDirection(new Vector3(5, 0, 0));
            }
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
            float distanceFromPreviousRoad = roadProperties.width * 2f;
            Vector3 startPos = points[0];
            bool canRun = true;
            while (canRun && points.Count > 0)
            {
                Vector3 junctionPos = points[0];
                Vector3 endJunction = points[0];
                Vector3 startJunction = points[0];
                float width = 0;

                if (GameObject.Find("Preview Junction") != null)
                {
                    GameObject previewJunction = GameObject.Find("Preview Junction");
                    ContinuationProperties continuationProperties = previewJunction.GetComponent<ContinuationProperties>();
                    width = continuationProperties.width;
                    junctionPos = previewJunction.transform.position;
                    startJunction = previewJunction.GetComponent<ContinuationProperties>().startPos;
                    endJunction = previewJunction.GetComponent<ContinuationProperties>().endPos;
                }


                MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
                Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera) +
                    road.transform.TransformDirection(new Vector3(0, 0, -roadProperties.width));
                //wapoints[0] = pos + road.transform.TransformDirection(new Vector3(10, 0, 0));

                // Rotate road 
                float angle = -Mathf.Atan2(endPosition.z - junctionPos.z, endPosition.x - junctionPos.x) * (180 / Mathf.PI);
                GameObject.Find("1B").transform.position = junctionPos;
                GameObject.Find("2B").transform.position = endPosition; ;
                road.transform.rotation = Quaternion.Euler(0, angle, 0);

                // Update Mesh //
                road.transform.position = junctionPos + road.transform.TransformDirection(new Vector3(0,0, width));
                //road.transform.position = new Vector3(road.transform.position.x, 0.2f, road.transform.position.z);
                Mesh newMesh = RoadMesh.CreateStraightMesh(points[0],
                    endPosition, 0.1f, roadProperties.width, roadProperties).mesh;
                roadProperties.mesh = newMesh;
                roadMeshFilter.mesh = newMesh;

                // Checks colliding and create intersections
                MeshCollider roadMeshCollider = road.GetComponent<MeshCollider>();
                roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
                roadMeshCollider.convex = true;

                // Update properties from preview
                RoadProperties previewProperties = road.GetComponent<RoadProperties>();
                previewProperties.ChangeProperties(roadProperties);

                yield return null;
            }
        }
    }
}