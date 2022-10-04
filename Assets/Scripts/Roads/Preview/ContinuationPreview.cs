using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadsMeshCreator;
using CustomHelper;
using CustomDebugger;
using System;
using System.Linq;

public class ContinuationPreview : MonoBehaviour
{
    public static void CreateContinuationPreview(GameObject previewRoad, Vector3 pivotPosition, float pivotRotation, out GameObject junctionObject, out GameObject junctionPivot)
    {
        //firstRoad = collision.gameObject;
        //collidedFirstRoad = true;

        // Creates the junction object and pivot
        junctionObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        junctionPivot = new GameObject("junctionPivot");
        // Positioning Pivot
        junctionPivot.transform.SetParent(previewRoad.transform); /// atumalaca
        RoadProperties firstProperties = previewRoad.gameObject.GetComponent<RoadProperties>();
        junctionPivot.transform.position = pivotPosition;//new Vector3(firstProperties.points[firstProperties.points.Count - 1].x, 0, 0);
        junctionPivot.transform.rotation = previewRoad.transform.rotation;
        junctionPivot.transform.localRotation = Quaternion.Euler(0, pivotRotation, 0);

        junctionObject.name = "Preview Junction";

        //StartCoroutine(ContinuationPreview.UpdateJunctionPreview(0.03f, previewRoad, junctionObject, junctionPivot));
    }

    /*-----------------------------------------*/

    public static void UpdateContinuationPreview(
        Vector3 arcCenter,
        GameObject continuationObject,
        RoadProperties roadProperties,
        GameObject road,
        Vector3 pivotPosition,
        Vector3 straightPos,
        GameObject pivotObject
        )
    {
        continuationObject.transform.position = arcCenter;

        // Update Arc
        #region
        Vector3 secondVertice = new Vector3();
        Vector3 mouseRelativeToPivot = pivotObject.transform.InverseTransformPoint(Raycasts.raycastPosition3D(roadProperties.camera));
        bool inverseNormals = false;
        // Change second vertice to be the most far vertex from arc center and also sets the arc object rotation
        if (mouseRelativeToPivot.z <= 0) // Right
        {
            inverseNormals = true;
            continuationObject.transform.rotation =
                Quaternion.Euler(-90, pivotObject.transform.parent.rotation.eulerAngles.y, 0);
            secondVertice = road.transform.position + road.transform.TransformDirection(0, 0, roadProperties.width);
        }
        else if (mouseRelativeToPivot.z > 0)// Left
        {
            inverseNormals = false;
            continuationObject.transform.rotation =
                Quaternion.Euler(90, pivotObject.transform.parent.rotation.eulerAngles.y, 180);
            secondVertice = road.transform.position + road.transform.TransformDirection(0, 0, -roadProperties.width);
        }

        float arcAngle = Vector3.Angle(secondVertice - arcCenter, (arcCenter +
                    pivotObject.transform.TransformDirection(0, 0, -roadProperties.width * 2)) - arcCenter);

        // Correct arc angle depending on side
        if (mouseRelativeToPivot.z < 0) // Right
        {
            arcAngle = Math.Abs(arcAngle - 180);
        }

        List<Vector2> points = InvertedArc(arcAngle,
    ((int)roadProperties.width) / 2, -(roadProperties.width));
        //junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);

        continuationObject.GetComponent<MeshFilter>().mesh = RoadMesh.CreateMeshAlongPointsB(points, roadProperties.width, inverseNormals);
        #endregion

        // Update Road1
        #region Road Update

        MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
        Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);

        // Check if arc is pointing left or right
        //Vector3 mouseRelativeToPivot = pivotObject.transform.InverseTransformPoint(Raycasts.raycastPosition3D(roadProperties.camera));
        if (mouseRelativeToPivot.z <= 0) // Right
        {
            road.transform.position = arcCenter + road.transform.TransformDirection(new Vector3(0, 0, roadProperties.width));
        }
        else if (mouseRelativeToPivot.z > 0)// Left
        {
            road.transform.position = arcCenter + road.transform.TransformDirection(new Vector3(0, 0, -roadProperties.width));
        }

        // Road angle
        float angle = -Mathf.Atan2(endPosition.z - road.transform.position.z, endPosition.x - road.transform.position.x) * (180 / Mathf.PI);
        road.transform.rotation = Quaternion.Euler(0, angle, 0);

        Mesh newMesh = RoadMesh.CreateStraightMesh(straightPos,
            endPosition, 0.1f, roadProperties.width, roadProperties).mesh;
        roadProperties.mesh = newMesh;
        roadMeshFilter.mesh = newMesh;

