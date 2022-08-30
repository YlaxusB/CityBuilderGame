using UnityEngine;
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
    }
}