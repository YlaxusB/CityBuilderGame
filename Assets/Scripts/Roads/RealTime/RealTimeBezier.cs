using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomDebugger;
using CustomHelper;
using RoadsMeshCreator;
using UnityEngine.InputSystem;
public class RealTimeBezier : MonoBehaviour
{
    // Anchor Point / A-B
    // Control Point / A-B
    GameObject AA;
    GameObject AB;
    GameObject CA;
    GameObject CB;
    List<Vector3> points = new List<Vector3>();

    GameObject road;
    GameObject straightRoad1;
    GameObject straightRoad2;

    public Material material;
    void Start()
    {
        AA = GameObject.Find("AA");
        AB = GameObject.Find("AB");
        CA = GameObject.Find("CA");
        CB = GameObject.Find("CB");

        road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.name = "RealTimeRoad";

        straightRoad1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        straightRoad1.name = "straightRoad1";

        straightRoad2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        straightRoad2.name = "straightRoad2";
    }

    bool keyPressed = true;
    void Update()
    {
        float multiplier = 0.01f;
        float width = 3;

        if (Input.GetKeyDown(KeyCode.G))
        {
            keyPressed = keyPressed ? false : true;
        }
        Vector3 AAP = AA.transform.position;
        Vector3 ABP = AB.transform.position;
        Vector3 CAP = CA.transform.position;
        Vector3 CBP = CB.transform.position;

        // Update control points posiition
        if (keyPressed)
        {
            CA.transform.position = AAP + AA.transform.TransformDirection(
                new Vector3((Vector3.Distance(AAP, ABP) / 2 + width * 2), 0, 0));

            CB.transform.position = ABP + AB.transform.TransformDirection(
                new Vector3(-(Vector3.Distance(AAP, ABP) / 2 + width * 2), 0, 0));
        }

        // Get anchor and control points (Anchor 1 - Control 1 - Control 2 - Anchor 2)
        points = new List<Vector3>()
            { AA.transform.position, CA.transform.position,
            CB.transform.position, AB.transform.position };

        road.GetComponent<MeshFilter>().mesh = 
            RoadMesh.CreateStraightContinuationMesh(
                new List<Vector2>() { Vector3Extensions.ToVector2(points[0]), Vector3Extensions.ToVector2(points[3]) },
                new List<Vector2>() { Vector3Extensions.ToVector2(points[1]), Vector3Extensions.ToVector2(points[2]) },
                multiplier, width);
        road.transform.position = AA.transform.position;
        road.transform.position += new Vector3(0, 0.2f, 0);
        road.transform.rotation = Quaternion.Euler(90, 0, 0);
        road.GetComponent<MeshRenderer>().material = material;

        // Straight roads;
        RoadProperties straightRoad1Properties = straightRoad1.AddComponent<RoadProperties>();
        straightRoad1.GetComponent<MeshFilter>().mesh =
            RoadMesh.CreateStraightMesh(points[0],
                points[0] - AA.transform.TransformDirection(new Vector3(-10, 0, 0)),
                multiplier, width, straightRoad1Properties).mesh;

        straightRoad1.transform.position = points[0];
        straightRoad1.transform.position += new Vector3(0, 0.2f, 0);
        straightRoad1.transform.rotation = AA.transform.rotation;
        straightRoad1.transform.rotation = Quaternion.Euler(180, AA.transform.rotation.eulerAngles.y, 180);
        straightRoad1.GetComponent<MeshRenderer>().material = material;


        RoadProperties straightRoad2Properties = straightRoad2.AddComponent<RoadProperties>();
        straightRoad2.GetComponent<MeshFilter>().mesh =
            RoadMesh.CreateStraightMesh(points[3], points[3] + AB.transform.TransformDirection(new Vector3(10, 0, 0)),
            multiplier, width, straightRoad2Properties).mesh;

        straightRoad2.transform.position = points[3];
        straightRoad2.transform.position += new Vector3(0, 0.2f, 0);
        straightRoad2.transform.rotation = AB.transform.rotation;
        straightRoad2.GetComponent<MeshRenderer>().material = material;
    }
}
