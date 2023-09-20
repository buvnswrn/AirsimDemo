using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
public class testPython : MonoBehaviour
{
    // Start is called before the first frame update
    bool takeOffFired = false;
    bool landingFired = false;
    private bool captureFired = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!takeOffFired && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Taking off...");
            PythonRunner.RunFile("Assets/Scripts/python/takeoff.py");
            resetFireVariables();
            takeOffFired = true;
        }
        else if (!landingFired && Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Landing...");
            PythonRunner.RunFile("Assets/Scripts/python/land.py");
            resetFireVariables();
            landingFired = true;
        }
        else if (!captureFired && Input.GetKeyDown(KeyCode.C))
        {
            resetFireVariables();
            captureFired = true;
            Debug.Log(("Capturing Image..."));
            PythonRunner.RunFile("Assets/Scripts/python/takeoff.py");
        }
    }

    private void resetFireVariables()
    {
        landingFired = false;
        takeOffFired = false;
        captureFired = false;
    }
}
