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
                float firstPointsToExclude = Mathf.Ceil(firstProperties.width / 10) + 1;
                float previewPointsToExclude = Mathf.Ceil(previewProperties.width / 10) + 1;
                Mesh newMesh = RoadMesh.UpdatePreviousMesh(firstProperties, ((int)firstPointsToExclude));

                // Delete the last x points of the previous road mesh
                MeshFilter firstCollidedMeshFilter = firstCollidedObject.gameObject.GetComponent<MeshFilter>();
                firstCollidedMeshFilter.mesh = newMesh;

                // Delete the first x points of this preview road mesh
                gameObject.GetComponent<MeshFilter>().mesh = RoadMesh.UpdatePreviewMesh(previewProperties,
                    ((int)previewPointsToExclude));

                // Continuation mesh
                /*
                GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                continuationObject.transform.position = firstCollidedObject.transform.position; //- firstProperties.points[firstProperties.points.Count - 1];
                continuationObject.transform.rotation = firstCollidedObject.transform.rotation;
                continuationObject.transform.position += continuationObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]);
                continuationObject.transform.rotation = firstCollidedObject.transform.rotation;
                continuationObject.name = "continuation object";
                */

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
                float firstPointsToExclude = Mathf.Ceil(firstProperties.width / 5) + 1;
                float previewPointsToExclude = Mathf.Ceil(previewProperties.width / 5) + 1;
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

        continuationObject.transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        continuationObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        // The end of previous road
        endCollidedRoad = firstCollidedObject.transform.position +
            (firstCollidedObject.transform.TransformDirection(firstProperties.points[firstProperties.points.Count - 1]));

        // The start of preview (after the points are deleted)
        Vector3 previewStart = transform.position + (transform.TransformDirection(previewProperties.points[0]));

        // The mid point to bezier, but its just the first (deleted) point of preview start
        Vector3 midPoint = transform.position;

        // Create and apply the new mesh
        continuationMeshFilter.mesh = RoadMesh.CreateStraightContinuationMesh(endCollidedRoad, midPoint, previewStart, 0.01f, firstProperties.width, firstCollidedObject.transform.rotation.y);
        //AdjustContinuation(firstCollidedObject, gameObject, continuationObject);
        return continuationObject;
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
