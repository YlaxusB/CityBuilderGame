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

    // the road wich mouse is over
    private GameObject roadInMouse = null;
    // the position mouse should be to exactly fit an already existing road
    public Vector3 suggestedEnd = Vector3.zero;

    bool buildingAfterJunction = false;
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
        StartCoroutine(PrePreview.Update(previewRoad, roadProperties, suggestedEnd, gameObject.GetComponent<GetRoad>()));
    }

    int i = 0;
    private bool continuation = false;
    private void Update()
    {
        HandleT();
        // When left click on any place that is not ui
        if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() && Raycasts.isMouseOverLayer(camera, "Terrain") && !IsMouseCloseToRoad() && !updatingAfterJunction)
        {
            Debug.Log("Cl");
            Destroy(previewRoad);
            StopAllCoroutines();
            addPoint();

            // if the player selected the straight shape and there are 2 points, then remove the old preview and
            // create the final road
            if (shape == "straight" && points.Count == 2)
            {
                buildingAfterJunction = false;
                // Check if it's a continuation
                if (continuation)
                {
                    Debug.Log("1");
                    PreviewColliderScript colliderScript = previewRoad.GetComponent<PreviewColliderScript>();
                    GameObject roadContinuation = colliderScript.BuildContinuation(0.01f);
                    CreateRoad.Straight(points, Raycasts.raycastPosition3D(roadProperties.camera), roadProperties, true, roadContinuation);
                    //colliderScript.InsertProperties(roadProperties, roadContinuation);
                    colliderScript.DestroyContinuation();
                }
                else
                {
                    Debug.Log("2");
                    CreateRoad.Straight(points, Raycasts.raycastPosition3D(roadProperties.camera), roadProperties, true, null);
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
                buildingAfterJunction = false;
                previewRoad = StraightPreview.Create(roadProperties, points, true);
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
        if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI() && !updatingAfterJunction)
        {
            buildingAfterJunction = false;
            //continuation = false; //////////////////////////////////////////////////////////////////////////////////////
            PreviewColliderScript colliderScript = previewRoad.GetComponent<PreviewColliderScript>();
            if (continuation)
            {
                colliderScript.DestroyContinuation();
            }

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
                Destroy(GameObject.Find("Preview Junction"));
                StartCoroutine(PrePreview.Update(previewRoad, roadProperties, Vector3.positiveInfinity, gameObject.GetComponent<GetRoad>()));
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
            continuation = false;
        }

        // If is building after junction and clicked right click
        if (Input.GetButtonDown("Fire2") && !UIToolkitRaycastChecker.IsPointerOverUI() && updatingAfterJunction)
        {
            Debug.Log("Right Clicked");
            points = new List<Vector3>();
            oldRoad.GetComponent<MeshRenderer>().enabled = true;
            buildingAfterJunction = false;
            updatingAfterJunction = false;
            continuation = false;
            Destroy(GameObject.Find("Preview Junction"));
            Destroy(GameObject.Find("Straight Preview Road"));
            // Restart the PrePreview
            previewRoad = PrePreview.Create(roadProperties);
            StartCoroutine(PrePreview.Update(previewRoad, roadProperties, suggestedEnd, gameObject.GetComponent<GetRoad>()));
        }
    }

    bool updatingAfterJunction = false;
    GameObject oldRoad;
    public void HandleT()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        if (hit.transform.gameObject != null)
        {
            GameObject referenceRoad = hit.transform.gameObject;

            Vector3 hittedPosition = hit.point;
            if (IsMouseCloseToRoad())
            {

                suggestedEnd = referenceRoad.transform.position + referenceRoad.transform.TransformDirection(Vector3.Distance(referenceRoad.transform.position, hittedPosition), 0, 0);
                // Clicked on already existing road
                if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() && Raycasts.isMouseOverLayer(camera, "Terrain") && !updatingAfterJunction)
                {
                    Debug.Log("Clicked here");
                    oldRoad = referenceRoad;
                    updatingAfterJunction = true;
                    if (points.Count == 0) // Checks if it's the first click, if yes then removes pre preview
                    {
                        Destroy(previewRoad);
                    }
                    RoadProperties referenceProperties = referenceRoad.GetComponent<RoadProperties>();

                    // Create first road to end right before junction
                    CreateRoad.Straight(new List<Vector3>() { referenceRoad.transform.position },
                        suggestedEnd + referenceRoad.transform.TransformDirection(-referenceProperties.width, 0, 0),
                        roadProperties, false, null);

                    // Create second road to start after junction
                    CreateRoad.Straight(new List<Vector3>() { suggestedEnd + referenceRoad.transform.TransformDirection(referenceProperties.width, 0, 0) },
                        referenceRoad.transform.position + referenceRoad.transform.TransformDirection(referenceProperties.points[referenceProperties.points.Count - 1].x, 0, 0),
                        roadProperties, false, null);

                    // Create the T-GameObject
                    JunctionProperties tProperties = new JunctionProperties();
                    tProperties.width = referenceProperties.width;
                    tProperties.material = referenceProperties.material;
                    tProperties.texture = referenceProperties.texture;
                    GameObject tJunction = Junctions.T(suggestedEnd +
                        referenceRoad.transform.TransformDirection(-referenceProperties.width, 0, 0),
                        suggestedEnd + referenceRoad.transform.TransformDirection(referenceProperties.width, 0, 0),
                        tProperties);

                    // Start the preview of the third road
                    Vector3 mouseRelativeToRoad = referenceRoad.transform.InverseTransformPoint(Raycasts.raycastPosition3D(roadProperties.camera));
                    float pivotRotation;
                    Vector3 pivotPosition;
                    if (mouseRelativeToRoad.z <= 0) // Right
                    {
                        pivotRotation = 90;
                        pivotPosition = tJunction.transform.position +
                        tJunction.transform.TransformDirection(tProperties.width, 0, -tProperties.width);
                    }
                    else // Left
                    {
                        pivotRotation = -90;
                        pivotPosition = tJunction.transform.position +
                        tJunction.transform.TransformDirection(tProperties.width, 0, tProperties.width);
                    }
                    // Create continuation
                    GameObject junctionObject;
                    GameObject junctionPivot;
                    ContinuationPreview.CreateContinuationPreview(tJunction, pivotPosition, pivotRotation,
                        out junctionObject, out junctionPivot);

                    // Create the straight road preview
                    GameObject straightPreview = StraightPreview.Create(referenceProperties, new List<Vector3>() { pivotPosition }, false);
                    //StartCoroutine(StraightPreview.Update(referenceRoad, referenceProperties, new List<Vector3>() { pivotPosition }, true));

                    StartCoroutine(UpdateContinuationPreview(referenceRoad, junctionObject, junctionPivot, straightPreview));
                    oldRoad.GetComponent<MeshRenderer>().enabled = false;
                    // Destroy the old road without junction
                    //Destroy(referenceRoad);
                    //points.Add(junctionPivot.transform.position);
                }
            }
            else // Mouse not over Road
            {
                suggestedEnd = Vector3.zero;
                roadInMouse = null;
            }
        }

    }

    public IEnumerator UpdateContinuationPreview(
        GameObject referenceRoad,
        GameObject junctionObject,
        GameObject junctionPivot,
        GameObject straightPreview
        )
    {
        RoadProperties referenceProperties = referenceRoad.GetComponent<RoadProperties>();
        while (true)
        {
            Vector3 mouseRelativeToPivot = junctionPivot.transform.InverseTransformPoint(Raycasts.raycastPosition3D(referenceProperties.camera));
            Vector3 arcCenter = new Vector3();
            if (mouseRelativeToPivot.z <= 0) // Right
            {
                arcCenter = junctionPivot.transform.position + junctionPivot.transform.TransformDirection(0, 0, -referenceProperties.width);
            }
            else
            {
                arcCenter = junctionPivot.transform.position + junctionPivot.transform.TransformDirection(0, 0, referenceProperties.width);
            }
            ContinuationPreview.UpdateContinuationPreview(
                arcCenter,
                junctionObject,
                referenceProperties,
                straightPreview,
                junctionPivot.transform.position,
                junctionPivot.transform.position,
                junctionPivot
                );

            // build final road
            if (Input.GetButtonDown("Fire1") && !UIToolkitRaycastChecker.IsPointerOverUI() &&
                Raycasts.isMouseOverLayer(camera, "Terrain") && !IsMouseCloseToRoad() && updatingAfterJunction)
            {
                //previewRoad.transform.position = junctionPivot.transform.position + junctionPivot.transform.TransformDirection(Vector3.Distance(junctionPivot.transform.position, Raycasts.raycastPosition3D(camera)), 0, 0);
                GameObject finalRoad = ContinuationPreview.CreateContinuationAndRoad(
                    arcCenter,
                    referenceProperties,
                    straightPreview,
                    junctionPivot.transform.position,
                    junctionPivot.transform.position,
                    junctionPivot
                    );
                Debug.Log(finalRoad.GetComponent<RoadProperties>().width);

                points = new List<Vector3>();
                //oldRoad.GetComponent<MeshRenderer>().enabled = false;

                buildingAfterJunction = false;
                updatingAfterJunction = false;
                continuation = true;
                Debug.Log(referenceProperties.width);

                DestroyImmediate(GameObject.Find("Preview Junction"));
                //Destroy(GameObject.Find(""));
                //Destroy(GameObject.Find("Straight Preview Road"));
                previewRoad = GameObject.Find("Straight Preview Road");
                //previewRoad = StraightPreview.Create(roadProperties, points, true);
                previewRoad.AddComponent<PreviewColliderScript>();
                previewRoad.GetComponent<PreviewColliderScript>().firstRoad = finalRoad;
                StartCoroutine(StraightPreview.Update(previewRoad, referenceProperties, new List<Vector3>() { Raycasts.raycastPosition3D(camera) }, continuation));
                addPoint();
                Destroy(oldRoad);
                break;
            }
            yield return null;
        }

    }

    /*
    void HandleContinuation()
    {

        IsMouseCloseToRoad(referenceRoad.transform);
    }
    */

    // Check if mouse is over or next to another existing road
    bool IsMouseCloseToRoad()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        GameObject hittedObject = hit.transform.gameObject;
        if (hittedObject.transform.name == "Road")
        {
            return true;
        }
        else
        {
            return false;
        }
        // return true;
    }

    // Move mouse pointing to fit another existing road, this will help visualization to build new roads
    void MovePreviewToRoad(Transform referenceTransform)
    {

    }

    public void addPoint()
    {
        Vector3 raycastPosition = Raycasts.raycastPosition3D(camera);
        points.Add(new Vector3(Mathf.Round(raycastPosition.x), 0.2f, Mathf.Round(raycastPosition.z)));
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