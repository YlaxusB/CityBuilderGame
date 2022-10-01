using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomHelper;
using RoadsMeshCreator;


namespace Preview
{
    public static class PrePreview
    {
        // Pre Preview (The preview when you choose road but not clicked on a point yet)
        public static GameObject Create(RoadProperties roadProperties)
        {
            // Create the initial plane
            GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
            previewRoad.transform.position = Raycasts.raycastLayer(roadProperties.camera, "Terrain") + new Vector3(0, roadProperties.height, 0);
            previewRoad.transform.rotation = Quaternion.Euler(180, 0, 0);

            // Material and textures
            MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
            roadMeshRenderer.material = roadProperties.previewMaterial;

            // Mesh
            MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();

            roadMeshFilter.mesh = RoadMesh.CreatePrePreviewMesh(previewRoad, Vector3Extensions.ToVector2(Raycasts.raycastLayer(roadProperties.camera, "Terrain")), roadProperties.width);
            // Add this plane to preview roads "folder" and start the update preview that runs each game update
            previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

            return previewRoad;
        }

        // Update the PrePreview
        public static IEnumerator Update(GameObject road, RoadProperties roadProperties, Vector3 suggestedEnd, GetRoad component)
        {
            bool canRun = true;

            // Loop that runs once every game update, breaks when user right click or left click
            while (canRun)
            {
                // Change the material if mouse is over another building (or not reaching terrain);
                if (!Raycasts.isMouseOverLayer(roadProperties.camera, "Terrain") && road.GetComponent<MeshRenderer>().material.name != roadProperties.obstructedMaterial.name)
                {
                    MeshRenderer meshRenderer = road.GetComponent<MeshRenderer>();
                    meshRenderer.material = roadProperties.obstructedMaterial;
                    meshRenderer.material.name = roadProperties.obstructedMaterial.name;
                }
                else if (road.GetComponent<MeshRenderer>().material.name != roadProperties.previewMaterial.name)
                {
                    MeshRenderer meshRenderer = road.GetComponent<MeshRenderer>();
                    meshRenderer.material = roadProperties.previewMaterial;
                    meshRenderer.material.name = roadProperties.previewMaterial.name;
                }


                // Set the position of preview to follow mouse
                Vector3 endPosition = Raycasts.raycastLayer(roadProperties.camera, "Terrain") + new Vector3(0, roadProperties.height, 0);
                suggestedEnd = component.suggestedEnd;
                if (suggestedEnd != Vector3.zero && suggestedEnd.x != Mathf.Infinity)
                {
                    Debug.Log(suggestedEnd);
                    endPosition = suggestedEnd;
                }
                road.transform.position = new Vector3(endPosition.x, endPosition.y, endPosition.z);
                yield return null;
            }
        }
    }
}