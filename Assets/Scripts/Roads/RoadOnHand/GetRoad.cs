using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

using UnityEngine.Events;

using CustomHelper;
using RoadsMeshCreator;
using Preview;

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
    //public object roadProperties = new object();
    Camera camera;
    public float height = 0.1f;

    RoadProperties roadProperties = new RoadProperties();
    List<Vector3> points = new List<Vector3>();
    GameObject previewRoad;
    private void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        // Set the  buidling on the hand to be the road name
        GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand = roadName;
        roadObstructedMaterial.mainTexture = roadTexture;
        roadPreviewMaterial.mainTexture = roadTexture;

        // The properties to pass to preview creation
        roadProperties.name = roadName;
        roadProperties.width = roadWidth;
        roadProperties.lanes = roadLanes;
        roadProperties.oneWay = oneWay;
        roadProperties.texture = roadTexture;
        roadProperties.material = roadMaterial;
        roadProperties.previewMaterial = roadPreviewMaterial;
        roadProperties.obstructedMaterial = roadObstructedMaterial;
        roadProperties.maxPointsAmount = maxPointsAmount;
        roadProperties.camera = camera;
        roadProperties.height = height;

        // Start the PrePreview
        previewRoad = PrePreview.Create(roadProperties);
        StartCoroutine(PrePreview.Update(previewRoad, roadProperties));
    }

    int i = 0;
    private void Update()
    {
        // When left click on any place that is not ui
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() && Raycasts.isMouseOverLayer(camera, "Terrain"))
        {
            Destroy(previewRoad);
            StopAllCoroutines();
            addPoint();
            if (points.Count == 1)
            {
                previewRoad = StraightPreview.Create(roadProperties, points);
                StartCoroutine(StraightPreview.Update(previewRoad, roadProperties, points));
            }
            else if (points.Count == 2)
            {
                Hands.clearHands(GameObject.Find(roadProperties.name).GetComponent<GetRoad>());
                //previewRoad = CurvedPreview.Create(roadProperties, points);
                // CurvedPreview.Update(previewRoad, roadProperties, points);
            }
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
}


public class RoadProperties
{
    public string name;
    public float width;
    public int lanes;
    public bool oneWay;
    public Texture texture;
    public Material material;
    public Material previewMaterial;
    public Material obstructedMaterial;
    public int maxPointsAmount;
    public Camera camera;
    public float height;
}