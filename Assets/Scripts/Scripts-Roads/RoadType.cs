using UnityEngine;
public struct Road
{
    public float roadWidth;
    public float roadLanes;
    public bool oneWay;
    public Texture roadTexture;
    public Material roadMaterial;

    public Road(float roadWidth, float roadLanes, bool oneWay, Texture roadTexture, Material roadMaterial)
    {
        this.roadWidth = roadWidth;
        this.roadLanes = roadLanes;
        this.oneWay = oneWay;
        this.roadTexture = roadTexture;
        this.roadMaterial = roadMaterial;
    }
}