using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFacingDirection : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 dir;
    void Start()
    {
        // dir = transform.TransformDirection(transform.forward);
        dir = transform.forward;
        Debug.Log(dir);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.forward;
        if (dir!=forward)
        {
            Debug.Log("Change in direction:"+ forward);
            dir = forward;
        }
    }
}
