using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bezierRoadsScript : MonoBehaviour
{
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

    private void Start()
    {
        Debug.Log("starto");
    }

    // Update is called once per frame
    bool canRun = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            canRun = canRun == true ? false : true;
            Debug.Log(canRun);
        }
        if (canRun)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Clicou");
                // Add the point where you clicked if theres less than 3 vectors in positions list
                if (positions.Count >= 0 && positions.Count <= 2)
                {
                    Debug.Log("Adicionou ponto");
                    addPoint();
                }

                // Build the roads on last click
                if (positions.Count == 3)
                {
                    Debug.Log("Construiu rua");
                    for (float t = 0; t < 1; t += 0.01f)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.GetComponent<MeshRenderer>().material = lineMaterial;
                        cube.tag = "line";
                        cube.transform.name = "line A";
                        cube.transform.position = CalculateCubicBezierPoint(t, positions[0], positions[1], positions[2]);
                        cube.transform.position = new Vector3(cube.transform.position.x, 1.3f, cube.transform.position.z);
                    }
                    positions = new List<Vector3>();
                }

                if(positions.Count > 0 && positions.Count < 3)
                {
                    previewBezierRoad();
                }
            }
            // Remove last point with secondary mouse click
            if (Input.GetButtonDown("Fire2"))
            {
                positions.RemoveAt(positions.Count - 1);
            }

            // Preview of a bezier road
            void previewBezierRoad()
            {
                //positions.Count - 1;
            }
        }
    }

    List<Vector3> positions = new List<Vector3>();
    public Camera cam;
    void addPoint()
    {
        curvedRoadScript curvedRoadScript = FindObjectOfType<curvedRoadScript>();
        positions.Add(curvedRoadScript.raycast(cam));
        Debug.Log(positions.Count);
    }

    void removePoints()
    {
        positions = new List<Vector3>();
    }
}
