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
using Logger = NLog.Logger;
using Newtonsoft.Json.Linq;
using UnityEngine.AI;

public class NavMeshPathService : MonoBehaviour
{
    private static HTTPService serviceRegistry;
    public string servicePrefix = "http://localhost:8099/";
    private NavMeshPath path;
    private readonly static Queue<Action> executeInMain = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
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
        Debug.Log("Creating HTTP Endpoint:navmeshpath");
        serviceRegistry.RegisterService("navmeshpath/", NavMeshServiceHandler);
    }



    private void NavMeshServiceHandler(HttpListenerContext context)
    {
        string body = new StreamReader(context.Request.InputStream).ReadToEnd();
        Debug.Log("Received message:"+body);
        position pos = JsonConvert.DeserializeObject<position>(body);
        Debug.Log("Received JSON:"+pos.start_position.ToString());
        executeInMain.Enqueue(() => getPath(pos.start_position, pos.end_position, context));
    }



    private Vector3 getPosition(string position_json)
    {
        position_json = position_json.Replace("(", "");
        position_json = position_json.Replace(")", "");
        string[] position_string_array = position_json.Split(",");
        if (position_string_array.Length == 3)
        {
            return new Vector3(float.Parse(position_string_array[0]), float.Parse(position_string_array[1]), float.Parse(position_string_array[2]));
        }
        else
        {
            throw new Exception("JSON provided is not Vector3");
        }
    }

    private void getPath(Vector3 start, Vector3 end, HttpListenerContext context)
    {
        try
        { 
        // Debug.Log(" target size:"+targetPosition.GetComponent<Collider>().bounds.size);
        Vector3 startPosition = new Vector3(start.x, 0.0f, start.z);
        Vector3 targetPos = new Vector3(end.x, 0.0f, end.z -10);
        NavMesh.CalculatePath(startPosition, targetPos, NavMesh.AllAreas, path);
        if(path.status == NavMeshPathStatus.PathComplete || path.status == NavMeshPathStatus.PathPartial)
        {
            Debug.Log("Got total points:"+path.corners.Length.ToString());
            Debug.Log("Got Path:"+path.corners.ToString());
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.Log("Visualizing Line from:"+path.corners[i].ToString()+" to "+path.corners[i + 1].ToString());
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red,100);
            }
        }
        else
        {
            Debug.Log("Path not found from "+startPosition.ToString()+" to "+targetPos.ToString());
        }
        serviceRegistry.SendResult(context, JsonConvert.SerializeObject(path.corners, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
        catch (Exception e)
        {
            Debug.LogError("Error in Navmesh"+e.Message);
        }
    }


}

public class position
{
    [JsonProperty("start_position")]
    public Vector3 start_position { get; set;}
    [JsonProperty("end_position")]
    public Vector3 end_position { get; set;}
}