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
    RoadProperties firstProperties;

    float firstPointsToExclude = 0;
    float lastPointsToExclude = 0;
    GameObject junctionObject;

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
                firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();

                RoadProperties lastProperties = gameObject.GetComponent<RoadProperties>();
                // The points that will be excluded, to create the junction
                firstPointsToExclude = Mathf.Ceil(firstProperties.width * 1);
                lastPointsToExclude = Mathf.Ceil(lastProperties.width * 0.5f);

                // Removes the points of the first road starting from end
                MeshFilter firstMeshFilter = firstRoad.GetComponent<MeshFilter>();
                Debug.Log("AAAAAAAAAAAAAAAAAAA");
                Debug.Log(firstRoad.transform.TransformDirection(new Vector3(lastProperties.width, 0, 0)));
                Debug.Log(transform.position);
                transform.position += firstRoad.transform.TransformDirection(new Vector3(lastProperties.width * 10,0,0));
                //Mesh newMesh = RoadMesh.RemoveMeshPoints(firstProperties, ((int)firstPointsToExclude), false);
                //firstMeshFilter.mesh = newMesh;
                //firstProperties.mesh = newMesh;

                // Creates the junction object
                junctionObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                junctionObject.name = "Junction Object";

                // Takes the end of the first road
                Vector3 firstRoadEnd = firstRoad.transform.position +
                    (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

                // Junction Positioning
                junctionObject.transform.position = firstRoadEnd;
                junctionObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                junctionObject.GetComponent<MeshFilter>().mesh = lastProperties.mesh;

                MeshFilter junctionMeshFilter = junctionObject.GetComponent<MeshFilter>();

                // Start of second road and mid between first and second road
                RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();
                Vector3 secondRoadStart = transform.position +
                    (transform.TransformDirection(secondProperties.points[secondProperties.points.Count - 1]));
                Vector3 midPosition = firstRoadEnd + (secondRoadStart - firstRoadEnd);

                StartCoroutine(UpdateJunctionPreview(0.03f));

            }
        }

    }

    // Loop to update the junction preview
    private IEnumerator UpdateJunctionPreview(float multiplier)
    {
        while (true)
        {
            RoadProperties firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();
            RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();

            MeshFilter junctionMeshFilter = junctionObject.GetComponent<MeshFilter>();

            // End, Start and Mid of the junction
            Vector3 firstRoadEnd = firstRoad.transform.position +
                (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
            Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));
            secondRoadStart = new Vector3((float)(secondRoadStart.x * 100f) / 100f,
                (float)(secondRoadStart.y * 100f) / 100f, (float)(secondRoadStart.z * 100f) / 100f);
            Vector3 midPosition = firstRoadEnd + (secondRoadStart - firstRoadEnd);

            // Anchor Points
            List<Vector2> anchorPoints = new List<Vector2>();
            anchorPoints.Add(Vector3Extensions.ToVector2(firstRoadEnd - firstRoadEnd));
            anchorPoints.Add(Vector3Extensions.ToVector2(secondRoadStart - firstRoadEnd));

            // Control Points
            List<Vector2> controlPoints = new List<Vector2>();
            controlPoints.Add(Vector3Extensions.ToVector2(firstRoadEnd + firstRoad.transform.TransformDirection(new Vector3(Vector3.Distance(secondRoadStart, firstRoadEnd) / 2f, 0, 0))));
            controlPoints.Add(Vector3Extensions.ToVector2(secondRoadStart + transform.TransformDirection(new Vector3(-Vector3.Distance(secondRoadStart, firstRoadEnd) / 2f, 0, 0))));

            controlPoints[0] -= Vector3Extensions.ToVector2(firstRoadEnd);
            controlPoints[1] -= Vector3Extensions.ToVector2(firstRoadEnd);

            MeshFilter continuationMeshFilter = junctionObject.GetComponent<MeshFilter>();
            continuationMeshFilter.mesh =
                RoadMesh.CreateStraightContinuationMesh(anchorPoints, controlPoints, multiplier, firstProperties.width);

            yield return null;
        }

    }

    // Build the final junction object
    public GameObject BuildContinuation(float multiplier)
    {
        RoadProperties firstProperties = firstRoad.gameObject.GetComponent<RoadProperties>();
        RoadProperties secondProperties = gameObject.GetComponent<RoadProperties>();

        MeshFilter continuationMeshFilter = junctionObject.GetComponent<MeshFilter>();

        // End, Start and Mid of the junction
        Vector3 firstRoadEnd = firstRoad.transform.position +
            (firstRoad.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));
        Vector3 secondRoadStart = transform.position + (transform.TransformDirection(secondProperties.points[0]));
        secondRoadStart = new Vector3((float)(secondRoadStart.x * 100f) / 100f,
            (float)(secondRoadStart.y * 100f) / 100f, (float)(secondRoadStart.z * 100f) / 100f);
        Vector3 midPosition = firstRoadEnd + (secondRoadStart - firstRoadEnd);

        // Positioning junction
        junctionObject.transform.position = new Vector3(firstRoadEnd.x, 0.2f, firstRoadEnd.z);
        junctionObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Anchor Points
        List<Vector2> anchorPoints = new List<Vector2>();
        anchorPoints.Add(Vector3Extensions.ToVector2(firstRoadEnd - firstRoadEnd));
        anchorPoints.Add(Vector3Extensions.ToVector2(secondRoadStart - firstRoadEnd));

        // Control Points
        List<Vector2> controlPoints = new List<Vector2>();
        controlPoints.Add(Vector3Extensions.ToVector2(firstRoadEnd + firstRoad.transform.TransformDirection(new Vector3(Vector3.Distance(secondRoadStart, firstRoadEnd) / 2f, 0, 0))));
        controlPoints.Add(Vector3Extensions.ToVector2(secondRoadStart + transform.TransformDirection(new Vector3(-Vector3.Distance(secondRoadStart, firstRoadEnd) / 2f, 0,0))));

        Debugger.Primitive(PrimitiveType.Cube, "1", firstRoadEnd + Vector3Extensions.ToVector3(anchorPoints[0]), Quaternion.Euler(0, 0, 0));
        Debugger.Primitive(PrimitiveType.Cube, "2", Vector3Extensions.ToVector3(controlPoints[0]), Quaternion.Euler(0, 0, 0));
        Debugger.Primitive(PrimitiveType.Cube, "3", Vector3Extensions.ToVector3(controlPoints[1]), Quaternion.Euler(0, 0, 0));
        Debugger.Primitive(PrimitiveType.Cube, "4", firstRoadEnd + Vector3Extensions.ToVector3(anchorPoints[1]), Quaternion.Euler(0, 0, 0));

        controlPoints[0] -= Vector3Extensions.ToVector2(firstRoadEnd);
        controlPoints[1] -= Vector3Extensions.ToVector2(firstRoadEnd);
        continuationMeshFilter.mesh =
            RoadMesh.CreateStraightContinuationMesh(anchorPoints, controlPoints, multiplier, firstProperties.width);
        return junctionObject;
    }

    /*
     // Get continuation object
public GameObject GetContinuation()
{
    RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
    RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();
    GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
    MeshFilter continuationMeshFilter = continuationObject.GetComponent<MeshFilter>();
    continuationObject.name = "EAE";
    Vector3 endCollidedRoad = firstCollidedObject.transform.position +
        (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

    continuationObject.transform.position = new Vector3(endCollidedRoad.x, 0.2f, endCollidedRoad.z);
    continuationObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    // The end of previous road
    endCollidedRoad = firstCollidedObject.transform.position +
        (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

    // The start of preview (after the points are deleted)
    Vector3 previewStart = transform.position + (transform.TransformDirection(previewProperties.points[0]));

    // The mid point to bezier, but its just the first (deleted) point of preview start
    Vector3 midPoint = transform.position;

    // Create and apply the new mesh
    Vector3 mid2 = endCollidedRoad + (firstCollidedObject.transform.TransformDirection(new Vector3(Vector3.Distance(previewStart, endCollidedRoad) ,0,0)));
    List<Vector2> anchorPoints = new List<Vector2>();
    anchorPoints.Add(Vector3Extensions.ToVector2(endCollidedRoad));
    anchorPoints.Add(Vector3Extensions.ToVector2(previewStart));
    List<Vector2> controlPoints = new List<Vector2>();
    float dist = Vector3.Distance(endCollidedRoad, previewStart);
    Vector2 ctrl2 = Vector3Extensions.ToVector2(previewStart + transform.TransformDirection(new Vector3(-(dist / 2), 0, 0)));
    //Vector2 ctrl2 = Vector3Extensions.ToVector2(previewStart + transform.TransformDirection(previewProperties.points[0]) - transform.TransformDirection(new Vector3(-(dist / 2), 0, 0)));
    Debug.Log(-Vector3.Distance(ctrl2, previewStart));
    //controlPoints.Add(endCollidedRoad + ((previewStart - endCollidedRoad) / 2));
    //controlPoints.Add(endCollidedRoad + ((previewStart - endCollidedRoad) / 2));
    //controlPoints.Add(Vector3Extensions.ToVector2(endCollidedRoad + firstCollidedObject.transform.TransformDirection(new Vector3(dist / 2, 0,0))));
    controlPoints.Add(ctrl2);
    controlPoints.Add(Vector3Extensions.ToVector2(endCollidedRoad + firstCollidedObject.transform.TransformDirection(new Vector3(Vector3.Distance(anchorPoints[0], controlPoints[0]), 0, 0))));
    Vector3 intersection = new Vector3();


    continuationMeshFilter.mesh = RoadMesh.CreateStraightContinuationMesh(anchorPoints, controlPoints, 0.001f, firstProperties.width);
    //AdjustContinuation(firstCollidedObject, gameObject, continuationObject);
    return continuationObject;
}
     */

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
/*
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
    bool firstCollidedBool = false;
    GameObject firstCollidedObject;

    GameObject a;
    GameObject b;
    GameObject c;
    GameObject d;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            Debug.Log(firstCollidedBool);
            RoadProperties colliderProperties = collision.gameObject.GetComponent<RoadProperties>();
            List<Vector3> colliderPoints = colliderProperties.points;

            if (!firstCollidedBool)
            {
                firstCollidedObject = collision.gameObject;
                firstCollidedBool = true;

                //////////
                RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
                RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();

                // The points that will be excluded, to create their connections
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                float firstPointsToExclude = Mathf.Ceil(firstProperties.width * 1);
                float previewPointsToExclude = Mathf.Ceil(previewProperties.width * 1);
                //previewPointsToExclude = firstPointsToExclude;
                // Remove the last points of collided road
                Mesh newMesh = RoadMesh.UpdatePreviousMesh(firstProperties, ((int)(firstPointsToExclude * 0.75f)));

                // Delete the last x points of the previous road mesh
                MeshFilter firstCollidedMeshFilter = firstCollidedObject.gameObject.GetComponent<MeshFilter>();
                firstCollidedMeshFilter.mesh = newMesh;

                // Delete the first x points of this preview road mesh
                gameObject.GetComponent<MeshFilter>().mesh = RoadMesh.UpdatePreviewMesh(previewProperties,
                    ((int)(previewPointsToExclude * 0.75f) + 1));

                // Continuation mesh

                //GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //continuationObject.transform.position = firstCollidedObject.transform.position; //- firstProperties.points[firstProperties.points.Count - 1];
                //continuationObject.transform.rotation = firstCollidedObject.transform.rotation;
                //continuationObject.transform.position += continuationObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]);
                //continuationObject.transform.rotation = firstCollidedObject.transform.rotation;
                //continuationObject.name = "continuation object";


// Create a bezier between last road and the new created road

GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            continuationObject.name = "continuation object";
            Vector3 endCollidedRoad = firstCollidedObject.transform.position +
                (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

            continuationObject.transform.position = endCollidedRoad;
            continuationObject.transform.rotation = Quaternion.Euler(90, 0, 0);

            MeshFilter continuationMeshFilter = continuationObject.GetComponent<MeshFilter>();

            Vector3 midPoint = firstCollidedObject.transform.position;

            Vector3 previewStart = transform.position + (transform.TransformDirection(previewProperties.points[0]));//previewProperties.points[0]));

            // Debugg
            a = Debugger.Primitive(PrimitiveType.Cube, "a", endCollidedRoad, Quaternion.Euler(0,0,0));
            b = Debugger.Primitive(PrimitiveType.Cube, "b", midPoint, Quaternion.Euler(0, 0, 0));
            c = Debugger.Primitive(PrimitiveType.Cube, "c", previewStart, Quaternion.Euler(0, 0, 0));

            d = Debugger.Primitive(PrimitiveType.Cube, "d", new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));

            // Start updating the connection
            StartCoroutine(UpdateStart());
        }


        // Check if road is colliding with the previous road (in case of road continuation)
        if (firstCollidedBool)
        {
            RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
            List<Vector3> firstPoints = firstProperties.points;

            RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();

            // How many points will be exluded when a road is pointing left or right (in relation to previous road)
            // pointing to a side will be there more colliing points, so the width divided by 5 (5 is the spacing between road points and)
            // is the quantity of points needed to exclude (+ 1 because the last point is less than 5 spacing)
            float firstPointsToExclude = Mathf.Ceil(firstProperties.width / 7) + 1;
            float previewPointsToExclude = Mathf.Ceil(previewProperties.width / 1) + 1;
            // Keep removing 
            //MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            //filter.mesh = RoadMesh.UpdatePreviewMesh(previewProperties, ((int)previewPointsToExclude));
        }
    }
}

private IEnumerator UpdateStart()
{
    while (true)
    {
        RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
        RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();

        GameObject continuationObject = GameObject.Find("continuation object");
        MeshFilter continuationMeshFilter = continuationObject.GetComponent<MeshFilter>();

        // The end of previous road
        Vector3 endCollidedRoad = firstCollidedObject.transform.position +
            (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1] - new Vector3(0.5f, 0, 0)));

        // The start of preview (after the points are deleted)
        Vector3 previewStart = transform.position + (transform.TransformDirection(previewProperties.points[0]) + new Vector3(0.5f, 0, 0));

        // The mid point to bezier, but its just the first (deleted) point of preview start
        Vector3 midPoint = transform.position;

        // Create and apply the new mesh
        continuationMeshFilter.mesh = RoadMesh.CreateBezierMesh(endCollidedRoad, midPoint, previewStart, 0.01f, firstProperties.width);

        // Debugg
        a.transform.position = endCollidedRoad;
        b.transform.position = midPoint;
        c.transform.position = previewStart;

        List<Mesh> meshes = new List<Mesh>();
        meshes.Add(firstCollidedObject.GetComponent<MeshFilter>().mesh);
        meshes.Add(gameObject.GetComponent<MeshFilter>().mesh);
        meshes.Add(continuationMeshFilter.mesh);
        MeshFilter dMeshf = d.GetComponent<MeshFilter>();
        dMeshf.mesh = RoadMesh.CombineMeshes(meshes);

        yield return null;
    }
}

// Get continuation object
public GameObject GetContinuation()
{
    RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
    RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();
    GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
    MeshFilter continuationMeshFilter = continuationObject.GetComponent<MeshFilter>();
    continuationObject.name = "EAE";
    Vector3 endCollidedRoad = firstCollidedObject.transform.position +
        (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

    continuationObject.transform.position = new Vector3(endCollidedRoad.x, 0.2f, endCollidedRoad.z);
    continuationObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    // The end of previous road
    endCollidedRoad = firstCollidedObject.transform.position +
        (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

    // The start of preview (after the points are deleted)
    Vector3 previewStart = transform.position + (transform.TransformDirection(previewProperties.points[0]));

    // The mid point to bezier, but its just the first (deleted) point of preview start
    Vector3 midPoint = transform.position;

    // Create and apply the new mesh
    Vector3 mid2 = endCollidedRoad + (firstCollidedObject.transform.TransformDirection(new Vector3(Vector3.Distance(previewStart, endCollidedRoad) ,0,0)));
    List<Vector2> anchorPoints = new List<Vector2>();
    anchorPoints.Add(Vector3Extensions.ToVector2(endCollidedRoad));
    anchorPoints.Add(Vector3Extensions.ToVector2(previewStart));
    List<Vector2> controlPoints = new List<Vector2>();
    float dist = Vector3.Distance(endCollidedRoad, previewStart);
    Vector2 ctrl2 = Vector3Extensions.ToVector2(previewStart + transform.TransformDirection(new Vector3(-(dist / 2), 0, 0)));
    //Vector2 ctrl2 = Vector3Extensions.ToVector2(previewStart + transform.TransformDirection(previewProperties.points[0]) - transform.TransformDirection(new Vector3(-(dist / 2), 0, 0)));
    Debug.Log(-Vector3.Distance(ctrl2, previewStart));
    //controlPoints.Add(endCollidedRoad + ((previewStart - endCollidedRoad) / 2));
    //controlPoints.Add(endCollidedRoad + ((previewStart - endCollidedRoad) / 2));
    //controlPoints.Add(Vector3Extensions.ToVector2(endCollidedRoad + firstCollidedObject.transform.TransformDirection(new Vector3(dist / 2, 0,0))));
    controlPoints.Add(ctrl2);
    controlPoints.Add(Vector3Extensions.ToVector2(endCollidedRoad + firstCollidedObject.transform.TransformDirection(new Vector3(Vector3.Distance(anchorPoints[0], controlPoints[0]), 0, 0))));
    Vector3 intersection = new Vector3();


    continuationMeshFilter.mesh = RoadMesh.CreateStraightContinuationMesh(anchorPoints, controlPoints, 0.001f, firstProperties.width);
    //AdjustContinuation(firstCollidedObject, gameObject, continuationObject);
    return continuationObject;
}
public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
{
    Vector3 lineVec3 = linePoint2 - linePoint1;
    Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
    Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

    float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

    //is coplanar, and not parrallel
    if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
    {
        float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
        intersection = linePoint1 + (lineVec1 * s);
        return true;
    }
    else
    {
        intersection = Vector3.zero;
        return false;
    }
}
// Adjust continuation
public void AdjustContinuation(GameObject collidedObject, GameObject previewObject, GameObject continuationObject)
{
    MeshFilter collidedMF = collidedObject.GetComponent<MeshFilter>();
    MeshFilter previewMF = previewObject.GetComponent<MeshFilter>();
    MeshFilter continuationMF = continuationObject.GetComponent<MeshFilter>();

    Mesh newMesh = new Mesh();
    List<Vector3> newVertices = new List<Vector3>();
    List<int> newTriangles = new List<int>();

    int oldestLastIndex = continuationMF.mesh.triangles.Length - 1;
    //newVertices = continuationMF.mesh.vertices.ToList();
    //newTriangles = continuationMF.mesh.triangles.ToList();
    Debug.Log(newTriangles.Count - 1);
    Debug.Log(newVertices.Count - 1);

    newVertices.Add(collidedMF.mesh.vertices[collidedMF.mesh.vertices.Length]);
    newVertices.Add(collidedMF.mesh.vertices[collidedMF.mesh.vertices.Length - 1]);
    newVertices.Add(continuationMF.mesh.vertices[0]);
    newVertices.Add(continuationMF.mesh.vertices[1]);

    newTriangles.Add(0);
    newTriangles.Add(2);
    newTriangles.Add(3);

    newTriangles.Add(1);
    newTriangles.Add(2);
    newTriangles.Add(3);

    Debug.Log("a");
    newMesh.vertices = newVertices.ToArray();
    newMesh.triangles = newTriangles.ToArray();
    //continuationMF.mesh = newMesh;
    Debug.Log("b");


    GameObject asd = GameObject.CreatePrimitive(PrimitiveType.Plane);
    MeshFilter asdf = asd.GetComponent<MeshFilter>();
    asdf.mesh.Clear();
    asd.transform.position = continuationMF.transform.position;
    asd.transform.rotation = continuationMF.transform.rotation;
    asdf.mesh = newMesh;
    //return continuationObject;
}

// Destroy the continuation preview
public void DestroyContinuation()
{
    Destroy(a);
    Destroy(b);
    Destroy(c);
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
*/