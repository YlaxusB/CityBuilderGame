using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roads : MonoBehaviour
{
    [Range(0, 50)] 
    public float roadWidth = 1;
    [Range(0, 12)]
    public float roadLanes = 1;

    public bool oneWay = false;

    public Texture roadTexture;
    public Material roadMaterial;
}
