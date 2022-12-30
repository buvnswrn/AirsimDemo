using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLister : MonoBehaviour
{
    // Start is called before the first frame update
    public string objectOfInterest;

    private List<GameObject> boxes;
    private Dictionary<int, Dictionary<int, double>> distanceHeuristics = new Dictionary<int, Dictionary<int, double>>();
    void Start()
    {
         boxes = new List<GameObject>();
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
     foreach (var obj in allObjects)
     {
         if (obj.name == objectOfInterest)
         {
             boxes.Add((GameObject)obj);
         }
     }

     // foreach (var box in boxes)
     // {
     //     Debug.Log(box.transform.position);
     // }
     CalculateHeuristics();
    }

    private void CalculateHeuristics()
    {
        foreach(GameObject box1 in boxes)
        {
            Dictionary<int, double> heuristicForThisBox = new Dictionary<int, double>();
            foreach (GameObject box2 in boxes)
            {
                if (box1.GetInstanceID() != box2.GetInstanceID())
                {
                    double distance = getEuclideanDistance(box1.transform.position, box2.transform.position);
                    Debug.Log(String.Format("Distance from box1({0}) - box2({1}) = {2}", box1.GetInstanceID(),
                        box2.GetInstanceID(), distance));
                    heuristicForThisBox.Add(box2.GetInstanceID(), distance);
                }  
            }
            distanceHeuristics.Add(box1.GetInstanceID(),heuristicForThisBox);
        }
    }

    public double getEuclideanDistance(Vector3 p1, Vector3 p2)
    {
        float dX = p2.x - p1.x;
        float dY = p2.y - p1.y;
        float dZ = p2.z - p1.z;
        return Math.Sqrt(Math.Pow(dX, 2)+ Math.Pow(dY, 2) + Math.Pow(dZ, 2));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
