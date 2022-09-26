using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using CustomHelper;
using RoadsMeshCreator;

public class CreateRoad : MonoBehaviour
{
    // Straight Road
    public static void Straight(List<Vector3> points, RoadProperties roadProperties, bool continuation, GameObject roadContinuation)
    {
        if (continuation)
        {
            points[0] = roadContinuation.GetComponent<ContinuationProperties>().endPos;
        }
        float multiplier = 0.1f;

        // Create the initial plane
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.name = "Road";
        road.layer = LayerMask.NameToLayer("Road");

        // Material and textures
        MeshRenderer roadMeshRenderer = road.GetComponent<MeshRenderer>();
        roadMeshRenderer.material = roadProperties.material;
        roadMeshRenderer.material.mainTexture = roadProperties.texture;

        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        road.transform.parent = GameObject.Find("Final Roads").transform;

        // Mesh Collider
        MeshCollider roadMeshCollider = road.GetComponent<MeshCollider>();
        MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
        roadMeshCollider.convex = true;

        // Change angle of the road
        points[0] = new Vector3(points[0].x, 0.2f, points[0].z);

        Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);
        float angle = -Mathf.Atan2(endPosition.z - points[0].z, endPosition.x - points[0].x) * (180 / Mathf.PI);
        road.transform.rotation = Quaternion.Euler(0, angle, 0);

        // Position
        road.transform.position = points[0];// + road.transform.TransformDirection(new Vector3(0, 0, roadProperties.width));

        // Mesh
        roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0], endPosition, multiplier, roadProperties.width, roadProperties).mesh;
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
        roadProperties = RoadMesh.CreateStraightMesh(points[0], endPosition, multiplier, roadProperties.width, roadProperties);
        road.AddComponent(roadProperties.GetType());
        RoadProperties component = road.GetComponent<RoadProperties>();
        component.ChangeProperties(roadProperties);
    }

    // Straight Continuation
    public static void StraightContinuation()
    {

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
