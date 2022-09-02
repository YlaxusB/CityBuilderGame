using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MainUIScript : MonoBehaviour
{
    private void OnEnable()
    {

        VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement; // basically the body of a html page

        // Assign the buttons their functions
        BlockRaycasts();
        AssignButtonFunctions();
    }

    // Block raycasts through ui
    private void BlockRaycasts()
    {
        VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        VisualElement mainUI = rootVisualElement.Q<VisualElement>("MainUI");
        VisualElement customUI = rootVisualElement.Q<VisualElement>("CustomUI");
        UIToolkitRaycastChecker.RegisterBlockingElement(mainUI);
        UIToolkitRaycastChecker.RegisterBlockingElement(customUI);
    }

    /* -------- Assign functions to buttons -------- */
    private void AssignButtonFunctions()
    {
        VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement; // basically the body of a html page
        Button buttonRoads = rootVisualElement.Q<Button>("ButtonRoads");
        // Roads
        buttonRoads.CaptureMouse();
        buttonRoads.RegisterCallback<ClickEvent>(Event => OpenCustomUI());
        buttonRoads.RegisterCallback<ClickEvent>(Event => InsertRoadsIntoCustomUI());
    }

    // If the ui is already open and a user click on the button it closes
    private void CloseCustomUI()
    {
        VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        VisualElement customUI = rootVisualElement.Q<VisualElement>("CustomUI");
        ScrollView scrollView = rootVisualElement.Q<ScrollView>("CustomList");
        VisualElement[] contents = scrollView.Children().ToArray();

        foreach (VisualElement content in contents)
        {
            scrollView.Remove(content);
        }
        customUI.visible = false;
    }

    // Changes the visibility of Custom UI (The UI above Main UI)
    private void OpenCustomUI()
    {
        VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        ScrollView scrollView = rootVisualElement.Q<ScrollView>("CustomList");
        if (rootVisualElement.Q<VisualElement>("CustomUI").visible)
        {
            CloseCustomUI();
        }
        else
        {
            scrollView.horizontalScroller.StretchToParentWidth();
            scrollView.horizontalScroller.style.height = new StyleLength(5);
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            rootVisualElement.Q<VisualElement>("CustomUI").visible = true;
        }
    }

    // Get all roads from the game and display in the ui
    private void InsertRoadsIntoCustomUI()
    {
        VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        if (rootVisualElement.Q<VisualElement>("CustomUI").visible)
        {
            // Get UI Elements

            ScrollView customList = rootVisualElement.Q<ScrollView>("CustomList");

            // Get all the roads in RoadsContents "folder" (gameobject serving like a folder)
            List<Transform> roads = new List<Transform>();
            for (int i = 0; i < GameObject.Find("RoadsContents").transform.childCount; i++)
            {
                roads.Add(GameObject.Find("RoadsContents").transform.GetChild(i));
            }

            // Create the image with road texture, then add a function to click on image and get that road to build
            foreach (Transform road in roads)
            {
                // The properties of a road are : road width, lanes, oneway, texture
                Roads roadProperties = road.GetComponent<Roads>();

                ScrollView scrollView = rootVisualElement.Q<ScrollView>("CustomList");
                Image img = new Image();
                img.image = roadProperties.roadTexture;
                img.style.width = new StyleLength(80);
                img.style.height = new StyleLength(80);
                img.style.paddingRight = new StyleLength(10);
                img.RegisterCallback<ClickEvent>(Event =>
                {
                    if(GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand != "")
                    {
                        Debug.Log("Já havia uma construção, era : " + GameObject.Find("Main Camera").GetComponent<Hands>().buildingOnHand);
                        Hands.clearHands(GameObject.Find("Main Camera").GetComponent<Hands>());
                        startRoad(roadProperties.roadWidth, ((int)roadProperties.roadLanes),
                        roadProperties.oneWay, roadProperties.roadTexture, roadProperties.roadMaterial, roadProperties.roadPreviewMaterial, roadProperties.roadObstructedMaterial, road.name, road);
                    } else
                    {
                        Debug.Log("Não havia nenhuma construção");
                        startRoad(roadProperties.roadWidth, ((int)roadProperties.roadLanes),
                        roadProperties.oneWay, roadProperties.roadTexture, roadProperties.roadMaterial, roadProperties.roadPreviewMaterial, roadProperties.roadObstructedMaterial, road.name, road);
                    }
                });
               // img.RegisterCallback<ClickEvent>(Event => startRoad(roadProperties.roadWidth, ((int)roadProperties.roadLanes),
               //     roadProperties.oneWay, roadProperties.roadTexture, roadProperties.roadMaterial, road.name, road));
                scrollView.Add(img);
            }
        }
    }

    private void startRoad(float roadWidth, int roadLanes, bool oneWay, Texture roadTexture, Material roadMaterial, Material roadPreviewMaterial, Material roadObstructedMaterial, string roadName, Transform contentRoad)
    {
        Hands handsOnMainCamera = GameObject.Find("Main Camera").GetComponent<Hands>();
        if (handsOnMainCamera.buildingOnHand == "")
        {
            GetRoad roadComponent = contentRoad.gameObject.AddComponent<GetRoad>();
            roadComponent.roadWidth = roadWidth;
            roadComponent.roadLanes = roadLanes;
            roadComponent.oneWay = oneWay;
            roadComponent.roadTexture = roadTexture;
            roadComponent.roadMaterial = roadMaterial;
            roadComponent.roadName = roadName;
            roadComponent.roadPreviewMaterial = roadPreviewMaterial;
            roadComponent.roadObstructedMaterial = roadObstructedMaterial;
        }
    }
}
