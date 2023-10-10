using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;

public class GetVisibleObjects : MonoBehaviour
{
    public GameObject droneObject;
    public string servicePrefix = "http://localhost:8099/";
    private static HTTPService _serviceRegistry;
    private readonly static Queue<Action> ExecuteInMain = new Queue<Action>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        _serviceRegistry = HTTPService.Initialize(new Uri(servicePrefix));
        Debug.Log("Creating HTTP Endpoint:getvisibleobjects");
        _serviceRegistry.RegisterService("getvisibleobjects/", GetVisibleObjectsServiceHandler);
    }

    private void GetVisibleObjectsServiceHandler(HttpListenerContext context)
    {
        string body = new StreamReader(context.Request.InputStream).ReadToEnd();
        Debug.Log("Received message:"+body);
        InterestedObject pos = JsonConvert.DeserializeObject<InterestedObject>(body);
        Debug.Log("Received JSON: ObjectOfInterest:"+pos.ObjectOfInterest);
        ExecuteInMain.Enqueue(()=>GetAllVisibleObjects(pos.ObjectOfInterest, context));
    }

    private void GetAllVisibleObjects(string objectOfInterest, HttpListenerContext context)
    {
        try
        {
            // TODO: Do the calculations to check if the object in front is a shelf or not. If so, return all object's position.
            RaycastHit hit;
            RaycastHit[] hits;

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Shelf");
            foreach (GameObject shelf in gameObjects)
            {
                if(shelf.name.Contains("Shelving"))
                {
                    Vector3 relativeVector = shelf.transform.position - droneObject.transform.position;
                    float angleToShelf = Vector3.Angle(relativeVector, droneObject.transform.forward);

                    if (angleToShelf >= -45 && angleToShelf <= 45)
                    {
                        Debug.LogWarning("Shelf in sight" + shelf.GetInstanceID() + ":" + angleToShelf);
                        GameObject rack = shelf.gameObject.transform.Find("Rack").gameObject;
                        // rack.GetComponent<Renderer>().material.color = Color.white;
                        if (WhetherRackIsVisible(rack, droneObject))
                        {
                            Debug.LogWarning("Shelf visible in camera" + shelf.GetInstanceID());
                        }
                    }
                }
            }
            
            if (Physics.Raycast(droneObject.transform.position,
                    droneObject.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Debug.DrawRay(droneObject.transform.position, droneObject.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow,10);
                Debug.Log("Hit" + hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawRay(droneObject.transform.position, droneObject.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Missed");
            }
            
            // Send the results back
            _serviceRegistry.SendResult(context, JsonConvert.SerializeObject(hit.collider.gameObject.transform.position, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
        catch (Exception e)
        {
            Debug.LogError("Error in GetVisibleObjects:"+e.Message);
        }
    }

    private bool WhetherRackIsVisible(GameObject rack, GameObject drone)
    {
        for (int i = 0; i < rack.transform.childCount; i++)
        {
            GameObject leg = rack.transform.GetChild(i).gameObject;
            Debug.DrawLine(rack.transform.position, leg.transform.position, Color.red, 100);
            if (RayCastToRack(drone, leg.transform.position, leg.gameObject.name))
            {
                return true;
            };
        }
        return false;
    }

    private bool RayCastToRack(GameObject drone, Vector3 position, string legName)
    {
        RaycastHit hit;
        Vector3 rayCastDir = position - drone.transform.position;
        // Gizmos.DrawSphere(position, 5);
        if (Physics.Raycast(drone.transform.position, rayCastDir, out hit))
        {
            Debug.DrawRay(drone.transform.position, rayCastDir.normalized * hit.distance, Color.yellow,100);
            if (hit.collider.gameObject.name == legName)
            {
                return true;
            }
        }
        else
        {
            Debug.DrawRay(drone.transform.position, rayCastDir * 1000, Color.red, 100);
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        while (ExecuteInMain.Count > 0)
        {
            ExecuteInMain.Dequeue().Invoke();
        }
    }
}

public class InterestedObject
{
    [JsonProperty("objectOfInterest")]
    public string ObjectOfInterest { get; set; }
}
