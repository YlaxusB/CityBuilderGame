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
    public Vector3 arcEnd;
    ContinuationProperties continuationProperties;

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
                continuationProperties =
                    junctionObject.AddComponent<ContinuationProperties>();

                // End from first road and Start from second road
                RoadProperties firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();
                RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();
                Vector3 firstRoadEnd = firstRoad.transform.position +
                    (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
                Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));

                continuationProperties.width = firstProperties.width;
                continuationProperties.startPos = firstRoadEnd;
                continuationProperties.endPos = secondRoadStart;
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

            // Define Arc Angle
            Vector3 secondVertice = transform.position + transform.TransformDirection(0, 0, firstProperties.width);
            Vector3 firstRoadEndCenter = junctionObject.transform.position +
                junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
            float angleFirstRoadToArcCenter = -Mathf.Atan2(firstRoadEndCenter.z - transform.position.z,
                 firstRoadEndCenter.x - transform.position.x) * (180 / Mathf.PI);
            float angleArcCenterToLastVertice = -Mathf.Atan2(transform.position.z - secondVertice.z,
                transform.position.x - secondVertice.x) * (180 / Mathf.PI);
            float localAngle1 = (junctionObject.transform.eulerAngles.y - angleArcCenterToLastVertice);
            float localAngle2 = (junctionObject.transform.eulerAngles.y - angleFirstRoadToArcCenter);
            float arcAngle = Mathf.Abs(Mathf.Clamp(localAngle2, 0, 90) -90) * 2;

            // Get the arc points and build mesh
            List<Vector2> points = CalculateVertices(arcAngle,
                ((int)firstProperties.width) / 2, -(firstProperties.width));
            junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);

            //InsertProperties(points, firstProperties); // Insert values into continuation object
            yield return null;
        }

    }

    /*
    private void InsertProperties(List<Vector2> points, RoadProperties roadProperties)
    {
        // Set the continuation properties
        ContinuationProperties continuationProperties = junctionObject.GetComponent<ContinuationProperties>();
        continuationProperties.startPos = new Vector3(points[0].x, 0.2f, points[0].y);
        continuationProperties.endPos = new Vector3(points[points.Count - 1].x, 0.2f, points[points.Count - 1].y);
        continuationProperties.width = roadProperties.width;
    }
    */

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

        junctionObject.GetComponent<MeshRenderer>().material = firstProperties.material;

        // Positioning junction
        junctionObject.transform.rotation = Quaternion.Euler(0, firstRoad.transform.eulerAngles.y - 90, 180);
        junctionObject.transform.position = firstRoadEnd + (firstRoadEnd - firstRoadEnd) +
            junctionObject.transform.TransformDirection(new Vector3(firstProperties.width, 0, 0));

        // Define Arc Angle
        Vector3 secondVertice = transform.position + transform.TransformDirection(0, 0, firstProperties.width);
        Vector3 firstRoadEndCenter = junctionObject.transform.position +
            junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
        float angleFirstRoadToArcCenter = -Mathf.Atan2(firstRoadEndCenter.z - transform.position.z,
             firstRoadEndCenter.x - transform.position.x) * (180 / Mathf.PI);
        float angleArcCenterToLastVertice = -Mathf.Atan2(transform.position.z - secondVertice.z,
            transform.position.x - secondVertice.x) * (180 / Mathf.PI);
        float localAngle1 = (junctionObject.transform.eulerAngles.y - angleArcCenterToLastVertice);
        float localAngle2 = (junctionObject.transform.eulerAngles.y - angleFirstRoadToArcCenter);
        float arcAngle = Mathf.Abs(Mathf.Clamp(localAngle2, 0, 90) - 90) * 2;
        // Create Arc
        List<Vector2> points = CalculateVertices(arcAngle,
            ((int)firstProperties.width) / 2, -(firstProperties.width));
        continuationMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);
        arcEnd = transform.position;

        continuationProperties.endPos = GameObject.Find("Straight Preview Road").transform.position;//transform.position;
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
    /*
    public void InsertProperties(RoadProperties roadProperties, GameObject continuation)
    {
        MeshRenderer continuationMaterial = continuation.GetComponent<MeshRenderer>();
        continuationMaterial.material = roadProperties.material;
        continuationMaterial.material.mainTexture = roadProperties.texture;
    }
    */
}