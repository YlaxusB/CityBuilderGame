using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadsMeshCreator;
using CustomHelper;

public class PreviewColliderScript : MonoBehaviour
{
    bool firstCollidedBool = false;
    GameObject firstCollidedObject;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            Debug.Log(firstCollidedBool);
            RoadProperties colliderProperties = collision.gameObject.GetComponent<RoadProperties>();
            List<Vector3> colliderPoints = colliderProperties.points;

            if (!firstCollidedBool)
            {
                firstCollidedObject = collision.gameObject;
                firstCollidedBool = true;

                //////////
                RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
                RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();

                float firstPointsToExclude = Mathf.Ceil(firstProperties.width / 5) + 1;
                float previewPointsToExclude = Mathf.Ceil(previewProperties.width / 5) + 1;
                Mesh newMesh = RoadMesh.UpdatePreviousMesh(firstProperties, ((int)firstPointsToExclude));


                MeshFilter firstCollidedMeshFilter = firstCollidedObject.gameObject.GetComponent<MeshFilter>();
                firstCollidedMeshFilter.mesh = newMesh;

                gameObject.GetComponent<MeshFilter>().mesh = RoadMesh.UpdatePreviewMesh(previewProperties,
                    ((int)previewPointsToExclude));

                // Continuation mesh
                GameObject continuationObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                continuationObject.transform.position = firstCollidedObject.transform.position; //- firstProperties.points[firstProperties.points.Count - 1];
                continuationObject.transform.rotation = firstCollidedObject.transform.rotation;
                continuationObject.name = "continuation object";

                // Create a bezier tomorrow
            }



            if (firstCollidedBool)
            {
                RoadProperties firstProperties = firstCollidedObject.gameObject.GetComponent<RoadProperties>();
                List<Vector3> firstPoints = firstProperties.points;

                RoadProperties previewProperties = gameObject.GetComponent<RoadProperties>();

                // How many points will be exluded when a road is pointing left or right (in relation to previous road)
                // pointing to a side will be there more colliing points, so the width divided by 5 (5 is the spacing between road points and)
                // is the quantity of points needed to exclude (+ 1 because the last point is less than 5 spacing)
                float firstPointsToExclude = Mathf.Ceil(firstProperties.width / 5) + 1;
                float previewPointsToExclude = Mathf.Ceil(previewProperties.width / 5) + 1;
                //RoadMesh.UpdatePreviousMesh(firstProperties, ((int)firstPointsToExclude));
                MeshFilter filter = gameObject.GetComponent<MeshFilter>();
                filter.mesh = RoadMesh.UpdatePreviewMesh(previewProperties, ((int)previewPointsToExclude));
                Debug.Log(firstPointsToExclude);
                Debug.Log(previewPointsToExclude);

                Debug.Log("Eita é mesmo");
            }
        }
    }
}
