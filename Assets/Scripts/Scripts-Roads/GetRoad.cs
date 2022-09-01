using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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
    public int maxPointsAmount;
    Camera camera;
    public float height = 1;

    List<Vector3> points = new List<Vector3>();
    private void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // Set the  buidling on the hand to be the road name
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = roadName;

        // Start the PrePreview
        CreatePrePreview();
    }

    int i = 0;
    private void Update()
    {
        // When left click on any place that is not ui
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            if (points.Count == 1) { StraightPreview(); }
            else if (points.Count == 2) { CurvedPreview(); }
        }

        // When right click on any place that is not ui
        if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            // If there's no more points, then just remove the road from the hands
            if (points.Count == 0)
            {
                Hands.clearHands(roadName);
            }
            else if (points.Count > 0)
            {
                points.RemoveAt(points.Count - 1);
            }
        }
    }

    // Pre Preview (The preview when you choose road but not clicked on a point yet)
    void CreatePrePreview()
    {
        // Create the initial plane
        GameObject previewRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
        previewRoad.name = "Pre Preview Road";

        // Material and textures
        MeshRenderer roadMeshRenderer = previewRoad.GetComponent<MeshRenderer>();
        Material material = roadMaterial;
        material.mainTexture = roadTexture;
        roadMeshRenderer.material = material;

        // Mesh
        MeshFilter roadMeshFilter = previewRoad.GetComponent<MeshFilter>();
        foreach(Vector3 a in roadMeshFilter.mesh.vertices)
        {
            Debug.Log(a);
        }
        roadMeshFilter.mesh = RoadMesh.CreatePrePreviewMesh(previewRoad, Vector3Extensions.ToVector2(CustomHelper.Raycasts.raycastLayer(camera, "Terrain")), roadWidth);
        previewRoad.transform.rotation = Quaternion.Euler(90, 0, 0);
        // Add this plane to preview roads "folder" and start the update preview that runs each game update
        previewRoad.transform.parent = GameObject.Find("Preview Roads").transform;
        StartCoroutine(UpdatePrePreview(previewRoad));
    }

    IEnumerator UpdatePrePreview(GameObject road)
    {
        bool canRun = true;
        // Loop that runs once every game update, breaks when user right click or left click
        while (canRun)
        {
            // If player right click or left click then destroy this preview
            if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI() ||
                Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI())
            {
                Hands.clearHands(roadName);
                Destroy(road);
                yield break;
            }
            // Set the position of preview to follow mouse
            road.transform.position = CustomHelper.Raycasts.raycastLayer(camera, "Terrain") + new Vector3(0, height, 0);
            yield return null;
        }
    }

    // Straight Preview (The preview when you choose a initial point)
    void StraightPreview()
    {

    }

    void UpdateStraightPreview()
    {

    }

    // Curved Preview (The preview when you choose a initial point and a mid point)
    void CurvedPreview()
    {

    }

    void UpdateCurvedPreview()
    {

    }
}
