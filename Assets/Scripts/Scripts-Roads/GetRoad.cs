using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class GetRoad : MonoBehaviour
{
    public string roadName;
    public float roadWidth;
    public int roadLanes;
    public bool oneWay;
    public Texture roadTexture;
    public Material roadMaterial;
    public int maxPointsAmount;

    List<Vector3> points = new List<Vector3>();

    private void OnEnable()
    {
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = roadName;
    }

    private void Update()
    {
        // When click on any place that is not ui
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            Debug.Log("Clicked with road");
        }

        // When right click on any place that is not ui
        if(Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            // If there's no more points, then just remove the road from the hands
            if(points.Count == 0)
            {
                Debug.Log("AAAAAAA PORRA");
                GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = "";
                Destroy(gameObject.GetComponent<GetRoad>());
            } else if (points.Count > 0)
            {
                points.RemoveAt(points.Count - 1);
            }
        }
    }
}
