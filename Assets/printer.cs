using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboticWarehouse
{
    public class printer : MonoBehaviour
    {
        public string objectOfInterest;
        // Start is called before the first frame update
        void Start()
        {
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name == objectOfInterest)
                {
                    Debug.LogWarning("Shelf Located At:"+obj.transform.position.ToString());
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }   
}
