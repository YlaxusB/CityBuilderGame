using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RoadsMeshCreator;

public class Junctions : MonoBehaviour
{
    public static GameObject T(Vector3 start, Vector3 end, JunctionProperties junctionProperties)
    {
        // Create the initial plane
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.name = "T-Junction";
        road.layer = LayerMask.NameToLayer("Road");

        // Material and textures
        MeshRenderer roadMeshRenderer = road.GetComponent<MeshRenderer>();
        roadMeshRenderer.material = junctionProperties.material;
        roadMeshRenderer.material.mainTexture = junctionProperties.texture;

        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        road.transform.parent = GameObject.Find("Final Roads").transform;

        // Mesh Collider
        MeshCollider roadMeshCollider = road.GetComponent<MeshCollider>();
        MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
        roadMeshCollider.convex = true;

        // Change angle of the road
        start = new Vector3(start.x, 0.2f, start.z);

        float angle = -Mathf.Atan2(end.z - start.z, end.x - start.x) * (180 / Mathf.PI);
        road.transform.rotation = Quaternion.Euler(0, angle, 0);

        // Position
        road.transform.position = start;// + road.transform.TransformDirection(new Vector3(0, 0, roadProperties.width));

        // Mesh
        roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(start, end, 0.1f, junctionProperties.width, new RoadProperties()).mesh;
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;

        return road;
    }
}


public class JunctionProperties : MonoBehaviour
{
    public float width;
    public Material material;
    public Texture texture;
}