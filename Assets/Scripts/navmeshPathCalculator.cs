using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// using Debug = System.Diagnostics.Debug;

public class navmeshPathCalculator : MonoBehaviour
{
    public GameObject targetPosition;
    private NavMeshPath path;

    private bool requestCame = true;
    // Start is called before the first frame update
    void Awake()
    {
        path = new NavMeshPath();
        Debug.Log(" target size:"+targetPosition.GetComponent<Collider>().bounds.size);
        Vector3 startPosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetPos = new Vector3(targetPosition.transform.position.x, 0.0f, targetPosition.transform.position.z -10);
        if (requestCame)
        {
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
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Listen to request
       
    }
}
