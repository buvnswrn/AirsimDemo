using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
public class AirsimExecutor
{
    public void takeOff()
    {
        PythonRunner.RunFile("Assets/Scripts/python/takeoff.py");
    }

    public void land()
    {
        PythonRunner.RunFile("Assets/Scripts/python/land.py");
    }

    public void moveToPosition(Vector3 position)
    {
        PythonRunner.RunFile("Assets/Scripts/python/moveToPosition.py");
    }
}
