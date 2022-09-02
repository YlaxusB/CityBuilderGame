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
    public static void clearHands(Component component)
    {
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = new string("");
        Destroy(component);
    }
}


