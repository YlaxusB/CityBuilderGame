using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CustomHelper
{
    public static class Raycasts
    {
        // Raycast and return x being x, and y being z
        public static Vector2 raycastPosition2D(Camera camera)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                return new Vector2(hit.point.x, hit.point.z);
            }
            else
            {
                return new Vector2(0, 0);
            }
        }

        // Raycast and return the hitted position
        public static Vector3 raycastPosition3D(Camera camera)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                return new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// Raycast only in the specified layer
        /// </summary>
        public static Vector3 raycastLayer(Camera camera, string layerName)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask(layerName);

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                Transform objectHit = hit.transform;
                return new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }

        // Check if the mouse is not over a UI
        /*
        public static bool isMouseOverUI()
        {
            GraphicRaycaster m_Raycaster;
            PointerEventData m_PointerEventData;
            EventSystem m_EventSystem;

            //Fetch the Raycaster from the GameObject (the Canvas)
            m_Raycaster = GetComponent<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            m_EventSystem = GetComponent<EventSystem>();
        }*/

        // Check if mouse is over terrain
        public static bool isMouseOverLayer(Camera camera, string layerName)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask(layerName);

            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask(layerName)))
            {
                if (LayerMask.LayerToName(hit.collider.gameObject.layer) == layerName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Return the hitted object when mouse over layer
        public static RaycastHit isMouseCollidingLayer(Camera camera, string layerName)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask(layerName);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (LayerMask.LayerToName(hit.collider.gameObject.layer) == layerName)
                {
                    return hit;
                }
                else
                {
                    return new RaycastHit();
                }
            }
            else
            {
                return new RaycastHit();
            }
        }
    }
}
