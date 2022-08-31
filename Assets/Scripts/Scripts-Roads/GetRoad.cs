using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class GetRoad : MonoBehaviour
{
    public string roadName;
    public float roadWidth;
    public float roadLanes;
    public bool oneWay;
    public Texture roadTexture;
    public Material roadMaterial;

    private void OnEnable()
    {
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = roadName;
    }

    private void OnMouseDown()
    {
        //Screen.add

    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            Debug.Log(roadWidth);
            Debug.Log(roadLanes);
            Debug.Log(oneWay);
            Debug.Log(roadTexture);
            Debug.Log(roadMaterial);
        }
    }
}
