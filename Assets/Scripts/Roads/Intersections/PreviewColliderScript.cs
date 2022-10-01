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
    public GameObject firstRoad;
    GameObject junctionObject;
    GameObject junctionPivot;
    public Vector3 arcEnd;
    ContinuationProperties continuationProperties;

    public Vector3 a1;
    public float a2;


    public float d1;
    public float d2;
    public float d3;
    public bool d4;

    public float ab;
    public float ac;
    public float cb;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            // Get properties from the collided road
            RoadProperties collidedProperties = collision.gameObject.GetComponent<RoadProperties>();
            List<Vector3> collidedPoints = collidedProperties.points;

            // Checks if it's the first time the preview is colliding with a road, if yes then
            // this will be the first road
            if (!collidedFirstRoad)
            {
                firstRoad = collision.gameObject;
                collidedFirstRoad = true;

                // Creates the junction object and pivot
                junctionObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                junctionPivot = new GameObject("junctionPivot");
                // Positioning Pivot
                junctionPivot.transform.SetParent(firstRoad.transform); /// atumalaca
                RoadProperties firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();
                junctionPivot.transform.localPosition = new Vector3(firstProperties.points[firstProperties.points.Count - 1].x, 0, 0);
                junctionPivot.transform.rotation = firstRoad.transform.rotation;

                continuationProperties =
                    junctionObject.AddComponent<ContinuationProperties>();

                // End from first road and Start from second road
                RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();
                Vector3 firstRoadEnd = firstRoad.transform.position +
                    (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
                Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));

                continuationProperties.width = firstProperties.width;
                continuationProperties.startPos = firstRoadEnd;
                continuationProperties.endPos = secondRoadStart;
                junctionObject.name = "Preview Junction";

                junctionObject.GetComponent<MeshRenderer>().material = firstProperties.previewMaterial;
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

            junctionPivot.transform.localPosition = new Vector3(firstProperties.points[firstProperties.points.Count - 1].x, 0, 0);
            junctionPivot.transform.rotation = firstRoad.transform.rotation;
            a1 = junctionPivot.transform.InverseTransformPoint(Raycasts.raycastPosition3D(firstProperties.camera));

            // End from first road and Start from second road
            Vector3 firstRoadEnd = firstRoad.transform.position +
                (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
            Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));

            float arcAngle;
            if (a1.z <= 0) // Right
            {
                // Define Arc Angle
                Vector3 start = junctionObject.transform.position + junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
                Vector3 secondVertice = transform.position + transform.TransformDirection(0, 0, firstProperties.width);
                Vector3 firstRoadEndCenter = junctionObject.transform.position +
                    junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
                arcAngle = Math.Abs(Vector3.Angle(firstRoadEndCenter + firstRoad.transform.TransformDirection(0, 0, firstProperties.width) -
                    junctionObject.transform.position, junctionObject.transform.position - secondVertice) - 180);

                // Positioning junction
                junctionObject.transform.rotation = Quaternion.Euler(-90, firstRoad.transform.eulerAngles.y - 90 + 180 + 180, 180);
                junctionObject.transform.position = junctionPivot.transform.position +
                    junctionPivot.transform.TransformDirection(new Vector3(0, 0, -firstProperties.width));
            }
            else // Left
            {
                // Define Arc Angle
                Vector3 start = junctionObject.transform.position + junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
                Vector3 secondVertice = transform.position + transform.TransformDirection(0, 0, -firstProperties.width);
                Vector3 firstRoadEndCenter = junctionObject.transform.position +
                    junctionObject.transform.TransformDirection(-firstProperties.width, 0, 0);
                arcAngle = Math.Abs(Vector3.Angle(firstRoadEndCenter + firstRoad.transform.TransformDirection(0, 0, firstProperties.width) -
                    junctionObject.transform.position, junctionObject.transform.position - secondVertice));
                //arcAngle = Math.Abs(Vector3.Angle(junctionPivot.transform.InverseTransformPoint(junctionObject.transform.position), junctionPivot.transform.InverseTransformPoint(secondVertice)) - 180);
                //arcAngle = Math.Abs(Vector3.Angle(junctionPivot.transform.InverseTransformPoint(junctionObject.transform.position), junctionPivot.transform.InverseTransformPoint(secondVertice)) - 180);
                /* ------- Actually the best method to determine arc angle  ----------------------*/
                arcAngle = Vector3.Angle(secondVertice - junctionObject.transform.position, (junctionObject.transform.position +
                    junctionPivot.transform.TransformDirection(0, 0, -firstProperties.width * 2)) - junctionObject.transform.position);
                // Positioning junction
                var euler = transform.parent.eulerAngles;
                if (!d4)
                {
                    d1 = 180 + 360 + 180 + 90;
                    d2 = firstRoad.transform.eulerAngles.y - 90 + 180 + 180;
                    d3 = 180 + 360 + 180;
                }
                junctionObject.transform.eulerAngles = new Vector3(d1, d2, d3);
                junctionObject.transform.position = junctionPivot.transform.position +
                    junctionPivot.transform.TransformDirection(new Vector3(0, 0, firstProperties.width));
                ab = Vector3.Angle(junctionObject.transform.position, junctionPivot.transform.position + junctionPivot.transform.TransformDirection(0, 0, -firstProperties.width));
                ac = Vector3.Angle(junctionObject.transform.position, secondVertice) * ((float)(180 / Math.PI));
                cb = Vector3.Angle(secondVertice - junctionObject.transform.position, (junctionObject.transform.position + 
                    junctionPivot.transform.TransformDirection(0,0,-firstProperties.width * 2)) - junctionObject.transform.position);
                GameObject.Find("1C").transform.position = junctionObject.transform.position +
                    junctionPivot.transform.TransformDirection(0, 0, -firstProperties.width * 2);
            }


            // Get the arc points and build mesh
            //List<Vector2> points = CalculateVertices(arcAngle,
            //    ((int)firstProperties.width) / 2, -(firstProperties.width));
            a2 = arcAngle;
            List<Vector2> points = InvertedArc(arcAngle,
                ((int)firstProperties.width) / 2, -(firstProperties.width));
            //junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);
            bool inverseNormals = false;
            if (a1.z <= 0)
            {
                inverseNormals = true;
            }
            junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPointsB(points, firstProperties.width, inverseNormals);

            //InsertProperties(points, firstProperties); // Insert values into continuation object
            yield return null;
        }

    }
    protected List<Vector2> InvertedArc(float angleInDegrees, int trianglesPerRad, float width)
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
        if (a1.z <= 0) // Right
        {
            junctionObject.transform.rotation = Quaternion.Euler(0, firstRoad.transform.eulerAngles.y - 90, 0);
            junctionObject.transform.rotation = Quaternion.Euler(-90, firstRoad.transform.eulerAngles.y - 90, 180);
            junctionObject.transform.position = firstRoadEnd + (firstRoadEnd - firstRoadEnd) +
                junctionObject.transform.TransformDirection(new Vector3(firstProperties.width, 0, 0));
        }
        else // Left
        {
            junctionObject.transform.rotation = Quaternion.Euler(90, firstRoad.transform.eulerAngles.y - 90, 0);
            junctionObject.transform.position = firstRoadEnd + (firstRoadEnd - firstRoadEnd) +
                junctionObject.transform.TransformDirection(new Vector3(firstProperties.width, 0, 0));
        }


        float arcAngle = a2;


        // Get the arc points and build mesh
        //List<Vector2> points = CalculateVertices(arcAngle,
        //    ((int)firstProperties.width) / 2, -(firstProperties.width));
        //a2 = arcAngle;
        List<Vector2> points = InvertedArc(arcAngle,
            ((int)firstProperties.width) / 2, -(firstProperties.width));
        //junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);
        bool inverseNormals = false;
        if (a1.z <= 0)
        {
            inverseNormals = true;
        }
        junctionObject.GetComponent<MeshRenderer>().material = firstProperties.material;
        continuationMeshFilter.mesh = RoadMesh.CreateMeshAlongPointsB(points, firstProperties.width, inverseNormals);
        arcEnd = transform.position;

        continuationProperties.endPos = GameObject.Find("Straight Preview Road").transform.position;//transform.position;
        return junctionObject;
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