        // Checks colliding and create intersections
        MeshCollider roadMeshCollider = road.GetComponent<MeshCollider>();
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
        roadMeshCollider.convex = true;
        #endregion Road Update
    }

    // Build Continuation Preview
    public static GameObject CreateContinuationAndRoad(
    Vector3 arcCenter,
        RoadProperties roadProperties,
        GameObject road,
        Vector3 pivotPosition,
        Vector3 straightPos,
        GameObject pivotObject
        )
    {
        GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        continuationObject.transform.name = "Continuation";
        continuationObject.transform.position = arcCenter;

        // Build Arc
        #region
        Vector3 secondVertice = new Vector3();
        Vector3 mouseRelativeToPivot = pivotObject.transform.InverseTransformPoint(Raycasts.raycastPosition3D(roadProperties.camera));
        bool inverseNormals = false;
        // Change second vertice to be the most far vertex from arc center and also sets the arc object rotation
        if (mouseRelativeToPivot.z <= 0) // Right
        {
            inverseNormals = true;
            continuationObject.transform.rotation =
                Quaternion.Euler(-90, pivotObject.transform.parent.rotation.eulerAngles.y, 0);
            secondVertice = road.transform.position + road.transform.TransformDirection(0, 0, roadProperties.width);
        }
        else if (mouseRelativeToPivot.z > 0)// Left
        {
            inverseNormals = false;
            continuationObject.transform.rotation =
                Quaternion.Euler(90, pivotObject.transform.parent.rotation.eulerAngles.y, 180);
            secondVertice = road.transform.position + road.transform.TransformDirection(0, 0, -roadProperties.width);
        }

        float arcAngle = Vector3.Angle(secondVertice - arcCenter, (arcCenter +
                    pivotObject.transform.TransformDirection(0, 0, -roadProperties.width * 2)) - arcCenter);

        // Correct arc angle depending on side
        if (mouseRelativeToPivot.z < 0) // Right
        {
            arcAngle = Math.Abs(arcAngle - 180);
        }

        List<Vector2> points = InvertedArc(arcAngle,
    ((int)roadProperties.width) / 2, -(roadProperties.width));
        //junctionMeshFilter.mesh = RoadMesh.CreateMeshAlongPoints(points, firstProperties.width);

        continuationObject.GetComponent<MeshFilter>().mesh = RoadMesh.CreateMeshAlongPointsB(points, roadProperties.width, inverseNormals);
        continuationObject.GetComponent<MeshRenderer>().material = roadProperties.material;
        #endregion

        // Build Road1
        #region Road Create
        GameObject finalRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
        finalRoad.name = "Road";
        finalRoad.transform.SetParent(GameObject.Find("Final Roads").transform);
        finalRoad.layer = LayerMask.NameToLayer("Road");
        //finalRoad
        MeshFilter roadMeshFilter = finalRoad.GetComponent<MeshFilter>();
        Vector3 endPosition = Raycasts.raycastPosition3D(roadProperties.camera);

        // Check if arc is pointing left or right
        //Vector3 mouseRelativeToPivot = pivotObject.transform.InverseTransformPoint(Raycasts.raycastPosition3D(roadProperties.camera));
        if (mouseRelativeToPivot.z <= 0) // Right
        {
            finalRoad.transform.position = arcCenter + road.transform.TransformDirection(new Vector3(0, 0, roadProperties.width));
        }
        else if (mouseRelativeToPivot.z > 0)// Left
        {
            finalRoad.transform.position = arcCenter + road.transform.TransformDirection(new Vector3(0, 0, -roadProperties.width));
        }

        // Road angle
        float angle = -Mathf.Atan2(endPosition.z - road.transform.position.z, endPosition.x - road.transform.position.x) * (180 / Mathf.PI);
        finalRoad.transform.rotation = Quaternion.Euler(0, angle, 0);

        Mesh newMesh = RoadMesh.CreateStraightMesh(straightPos,
            endPosition, 0.1f, roadProperties.width, roadProperties).mesh;
        roadProperties.mesh = newMesh;
        roadMeshFilter.mesh = newMesh;

        // Checks colliding and create intersections
        MeshCollider roadMeshCollider = finalRoad.GetComponent<MeshCollider>();
        roadMeshCollider.sharedMesh = roadMeshFilter.mesh;
        roadMeshCollider.convex = true;
        finalRoad.GetComponent<MeshRenderer>().material = roadProperties.material;

        Debug.Log(roadProperties);
        RoadProperties finalProperties = finalRoad.AddComponent<RoadProperties>();
        finalProperties.camera = roadProperties.camera;
        finalProperties.width = roadProperties.width;
        finalProperties.material = roadProperties.material;
        finalProperties.obstructedMaterial = roadProperties.obstructedMaterial;
        finalProperties.previewMaterial = roadProperties.previewMaterial;
        //finalProperties.
        finalProperties.points = RoadMesh.CreateStraightMesh(straightPos,
            endPosition, 0.1f, roadProperties.width, roadProperties).points;
        #endregion Road Create

        return finalRoad;
    }

    // Arc calcuations
    static protected List<Vector2> InvertedArc(float angleInDegrees, int trianglesPerRad, float width)
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
    static private int GetTriangleCount(int trianglesPerRad)
    {
        return Mathf.CeilToInt(12 * Mathf.PI * trianglesPerRad);
    }
}

