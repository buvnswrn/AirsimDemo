using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
public class testPython : MonoBehaviour
{
    // Start is called before the first frame update
    bool takeOffFired = false;
    bool landingFired = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!takeOffFired && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Taking off...");
            PythonRunner.RunFile("D:\\Bhuvan\\Projects\\temp\\pythonScripts\\takeoff.py");
            takeOffFired = true;
            landingFired = false;
        }
        else if (!landingFired && Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Landing...");
            landingFired = true;
            takeOffFired = false;
        }
    }
}
