using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTesting : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    [Range(0.5f, 1.5f)]
    public float spacing = 1;
    [Range(0.1f, 10f)]
    public float roadWidth = 1;
    public Camera camera;

    bool canRun = false;
    List<Vector3> points = new List<Vector3>();

    public void Run()
    {
        Debug.Log("Started");
        canRun = canRun == true ? false : true;
    }

    bool canRunStraightPreview = false;
    bool canRunCurvedPreview = false;

    GameObject straightPreviewPivot;
    GameObject gameObject;
    GameObject straightGameObject;
    void Update()
    {
        if (canRun)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log(raycast(camera));
                Debug.Log("Clicked");
                if (points.Count < 3)
                {
                    Debug.Log("Added");
                    points.Add(raycast(camera));
                }
                Debug.Log(points[0]);

                // Starts the straight preview if there's only one position in points
                if(points.Count == 1)
                {
                    Destroy(GameObject.Find("PreviewEarlyRoad"));
                    // Creates the pivot and cube of straight preview
                    straightGameObject = CreateStraightPreview(new List<Vector3> { points[0], raycast(camera) }, camera, 0.01f);
                    canRunStraightPreview = true;
                } else if(points.Count > 1)
                {
                    Destroy(straightGameObject);
                    canRunStraightPreview = false;
                }

                // Starts the curved preview if there's only two positions in points
                if (points.Count == 2)
                {
                    gameObject = CreateCurvedPreview(new List<Vector3> { points[0], points[1], raycast(camera)}, 0.01f);
                    canRunCurvedPreview = true;
                }
                else if (points.Count > 2)
                {
                    Destroy(straightPreviewPivot);
                    canRunCurvedPreview = false;
                }

                if (points.Count == 3)
                {
                    foreach(Vector3 point in points)
                    {
                        Debug.Log(point);
                    }
                    CreateRoad();
                    Destroy(gameObject);
                    Debug.Log("Created");
                }
            }

            // If player clicked curved road but not yet clicked to create a road, then make a cube where mouse is
            if (points.Count == 0)
            {
                GameObject previewEarlyRoad;
                if (GameObject.Find("PreviewEarlyRoad") == null)
                {
                    previewEarlyRoad = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    previewEarlyRoad.name = "PreviewEarlyRoad";
                }
                else
                {
                    previewEarlyRoad = GameObject.Find("PreviewEarlyRoad");
                }
                previewEarlyRoad.transform.position = raycast(camera);
                previewEarlyRoad.transform.position = new Vector3(previewEarlyRoad.transform.position.x, 1.6f, previewEarlyRoad.transform.position.z);
            }
        }

        // Update preview of straight road every frame
        if (canRunStraightPreview)
        {
            updateStraightPreview(straightGameObject, 0.1f);
        }

        // Update preview of curved road every frame
        if (canRunCurvedPreview)
        {
            updateCurvedPreview(updateCurvedMesh(gameObject.GetComponent<MeshFilter>().mesh), gameObject);
        }
    }

    // Apply the mesh into gameObject
    void updateCurvedPreview(Mesh mesh, GameObject gameObject)
    {
        gameObject.GetComponent<MeshFilter>().mesh = updateCurvedMesh(mesh);
    }

    // Update the mesh
    Mesh updateCurvedMesh(Mesh mesh)
    {
        Vector2[] bezierPoints = GetCurvedPoints(new Vector3[] { points[0], points[1], raycast(camera) }, 0.01f).ToArray();
        mesh = CreateRoadMesh(bezierPoints);
        return mesh;
    }

    // Create the gameObject of curved preview road
    GameObject CreateCurvedPreview(List<Vector3> points, float tMultiplier)
    {
        GameObject gameObject = new GameObject("Curved Preview Mesh", typeof(MeshFilter), typeof(MeshRenderer));

        Vector2[] bezierPoints = GetCurvedPoints(new Vector3[] { points[0], points[1], raycast(camera) }, 0).ToArray();
        Mesh mesh = CreateRoadMesh(bezierPoints);
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        gameObject.GetComponent<MeshRenderer>().material = material;
        gameObject.transform.position = new Vector3(0, 1.6f, 0);
        gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        points = new List<Vector3>();

        return gameObject;
    }

    // Create the gameObject of straight preview road
    GameObject CreateStraightPreview(List<Vector3> points, Camera camera, float tMultiplier)
    {
        GameObject straightGameObject = new GameObject("Straight Preview Mesh", typeof(MeshFilter), typeof(MeshRenderer));

        //Vector2[] bezierPoints = GetCurvedPoints(new Vector3[] { points[0], points[1], raycast(camera) }, 0).ToArray();
        Vector3 ray = raycast(camera);
        Vector2[] bezierPoints = new Vector2[]{ Vector2.Lerp(new Vector2(points[0].x, points[0].z), new Vector2(ray.x, ray.z), 0) };
        Mesh mesh = CreateRoadMesh(bezierPoints);
       // straightGameObject.GetComponent<MeshFilter>().mesh = mesh;

        straightGameObject.GetComponent<MeshRenderer>().material = material;
        straightGameObject.transform.position = new Vector3(points[0].x, 1.6f, points[0].z);
        straightGameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        points = new List<Vector3>();

        return straightGameObject;
    }

    // The preview of a straight road, using starting position and a mouse raycast
    void updateStraightPreview(GameObject straightGameObject, float tMultiplier)
    {
        Vector2 endPosition = Vector3To2(raycast(camera));

        Vector2 startPosition = Vector3To2(points[0]);
        float angle = -Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * (180 / Mathf.PI);
        Debug.Log(angle);
        //straightGameObject.transform.rotation = Quaternion.Euler(90, angle, 0);

        List<Vector2> bezierPoints = new List<Vector2>(); //GetCurvedPoints(new Vector3[] { points[0], points[1], raycast(camera) }, 0.01f).ToArray();
        Vector3 ray = raycast(camera);
        Debug.Log("INICIO -----------------------");
        Debug.Log(points[0]);
        Debug.Log(startPosition);
        Debug.Log(endPosition);
        for (float i = 0; i < 1; i += tMultiplier)
        {
            Vector2 calc = startPosition + i * (endPosition - startPosition);
            Debug.Log(i + "   " + calc);
            bezierPoints.Add(calc);
        }
        
        straightGameObject.GetComponent<MeshFilter>().mesh = CreateRoadMesh(bezierPoints.ToArray());//updateCurvedMesh(mesh);
    }

    // Create an empty object with the mesh
    void CreateRoad()
    {
        GameObject gameObject = new GameObject("Road", typeof(MeshFilter), typeof(MeshRenderer));

        Vector2[] bezierPoints = GetCurvedPoints(points.ToArray(), 0.01f).ToArray();
        Mesh mesh = CreateRoadMesh(bezierPoints);
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        gameObject.GetComponent<MeshRenderer>().material = material;
        gameObject.transform.position = new Vector3(0, 1.6f, 0);
        gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        points = new List<Vector3>();
    }

    // Creates the mesh of the road
    Mesh CreateRoadMesh(Vector2[] points)
    {
        // vertices = 2 * number of points
        // triangles = (2 * (number of points - 1) * 3) 
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int[] tris = new int[2 * (points.Length - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 forward = Vector2.zero;
            if (i < points.Length - 1)
            {
                forward += points[i + 1] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[i - 1];
            }
            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;

            float completionPercent = i / (float)(points.Length - 1);
            uvs[vertIndex] = new Vector2(0, completionPercent);
            uvs[vertIndex + 1] = new Vector2(1, completionPercent);


            if (i < points.Length - 1)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        return mesh;
    }


    Vector2 Vector3To2(Vector3 vector) // Get the x and z from a vector3 and put them into x and y respectively
    {
        return new Vector2(vector.x, vector.z);
    }


    // Calculate and return the points of Quadratic bezier
    List<Vector2> GetCurvedPoints(Vector3[] staticPoints, float tMultiplier)
    {
        List<Vector2> points = new List<Vector2>();
        for (float t = 0; t < 1; t += tMultiplier)
        {
            points.Add(CalculateQuadraticBezierPoint(t, Vector3To2(staticPoints[0]), Vector3To2(staticPoints[1]), Vector3To2(staticPoints[2])));
        }

        return points;
    }

    // Calculates the Quadratic bezier of 3 points (start, mid, end)
    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
    }

    // Does a raycast, returning a Vector3
    public Vector3 raycast(Camera camera)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            return new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
}
