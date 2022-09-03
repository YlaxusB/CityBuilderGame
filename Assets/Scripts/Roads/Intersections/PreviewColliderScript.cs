using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewColliderScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("eae");
        if(collision.gameObject.layer == LayerMask.GetMask("Road"))
        {
            Debug.Log(collision.gameObject.GetComponent<RoadProperties>().points);
        }
    }
}
