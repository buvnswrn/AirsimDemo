using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor.Build.Content;
using UnityEngine;

namespace RoboticWarehouse
{
    public class ItemLister : MonoBehaviour
    {
        // Start is called before the first frame update
        public string objectOfInterest;

        private List<GameObject> boxes;
        private List<String> locations;
        private List<String> non_fluents_tasks;
        private List<String> tasks;
        private List<String> move_cost;
        private GameObject launchpad;

        private Dictionary<String, Dictionary<String, double>> distanceHeuristics =
            new Dictionary<String, Dictionary<String, double>>();

        void Start()
        {
            boxes = new List<GameObject>();
            locations = new List<String>();
            non_fluents_tasks = new List<String>();
            tasks = new List<String>();
            move_cost = new List<String>();
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name == objectOfInterest)
                {
                    boxes.Add((GameObject)obj.transform.parent.transform.parent.transform.parent.gameObject);
                    String shelf_id = "shelf_" + ((GameObject)obj).transform.parent.transform.parent.transform.parent
                        .gameObject
                        .GetInstanceID().ToString();
                    locations.Add(shelf_id);
                    // TASK(obj, capture, shelf_id);
                    String box_id = "box_" + obj.GetInstanceID();
                    // tasks.Add(box_id);
                    tasks.Add(shelf_id);
                    String taskString = "TASK( " + box_id + "," + "capture_image," + shelf_id + ");";
                    non_fluents_tasks.Add(taskString);
                }
                else if (obj.name == "launchpad")
                {
                    String launchpad_id = "launchpad_" + obj.GetInstanceID();
                    tasks.Add(launchpad_id);
                    launchpad = obj.gameObject;
                }
            }

            locations = locations.Distinct().ToList();
            non_fluents_tasks = non_fluents_tasks.Distinct().ToList();

            // foreach (var box in non_fluents_tasks)
            // {
            //     Debug.Log(box);
            // }

            CalculateHeuristics();
            ExtractMoveToCost();
            move_cost = move_cost.Distinct().ToList();

            // foreach (var box in move_cost)
            // {
            //     Debug.Log(box);
            // }

            WriteToFiles();
        }

        private void WriteToFiles()
        {
            string move_cost_string = String.Join("; \n", move_cost);
            File.WriteAllLines("move_cost.txt", move_cost);
            string non_fluents_tasks_string = String.Join("; \n", non_fluents_tasks);
            File.WriteAllLines("non_fluents_tasks.txt", non_fluents_tasks);
            string locations_string = String.Join("; \n", locations);
            File.WriteAllLines("locations.txt", locations);
            string tasks_string = String.Join("; \n", tasks);
            File.WriteAllLines("tasks.txt", tasks);

        }

        private void ExtractMoveToCost()
        {
            foreach (String box1 in distanceHeuristics.Keys)
            {
                foreach (String box2 in distanceHeuristics[box1].Keys)
                {
                    String moveToCost = "MOVE_COST( " + box1 + " , " + box2 + " ) " + " = " +
                                        distanceHeuristics[box1][box2].ToString() +
                                        ";";
                    move_cost.Add(moveToCost);
                }
            }

        }

        private void CalculateHeuristics()
        {
            foreach (GameObject box1 in boxes)
            {
                Dictionary<String, double> heuristicForThisBox = new Dictionary<String, double>();
                double launchpad_distance =
                    getEuclideanDistance(box1.transform.position, this.launchpad.transform.position);
                heuristicForThisBox.Add("launchpad_" + this.launchpad.GetInstanceID().ToString(), launchpad_distance);
                // Debug.Log("launchpad_"+this.launchpad.GetInstanceID().ToString()+"="+launchpad_distance);
                // String box1_id = "box_" + box1.GetInstanceID().ToString();
                String box1_id = "shelf_" + box1.GetInstanceID().ToString();
                foreach (GameObject box2 in boxes)
                {
                    if (box1.GetInstanceID() != box2.GetInstanceID())
                    {
                        double distance = getEuclideanDistance(box1.transform.position, box2.transform.position);
                        // String box2_id = "box_" + box2.GetInstanceID().ToString();
                        String box2_id = "shelf_" + box2.GetInstanceID().ToString();
                        // Debug.Log(String.Format("Distance from box1({0}) - box2({1}) = {2}", box1.GetInstanceID(),
                        //     box2.GetInstanceID(), distance));
                        if (!heuristicForThisBox.ContainsKey(box2_id))
                            heuristicForThisBox.Add(box2_id, distance);
                        // String moveto_cost_string = "MOVE_COST( " + "box_" + box1.GetInstanceID() + "," 
                        //                           + " box_" + box2.GetInstanceID() + ")"
                        //                           + " = " + distance.ToString() + ";";
                        // move_cost.Add(moveto_cost_string);
                        // Debug.Log(moveto_cost_string);
                    }
                }

                if (!distanceHeuristics.ContainsKey(box1_id))
                    distanceHeuristics.Add(box1_id, heuristicForThisBox);
            }
        }

        public double getEuclideanDistance(Vector3 p1, Vector3 p2)
        {
            float dX = p2.x - p1.x;
            float dY = p2.y - p1.y;
            float dZ = p2.z - p1.z;
            return Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2) + Math.Pow(dZ, 2));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
