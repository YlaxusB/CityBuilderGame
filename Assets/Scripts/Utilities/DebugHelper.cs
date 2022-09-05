using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomDebugger
{
    public class Debugger
    {
        public static GameObject Primitive(PrimitiveType primitiveType, string name, Vector3 position, Quaternion rotation)
        {
            GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.name = name;
            return gameObject;
        }
    }
}