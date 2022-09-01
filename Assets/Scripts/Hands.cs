using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Hands : MonoBehaviour
{
    public string buildingOnHand = new string("");
    /*
    if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI())
    {
        Debug.Log(UIToolkitRaycastChecker.IsPointerOverUI());
    }
    */
    public static void clearHands(string buildingName)
    {
        Debug.Log(buildingName);
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = new string("");
        Destroy(GameObject.Find(buildingName).GetComponent<GetRoad>());
        Debug.Log("Cleared Hands");
        Debug.Log("Current Building on Hand : " + GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand);
    }
}


