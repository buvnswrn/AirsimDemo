using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SyncObjects))]
public class SyncToRepository : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SyncObjects call = (SyncObjects)target;
        if(GUILayout.Button("Sync to Repository")) {
            call.SyncAll();
        }
    }
}
