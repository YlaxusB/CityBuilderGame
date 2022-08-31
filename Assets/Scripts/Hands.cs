using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;


public class Hands : MonoBehaviour
{
    //Item itemHolding = new Item();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log(EventSystem.current.IsPointerOverGameObject());
        }
    }
}
