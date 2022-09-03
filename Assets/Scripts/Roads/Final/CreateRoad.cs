using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomHelper;
using RoadsMeshCreator;

public class CreateRoad : MonoBehaviour
{
    // Straight Road
    public static void Straight(List<Vector3> points, RoadProperties roadProperties)
    {
        float multiplier = 0.1f;

        // Create the initial plane
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.transform.position = new Vector3(Mathf.Round(points[0].x), roadProperties.height, Mathf.Round(points[0].z));
        road.transform.rotation = Quaternion.Euler(180, 0, 0);
        road.name = "Road";
        road.layer = LayerMask.GetMask("Road");

        // Material and textures
        MeshRenderer roadMeshRenderer = road.GetComponent<MeshRenderer>();
        roadMeshRenderer.material = roadProperties.material;
        roadMeshRenderer.material.mainTexture = roadProperties.texture;

        // Mesh
        MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
        roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
            Raycasts.raycastLayer(roadProperties.camera, "Terrain"), 0.1f, roadProperties.width, roadProperties).mesh;

        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        road.transform.parent = GameObject.Find("Final Roads").transform;
        roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0], points[1], multiplier, roadProperties.width, roadProperties).mesh;

        // Mesh Collider
        MeshCollider roadMeshCollider = road.GetComponent<MeshCollider>();
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
        roadMeshCollider.convex = true;

        // Change angle of the road
        float angle = -Mathf.Atan2(points[1].z - points[0].z, points[1].x - points[0].x) * (180 / Mathf.PI);
        road.transform.rotation = Quaternion.Euler(0, angle, 0);


        roadProperties = RoadMesh.CreateStraightMesh(points[0], points[1], multiplier, roadProperties.width, roadProperties);

        road.AddComponent(roadProperties.GetType());
        RoadProperties component = road.GetComponent<RoadProperties>();
        component.ChangeProperties(roadProperties);
    }

    // Bezier Road
    public static void Bezier(List<Vector3> points, RoadProperties roadProperties)
    {
        float multiplier = 0.01f;

        // Create the initial plane
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.transform.position = new Vector3(points[0].x, roadProperties.height, points[0].z);
        road.transform.rotation = Quaternion.Euler(180, 0, 0);
        road.name = "Road";

        // Material and textures
        MeshRenderer roadMeshRenderer = road.GetComponent<MeshRenderer>();
        roadMeshRenderer.material = roadProperties.material;
        roadMeshRenderer.material.mainTexture = roadProperties.texture;

        // Mesh
        MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
       // roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
       //     Raycasts.raycastLayer(roadProperties.camera, "Terrain"), 0.1f, roadProperties.width);

        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        road.transform.parent = GameObject.Find("Final Roads").transform;
        roadMeshFilter.mesh = RoadMesh.CreateBezierMesh(points[0], points[1], points[2], multiplier, roadProperties.width);

        // Change angle of the road
        //float angle = -Mathf.Atan2(points[1].z - points[0].z, points[1].x - points[0].x) * (180 / Mathf.PI);
        road.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}