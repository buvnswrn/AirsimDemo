using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLister : MonoBehaviour
{
    // Start is called before the first frame update
    public string objectOfInterest;
    void Start()
    {
        List<GameObject> boxes = new List<GameObject>();
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
     foreach (var obj in allObjects)
     {
         if (obj.name == objectOfInterest)
         {
             boxes.Add((GameObject)obj);
         }
     }

     foreach (var box in boxes)
     {
         Debug.Log(box.transform.position);
     }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
