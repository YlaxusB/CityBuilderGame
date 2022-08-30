using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class curvedRoadScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Material lineMaterial;
    void createLines()
    {

        GameObject cubeA = GameObject.Find("PointA");
        Vector3 pointA = cubeA.transform.position;

        GameObject cubeB = GameObject.Find("PointB");
        Vector3 pointB = cubeB.transform.position;

        Vector3 pointMid = pointA + ((pointB - pointA) / 2);
        GameObject cubeMid = GameObject.Find("PointMid");
        cubeMid.transform.position = pointMid;

        GameObject cubeMidHigh = GameObject.Find("PointMidHigh");
        Vector3 pointMidHigh = cubeMidHigh.transform.position;

        /*
        Vector3 pointMidHigh = pointA + ((pointB - pointA) / 2);
        pointMidHigh += new Vector3(100f, 0, 0);
        GameObject cubeMidHigh = GameObject.Find("PointMidHigh");
        cubeMidHigh.transform.position = pointMidHigh;
        */

        Debug.Log(Vector3.Distance(pointA, pointMidHigh) + "\n" + Vector3.Distance(pointB, pointMidHigh));
        for (float t = 0; t < 1; t += 0.01f)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = lineMaterial;
            cube.tag = "line";
            cube.transform.name = "line A";
            cube.transform.position = CalculateCubicBezierPoint(t, pointA, pointMidHigh, pointB);
            cube.transform.position = new Vector3(cube.transform.position.x, 1.3f, cube.transform.position.z);
            /*
            cube.transform.position = -Vector3.Slerp(pointA, pointMidHigh, i);
            cube.transform.position += ((pointA + pointB) / 2);
            cube.transform.position -= new Vector3(pointMidHigh.x - pointMid.x, 0, 0);
            cube.transform.position = new Vector3(cube.transform.position.x, 1.3f, cube.transform.position.z);
            */
        }
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
    }
    void Start()
    {

    }

    Vector3 bezier(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, t);
    }



    bool toggle = false;
    public void toggleUpdate()
    {
        if (toggle)
        {
            toggle = false;
        }
        else
        {
            toggle = true;
        }
    }

    // Update is called once per frame
    List<Vector3> positions = new List<Vector3>();
    List<GameObject> preview = new List<GameObject>();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            createLines();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {

        }
        if (toggle)
        {
            // Remove last position from list when click secondary mouse button
            if (Input.GetButtonDown("Fire2"))
            {
                if (positions.Count != 0)
                {
                    positions.RemoveAt(positions.Count - 1);
                }
            }

            // Add a position to list
            if (Input.GetButtonDown("Fire1"))
            {
                if (positions.Count <= 2) // to get 3 positions
                {
                    positions.Add(raycast(Camera));
                }

                if (positions.Count > 0 && GameObject.Find("Preview Pivot") == null)
                {
                    preview = createPreviewRoad(positions[0], raycast(Camera));
                }

                // Create static preview roads when have at least 2 positions
                if (positions.Count >= 2 && GameObject.FindGameObjectsWithTag("Preview Static Pivot").Length <= 1)
                {
                    createStaticPreview(positions[positions.Count - 2], positions[positions.Count - 1]);
                }
                // Delete previews when click (if already have two of them) and create the definitive road
                if (GameObject.FindGameObjectsWithTag("Preview Static Pivot").Length == 2)
                {
                    foreach (GameObject preview in GameObject.FindGameObjectsWithTag("Preview Static Pivot"))
                    {
                        Destroy(preview);
                    }
                    createFinalRoad();
                }
            }
            // Update the moving preview
            if (positions.Count <= 2 && positions.Count > 0 && GameObject.Find("Preview Pivot") != null)
            {
                preview[0].transform.position = positions[positions.Count - 1];
                updatePreviewRoad(positions[positions.Count - 1], raycast(Camera), preview[0], preview[1]);
            }
            else if (positions.Count == 3 && GameObject.Find("Preview Pivot") != null)
            {
                Destroy(GameObject.Find("Preview Pivot"));
            }
        }
    }

    void createFinalRoad()
    {
        // Get the click positions
        Vector3 startPosition = positions[0];
        Vector3 midPosition = positions[1];
        Vector3 endPosition = positions[2];

        // The angle between first and last positions
        float angle = -Mathf.Atan2(endPosition.z - startPosition.z, endPosition.x - startPosition.x) * (180 / Mathf.PI);

        GameObject roadPivot = new GameObject();
        roadPivot.transform.name = "Road Pivot";
        roadPivot.transform.position = startPosition;
        roadPivot.transform.rotation = Quaternion.Euler(0, angle, 0);
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.transform.SetParent(roadPivot.transform);
        road.name = "road";
        MeshFilter mf = road.GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        Mesh mesh = mf.sharedMesh;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        vertices.Add(road.transform.TransformPoint(new Vector3(0, 0, 1)));
        vertices.Add(road.transform.TransformPoint(new Vector3(0, 0, -1)));

        GameObject pivot1 = new GameObject();
        pivot1.transform.SetParent(road.transform);
        pivot1.transform.position = positions[1];
        pivot1.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(positions[1].z - positions[0].z, positions[1].x - positions[0].x) * (180 / Mathf.PI), 0);

        vertices.Add(pivot1.transform.TransformPoint(0, 0, 1));
        vertices.Add(pivot1.transform.TransformPoint(0, 0, -1));

        GameObject pivot2 = new GameObject();
        pivot2.transform.SetParent(road.transform);
        pivot2.transform.position = positions[2];
        pivot2.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(positions[2].z - positions[1].z, positions[2].x - positions[1].x) * (180 / Mathf.PI), 0);
        vertices.Add(pivot2.transform.TransformPoint(0, 0, 1));
        vertices.Add(pivot2.transform.TransformPoint(0, 0, -1));
        for (int i = 0; i < vertices.Count; i++)
        {
            //triangles.Add(i);
        }
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        // mesh.triangles = triangleIndices;
    }

    int angleMultiple = 0;
    public Material previewMaterial;
    List<GameObject> createPreviewRoad(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject previewPivot = new GameObject();
        previewPivot.name = "Preview Pivot";
        GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        previewRoad.name = "Preview Road";
        previewRoad.GetComponent<MeshRenderer>().material = previewMaterial;
        previewRoad.transform.position = new Vector3(0.5f, 0, 0);
        previewRoad.transform.parent = previewPivot.transform;
        previewPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        previewPivot.transform.position = startPosition;

        List<GameObject> values = new List<GameObject>();
        values.Add(previewPivot);
        values.Add(previewRoad);
        return values;
    }

    void createStaticPreview(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject previewPivot = new GameObject();
        previewPivot.tag = "Preview Static Pivot";
        previewPivot.name = "Preview Static Pivot";
        GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        previewRoad.name = "Preview Static Road";
        previewRoad.GetComponent<MeshRenderer>().material = previewMaterial;
        previewRoad.transform.position = new Vector3(0.5f, 0, 0);
        previewRoad.transform.parent = previewPivot.transform;
        float angle = -Mathf.Atan2(endPosition.z - startPosition.z, endPosition.x - startPosition.x) * (180 / Mathf.PI);
        previewPivot.transform.rotation = Quaternion.Euler(0, angle, 0);
        previewPivot.transform.position = startPosition;
        previewPivot.transform.localScale = new Vector3(Vector3.Distance(endPosition, startPosition), 1, 1);
    }

    void updatePreviewRoad(Vector3 startPosition, Vector3 endPosition, GameObject previewPivot, GameObject previewRoad)
    {
        float angle = -Mathf.Atan2(endPosition.z - startPosition.z, endPosition.x - startPosition.x) * (180 / Mathf.PI);
        previewPivot.transform.rotation = Quaternion.Euler(0, angle, 0);
        previewPivot.transform.localScale = new Vector3(Vector3.Distance(endPosition, startPosition), 1, 1);
    }

    public Camera Camera;
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