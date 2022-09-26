using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomDebugger;
using CustomHelper;

public class MainPath : MonoBehaviour
{
    // Start is called before the first frame update

    List<Vector3> anchorPoints = new List<Vector3>();
    List<Vector3> controlPoints = new List<Vector3>();
    void Start()
    {
        GameObject a = GameObject.Find("A");
        GameObject b = GameObject.Find("B");
        Vector3 pos1 = a.transform.position;
        Vector3 pos2 = pos1 + (Vector3.left + Vector3.up)*.5f;
        //Vector3 pos2 = b.transform.position;
        //Vector3 pos3 = c.transform.position;
        Vector3 pos4 = b.transform.position;
        Vector3 pos3 = pos4 + (Vector3.right + Vector3.down) * .5f;
        Debugger.Primitive(PrimitiveType.Cube, "2", pos2 * 2, Quaternion.Euler(0, 0, 0));
        Debugger.Primitive(PrimitiveType.Cube, "3", pos3 * 2, Quaternion.Euler(0, 0, 0));

        anchorPoints.Add(pos1);
        anchorPoints.Add(pos4);
        controlPoints.Add(pos1 + a.transform.TransformDirection(new Vector3(Vector3.Distance(pos1, pos4), 0, 0)));
        controlPoints.Add(pos4 + b.transform.TransformDirection(new Vector3(Vector3.Distance(pos1, pos4), 0, 0)));
        Vector3 dir = Vector3.zero;
        if (true)
        {
            Vector3 offset = anchorPoints[0] - anchorPoints[1];
            dir += offset.normalized;
            float neigDist = offset.magnitude;
        }
        if (true)
        {
            Vector3 offset = anchorPoints[1] - anchorPoints[0];
            dir -= offset.normalized;
            float neigDist = -offset.magnitude;
        }

        for (float i = 0; i < 1; i += 0.01f)
        {
            Vector3 bezierPoint = BezierCurves.Cubic(i, anchorPoints[0], controlPoints[0], controlPoints[1], anchorPoints[1]);
            GameObject cube = Debugger.Primitive(PrimitiveType.Cube, "BezierPoint", bezierPoint, Quaternion.Euler(0,0,0));
            cube.transform.SetParent(GameObject.Find("Beziers").transform);
        }
    }
    /*
    public Vector3 AutoSetControlPoints(int index)
    {
        Vector3 pos = anchorPoints[index];
        Vector3 dir = Vector3.zero;
        float neighbourDistances;

    }*/

    // Update is called once per frame
}
