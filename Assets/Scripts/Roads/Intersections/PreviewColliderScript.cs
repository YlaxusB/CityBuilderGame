using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadsMeshCreator;
using CustomHelper;
using CustomDebugger;
using System;
using System.Linq;
public class PreviewColliderScript : MonoBehaviour
{
    bool collidedFirstRoad;
    GameObject firstRoad;
    GameObject junctionObject;

    public float a1;
    public float a2;
    public float a3;
    public float a4;

    float finalValue = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            // Get properties from the collided road
            RoadProperties collidedProperties = collision.gameObject.GetComponent<RoadProperties>();
            List<Vector3> collidedPoints = collidedProperties.points;

            // Checks if it's the first time the preview is colliding with a road, if yes then this will be the first road
            if (!collidedFirstRoad)
            {
                firstRoad = collision.gameObject;
                collidedFirstRoad = true;

                // Creates the junction object
                junctionObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                junctionObject.AddComponent<ContinuationProperties>();
                junctionObject.name = "Preview Junction";
                StartCoroutine(UpdateJunctionPreview(0.03f));

            }
        }

    }

    // Loop to update the already existing junction preview
    private IEnumerator UpdateJunctionPreview(float multiplier)
    {
        while (true)
        {
            RoadProperties firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();
            RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();
            MeshFilter junctionMeshFilter = junctionObject.GetComponent<MeshFilter>();

            // End from first road and Start from second road
            Vector3 firstRoadEnd = firstRoad.transform.position +
                (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
            Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));

            // Positioning junction
            junctionObject.transform.rotation = Quaternion.Euler(0, firstRoad.transform.eulerAngles.y - 90, 180);
            junctionObject.transform.position = firstRoadEnd + (firstRoadEnd - firstRoadEnd) +
                junctionObject.transform.TransformDirection(new Vector3(firstProperties.width, 0, 0));

            // Get arc angle
            Vector3 endPosition = Raycasts.raycastPosition3D(firstProperties.camera);
            Vector3 start = junctionObject.transform.position + junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
            float angle = -Mathf.Atan2(start.z - transform.position.z,
                start.x - transform.position.x) * (180 / Mathf.PI);
            angle = Math.Abs(firstRoad.transform.eulerAngles.y - transform.transform.eulerAngles.y);
            finalValue = angle;//(Math.Abs(angle) - (firstRoad.transform.eulerAngles.y - Math.Abs(angle)));
            a1 = angle;
            a2 = firstRoad.transform.eulerAngles.y;
            GameObject.Find("1A").transform.position = transform.position;
            GameObject.Find("2A").transform.position = junctionObject.transform.position +
                junctionObject.transform.TransformDirection(-firstProperties.width * 2, 0, 0);
            a4 = Vector3.Distance(junctionObject.transform.position +
                junctionObject.transform.TransformDirection(-firstProperties.width * 2, 0, 0), 
                transform.position);
            finalValue = angle;
            if(finalValue < 0)
            {
                finalValue = Math.Abs(finalValue) + Math.Abs(180 - Math.Abs(finalValue));
            }
            //finalValue = Vector3.Dot(junctionObject.transform.position +
           //     junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0), transform.position) * Mathf.Deg2Rad;
            a3 = finalValue;

            Vector3 secondVertice = transform.position + transform.TransformDirection(0,0,firstProperties.width);

            GameObject.Find("1A").transform.position = secondVertice;
            var degrees = angle;
            var radians = degrees * Mathf.Deg2Rad;
            var x = Mathf.Cos(radians);
            var y = Mathf.Sin(radians);
            //var pos = Vector3(x, y, 0); //Vector2 is fine, if you're in 2D

            // Get the arc points and build mesh
            List<Vector2> points = CalculateVertices(finalValue,
                ((int)firstProperties.width) / 2, -(firstProperties.width));
            junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);

            InsertProperties(points, firstProperties); // Insert values into continuation object
            yield return null;
        }

    }

    private void InsertProperties(List<Vector2> points, RoadProperties roadProperties)
    {
        // Set the continuation properties
        ContinuationProperties continuationProperties = junctionObject.GetComponent<ContinuationProperties>();
        continuationProperties.startPos = new Vector3(points[0].x, 0.2f, points[0].y);
        continuationProperties.endPos = new Vector3(points[points.Count - 1].x, 0.2f, points[points.Count - 1].y);
        continuationProperties.width = roadProperties.width;
    }

    // Build the final junction object
    public GameObject BuildContinuation(float multiplier)
    {
        junctionObject.name = "Junction";
        RoadProperties firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();
        RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();
        MeshFilter continuationMeshFilter = junctionObject.GetComponent<MeshFilter>();

        // End from first road and Start from second road
        Vector3 firstRoadEnd = firstRoad.transform.position +
            (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
        Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));

        // Positioning junction
        junctionObject.transform.rotation = Quaternion.Euler(0, firstRoad.transform.eulerAngles.y - 90, 180);
        junctionObject.transform.position = firstRoadEnd + (firstRoadEnd - firstRoadEnd) +
            junctionObject.transform.TransformDirection(new Vector3(firstProperties.width, 0, 0));

        // Get the arc points and build mesh
        List<Vector2> points = CalculateVertices(finalValue,
            ((int)firstProperties.width) / 2, -(firstProperties.width));
        continuationMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);


        InsertProperties(points, firstProperties);
        // Get arc angle
        Vector3 endPosition = Raycasts.raycastPosition3D(firstProperties.camera);
        Vector3 start = junctionObject.transform.position + junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
        float angle = -Mathf.Atan2(start.z - transform.position.z,
            start.x - transform.position.x) * (180 / Mathf.PI);
        finalValue = angle;
        List<Vector2> asd = ArcPercentage(360 * 0.25f,
            ((int)firstProperties.width) / 2, -(firstProperties.width));
        foreach (Vector2 vector in asd.ToArray())
        {
            Debugger.Primitive(PrimitiveType.Cube, "Testing", junctionObject.transform.position +
                new Vector3(vector.x, 0.2f, vector.y), Quaternion.Euler(0, 0, 0));
        }
        Debug.Log(asd.Count);
        return junctionObject;
    }

    protected List<Vector2> ArcPercentage(float angleInDegrees, int trianglesPerRad, float width)
    {
        var triangleCount = GetTriangleCount(trianglesPerRad);
        float sectorAngle = Mathf.Deg2Rad * angleInDegrees;
        var vertices = new List<Vector2>();
        //vertices.Add(Vector2.zero);
        for (int i = 0; i <= triangleCount + 1; i++)
        {
            float theta = i / (float)triangleCount * sectorAngle;
            var vertex = (new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * width) * 2;
            vertices.Add(new Vector2(vertex.x, vertex.z));
        }
        //vertices.Add();
        return vertices;
    }

    /*
    [Header("Mesh Options")]
    [Min(0)]
    [SerializeField]
    private int trianglesPerRad = 5;
    [SerializeField]
    [Range(0f, 360f)]
    private float angleInDegrees = 270f;
    */
    // Create arc
    protected List<Vector2> CalculateVertices(float angleInDegrees, int trianglesPerRad, float width)
    {
        var triangleCount = GetTriangleCount(trianglesPerRad);
        float sectorAngle = Mathf.Deg2Rad * angleInDegrees;
        var vertices = new List<Vector2>();
        //vertices.Add(Vector2.zero);
        for (int i = 0; i <= triangleCount + 1; i++)
        {
            float theta = i / (float)triangleCount * sectorAngle;
            var vertex = (new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * width) * 2;
            vertices.Add(new Vector2(vertex.x, vertex.z));
        }
        //vertices.Add();
        return vertices;
    }
    // Get how many triangles the arc will have
    private int GetTriangleCount(int trianglesPerRad)
    {
        return Mathf.CeilToInt(12 * Mathf.PI * trianglesPerRad);
    }

    // Destroy the continuation preview
    public void DestroyContinuation()
    {
        Destroy(GameObject.Find("continuation object"));
        StopAllCoroutines();
    }

    // Insert some properties to road continuation
    public void InsertProperties(RoadProperties roadProperties, GameObject continuation)
    {
        MeshRenderer continuationMaterial = continuation.GetComponent<MeshRenderer>();
        continuationMaterial.material = roadProperties.material;
        continuationMaterial.material.mainTexture = roadProperties.texture;

    }
}