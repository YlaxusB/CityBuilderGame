using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

using CustomHelper;
using RoadsMeshCreator;

public class GetRoad : MonoBehaviour
{
    public string roadName;
    public float roadWidth;
    public int roadLanes;
    public bool oneWay;
    public Texture roadTexture;
    public Material roadMaterial;
    public Material roadPreviewMaterial;
    public Material roadObstructedMaterial;
    public int maxPointsAmount;
    Camera camera;
    public float height = 0.1f;

    List<Vector3> points = new List<Vector3>();
    private void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // Set the  buidling on the hand to be the road name
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = roadName;
        roadObstructedMaterial.mainTexture = roadTexture;
        roadPreviewMaterial.mainTexture = roadTexture;
        // Start the PrePreview
        CreatePrePreview();
        
    }

    int i = 0;
    private void Update()
    {
        // When left click on any place that is not ui
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() && Raycasts.isMouseOverLayer(camera, "Terrain"))
        {
            addPoint();
            if (points.Count == 1) { CreateStraightPreview(); }
            //else if (points.Count == 2) { CurvedPreview(); }
        }

        // When right click on any place that is not ui
        if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            // If there's no more points, then just remove the road from the hands
            if (points.Count == 0)
            {
                Hands.clearHands(GameObject.Find(roadName).GetComponent<GetRoad>());
            }
            else if (points.Count > 0)
            {
                points.RemoveAt(points.Count - 1);
            }
        }
    }

    public void addPoint()
    {
        points.Add(Raycasts.raycastPosition3D(camera));
    }

    #region Previews
    // Pre Preview (The preview when you choose road but not clicked on a point yet)
    void CreatePrePreview()
    {
        // Create the initial plane
        GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
        previewRoad.transform.position = Raycasts.raycastLayer(camera, "Terrain") + new Vector3(0, height, 0);
        previewRoad.transform.rotation = Quaternion.Euler(180, 0, 0);
        previewRoad.name = "Pre Preview Road";

        // Material and textures
        MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
        roadMeshRenderer.material = roadPreviewMaterial;

        // Mesh
        MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();

        roadMeshFilter.mesh = RoadMesh.CreatePrePreviewMesh(previewRoad, Vector3Extensions.ToVector2(Raycasts.raycastLayer(camera, "Terrain")), roadWidth);
        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

        //sphere.transform.position = Vector3Extensions.ToVector2(CustomHelper.Raycasts.raycastLayer(camera, "Terrain"));

        StartCoroutine(UpdatePrePreview(previewRoad));
    }

    IEnumerator UpdatePrePreview(GameObject road)
    {
        bool canRun = true;

        // Loop that runs once every game update, breaks when user right click or left click
        while (canRun)
        {
            // Change the material if mouse is over another building (or not reaching terrain);
            if (!Raycasts.isMouseOverLayer(camera, "Terrain") && road.GetComponent<MeshRenderer>().material.name != roadObstructedMaterial.name)
            {
                MeshRenderer meshRenderer = road.GetComponent<MeshRenderer>();
                meshRenderer.material = roadObstructedMaterial;
                meshRenderer.material.name = roadObstructedMaterial.name;
            } else if (road.GetComponent<MeshRenderer>().material.name != roadPreviewMaterial.name)
            {
                MeshRenderer meshRenderer = road.GetComponent<MeshRenderer>();
                meshRenderer.material = roadPreviewMaterial;
                meshRenderer.material.name = roadPreviewMaterial.name;
            }

            // If player right click or left click then destroy this preview (if left click then starts next preview)
            if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() && Raycasts.isMouseOverLayer(camera, "Terrain"))
            {
                Hands.clearHands(GameObject.Find(roadName).GetComponent<GetRoad>());
                Destroy(road);
                StopAllCoroutines();
                CreateStraightPreview();
                yield break;
            }
            if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI())
            {
                Hands.clearHands(GameObject.Find(roadName).GetComponent<GetRoad>());
                Destroy(road);
                yield break;
            }
            // Set the position of preview to follow mouse
            road.transform.position = Raycasts.raycastLayer(camera, "Terrain") + new Vector3(0, height, 0);
            yield return null;
        }
    }

    // Straight Preview (The preview when you choose a initial point)
    void CreateStraightPreview()
    {
        // Create the initial plane
        GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
        previewRoad.transform.position = Raycasts.raycastLayer(camera, "Terrain") + new Vector3(0, height, 0);
        previewRoad.transform.rotation = Quaternion.Euler(180, 0, 0);
        previewRoad.name = "Straight Preview Road";

        // Material and textures
        MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
        roadMeshRenderer.material = roadPreviewMaterial;

        // Mesh
        MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();
        roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
            Raycasts.raycastLayer(camera, "Terrain"), 0.1f, roadWidth);
       
        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;

        //StartCoroutine(UpdateStraightPreview(previewRoad));
    }

    IEnumerator UpdateStraightPreview(GameObject road)
    {
        bool canRun = true;
        MeshFilter roadMeshFilter = road.GetComponent<MeshFilter>();
        while (canRun)
        {
            Vector3 endPosition = Raycasts.raycastPosition3D(camera);
            // Update Mesh
            roadMeshFilter.mesh = RoadMesh.CreateStraightMesh(points[0],
                Raycasts.raycastLayer(camera, "Terrain"), 0.1f, roadWidth);
            float angle = -Mathf.Atan2(endPosition.z - points[0].z, endPosition.x - points[0].x) * (180 / Mathf.PI);
            road.transform.rotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }
    }

    // Curved Preview (The preview when you choose a initial point and a mid point)
    void CurvedPreview()
    {

    }

    void UpdateCurvedPreview()
    {

    }
    #endregion
}
