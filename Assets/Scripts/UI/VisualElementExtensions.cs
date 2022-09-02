using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static void BlockRaycasts(this VisualElement element) =>
        UIToolkitRaycastChecker.RegisterBlockingElement(element);

    public static void AllowRaycasts(this VisualElement element) =>
        UIToolkitRaycastChecker.UnregisterBlockingElement(element);

    public static void IsBlockingRaycasts(this VisualElement element) =>
        UIToolkitRaycastChecker.IsBlockingRaycasts(element);
}