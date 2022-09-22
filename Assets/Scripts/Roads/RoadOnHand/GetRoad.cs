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
    public float height = 0.2f;

    RoadProperties roadProperties = new RoadProperties();
    List<Vector3> points = new List<Vector3>();
    GameObject previewRoad;

    string shape = "straight";
    public void ChangeShape(string newShape)
    {
        shape = newShape;
    }

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
    private bool continuation = false;
    private void Update()
    {
        // When left click on any place that is not ui
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() && Raycasts.isMouseOverLayer(camera, "Terrain"))
        {
            Destroy(previewRoad);
            StopAllCoroutines();
            addPoint();

            // if the player selected the straight shape and there are 2 points, then remove the old preview and
            // create the final road
            if (shape == "straight" && points.Count == 2)
            {
                // Check if it's a continuation
                if (continuation)
                {
                    CreateRoad.Straight(points, roadProperties, continuation);
                    PreviewColliderScript colliderScript = previewRoad.GetComponent<PreviewColliderScript>();
                    GameObject roadContinuation = colliderScript.BuildContinuation(0.01f);
                    colliderScript.InsertProperties(roadProperties, roadContinuation);
                    colliderScript.DestroyContinuation();
                }
                else
                {
                    CreateRoad.Straight(points, roadProperties, continuation);
                    PreviewColliderScript colliderScript = previewRoad.GetComponent<PreviewColliderScript>();
                    colliderScript.DestroyContinuation();
                }
                continuation = true;

                Destroy(previewRoad);
                points = new List<Vector3>() { points[1] };
            }
            else if (shape == "bezier" && points.Count == 3)
            {
                CreateRoad.Bezier(points, roadProperties);
                Destroy(previewRoad);

                //previewRoad = CurvedPreview.CreateBezierContinuation(roadProperties, points);
                //StartCoroutine(CurvedPreview.UpdateBezierContinuation(previewRoad, roadProperties, points));
                points = new List<Vector3>() { points[2] };
            }

            // Starts the preview based on how many times the player clicked
            if (points.Count == 1)
            {
                // Straight
                previewRoad = StraightPreview.Create(roadProperties, points, continuation);
                StartCoroutine(StraightPreview.Update(previewRoad, roadProperties, points, continuation));
            }
            else if (points.Count == 2)
            {
                // Bezier
                previewRoad = CurvedPreview.CreateBezier(roadProperties, points);
                StartCoroutine(CurvedPreview.UpdateBezier(previewRoad, roadProperties, points));
            }
        }
        // When right click on any place that is not ui
        if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI())
        {
            continuation = false; //////////////////////////////////////////////////////////////////////////////////////
            PreviewColliderScript colliderScript = previewRoad.GetComponent<PreviewColliderScript>();
            colliderScript.DestroyContinuation();

            // If there's no more points, then just remove the road from the hands
            if (points.Count == 0)
            {
                // If there's no points then destroy the preview and clear hands
                Destroy(previewRoad);
                Hands.clearHands(GameObject.Find(roadName).GetComponent<GetRoad>());
            }
            else if (points.Count == 1)
            {
                // If there's one point and user right clicked, then destroy the straight preview and create a pre preview
                points.RemoveAt(points.Count - 1);
                Destroy(previewRoad);
                previewRoad = PrePreview.Create(roadProperties);
                StartCoroutine(PrePreview.Update(previewRoad, roadProperties));
            }
            else if (points.Count == 2)
            {
                // If there's two points and user right clicked, then destroy the curved preview and create a straigt preview
                points = new List<Vector3>() { points[0] };
                StopAllCoroutines();
                Destroy(previewRoad);
                previewRoad = StraightPreview.Create(roadProperties, points, continuation);
                StartCoroutine(StraightPreview.Update(previewRoad, roadProperties, points, continuation));
            }
            else if (points.Count > 2)
            {
                points.RemoveAt(points.Count - 1);
            }
        }
    }

    public void addPoint()
    {
        Vector3 raycastPosition = Raycasts.raycastPosition3D(camera);
        points.Add(new Vector3(Mathf.Round(raycastPosition.x), Mathf.Round(raycastPosition.y), Mathf.Round(raycastPosition.z)));
    }
}


public class RoadProperties : MonoBehaviour
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

    public Mesh mesh;
    public List<Vector3> points;

    public void ChangeProperties(RoadProperties roadProperties)
    {
        name = roadProperties.name;
        width = roadProperties.width;
        lanes = roadProperties.lanes;
        oneWay = roadProperties.oneWay;
        texture = roadProperties.texture;
        material = roadProperties.material;
        previewMaterial = roadProperties.previewMaterial;
        obstructedMaterial = roadProperties.obstructedMaterial;
        camera = roadProperties.camera;
        height = roadProperties.height;

        mesh = roadProperties.mesh;
        points = roadProperties.points;
    }
    private void OnValidate()
    {
        
    }
}