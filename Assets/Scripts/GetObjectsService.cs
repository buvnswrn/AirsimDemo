using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Policy;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.AI;

public class GetObjectsService : MonoBehaviour
{
    public string objectOfInterest;
    private static HTTPService serviceRegistry;

    public string servicePrefix = "http://localhost:8099/";

    private readonly static Queue<Action> executeInMain = new Queue<Action>();

    private List<GameObjectOfInterest> listOfGameObjectOfInterest = new List<GameObjectOfInterest>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        while (executeInMain.Count > 0)
        {
            executeInMain.Dequeue().Invoke();
        }
    }

    void OnEnable()
    {
        serviceRegistry = HTTPService.Initialize(new Uri(servicePrefix));
        Debug.Log("Creating HTTP Endpoint:getObjects");
        serviceRegistry.RegisterService("getObjects/", GetObjectsServiceHandler);
    }

    private void GetObjectsServiceHandler(HttpListenerContext context)
    {
        executeInMain.Enqueue(()=> getObjects(objectOfInterest, context));
    }

    private void getObjects(string name, HttpListenerContext context)
    {
        Debug.Log("Getting the objects of Interest:"+name);
        var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var gameObject in allGameObjects)
        {
            if (gameObject.name == objectOfInterest)
            {
                string objectName;
                if (objectOfInterest.Contains("box"))
                {
                    objectName = "box_" + gameObject.GetInstanceID();
                    GameObjectOfInterest gameObjectOfInterest = new GameObjectOfInterest();
                    gameObjectOfInterest.name = objectName;
                    gameObjectOfInterest.position = gameObject.transform.position;
                    listOfGameObjectOfInterest.Add(gameObjectOfInterest);
                }
            }
        }
        serviceRegistry.SendResult(context, JsonConvert.SerializeObject(listOfGameObjectOfInterest, Formatting.Indented, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }));
    }
    
}

public class GameObjectOfInterest
{
    [JsonProperty("position")] public Vector3 position { get; set; }
    [JsonProperty("name")] public string name { get; set; }
}
