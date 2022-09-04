using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewColliderScript : MonoBehaviour
{
    bool firstCollidedBool = false;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            RoadProperties colliderProperties = collision.gameObject.GetComponent<RoadProperties>();
            List<Vector3> colliderPoints = colliderProperties.points;

            if (!firstCollidedBool)
            {
                firstCollidedBool = true;
            }



            if (firstCollidedBool)
            {
                Debug.Log("Eita é mesmo");
            }
        }
    }
}
