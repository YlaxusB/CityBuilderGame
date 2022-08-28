using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
public class ButtonsFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GameObject.Find("Road Button").GetComponent<Button>();
        btn.onClick.AddListener(buttonRoad);
    }
    public bool canRun = false;
    Vector3 roadStart;
    Vector3 roadEnd;
    void buttonRoad()
    {
        canRun = true;
        roadStart = new Vector3();
        roadEnd = new Vector3();
        click = 0;
    }

    public double click = 0;
    void setRoad()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (click == 0)
            {
                roadStart = raycast("Road Start");
            } else if(click == 1)
            {
                roadEnd = raycast("Road End");
                buildRoad();
                click = -1;

            }
        }
        Vector3.Distance(roadStart, roadEnd);
    }

    public GameObject prefab;
    void buildRoad()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "road";
        // Angle of the road
        float angle = -Mathf.Atan2(roadEnd.z - roadStart.z, roadEnd.x - roadStart.x) * (180 / Mathf.PI);
        cube.transform.localRotation = Quaternion.Euler(0, angle, 0);

        // Position and size
        cube.transform.position = new Vector3(roadStart.x + ((roadEnd.x - roadStart.x) / 2), roadStart.y, roadStart.z +  ((roadEnd.z - roadStart.z) / 2));
        cube.transform.localScale = new Vector3(Vector3.Distance(roadEnd, roadStart), 1, 1);
    }
    
    public Vector3 raycast(string cubeName)
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            return new Vector3(hit.point.x, hit.point.y, hit.point.z);
        } else
        {
            return new Vector3(0, 0, 0);
        }
    }
    // Update is called once per frame
    public GameObject previewPivot;
    public GameObject previewCube;
    void Update()
    {
        if (canRun)
        {
            setRoad();
            if (Input.GetButtonDown("Fire1"))
            {
                previewPivot = new GameObject();
                previewPivot.name = "Preview Pivot";
                previewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                previewCube.transform.position = new Vector3(0.5f, 0, 0);
                previewCube.name = "Preview Road Cube";
                previewCube.transform.SetParent(previewPivot.transform);
                click++;
            }
            if (click == 1)
            {
                previewRoad(previewCube, previewPivot);
            } else if (click == 0)
            {
                Destroy(GameObject.Find("Preview Pivot"));
            }
        }
    }

    float lastAngle = 0;
    public int angleMultiple = 90;
    void previewRoad(GameObject cube,  GameObject pivot)
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        Vector3 roadMouse = hit.point;

        float angle = -Mathf.Atan2(roadMouse.z - roadStart.z, roadMouse.x - roadStart.x) * (180 / Mathf.PI);
        if (angleMultiple != 0)
        {
            angle = round(((int)angle), angleMultiple);
        }

        // Position and size
        pivot.transform.position = roadStart;//new Vector3(roadStart.x + ((roadMouse.x - roadStart.x) / 2), roadStart.y, roadStart.z + ((roadMouse.z - roadStart.z) / 2));
        pivot.transform.localRotation = Quaternion.Euler(0, angle, 0);
        pivot.transform.localScale = new Vector3(Vector3.Distance(roadMouse, roadStart), 1, 1);
    }

    static int round(int n, int multiple)
    {
        // Smaller multiple
        int a = (n / multiple) * multiple;

        // Larger multiple
        int b = a + multiple;

        // Return of closest of two
        return (n - a > b - n) ? b : a;
    }
}
