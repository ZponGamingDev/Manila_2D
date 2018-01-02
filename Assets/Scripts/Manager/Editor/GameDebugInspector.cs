using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameDebugManager))]
public class GameDebugInspector : Editor 
{
    GameDebugManager m;
    void HandleCapFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
    }

    void OnSceneGUI()
    {
        m = target as GameDebugManager;
        //Handles.BeginGUI();

        Handles.Button(Vector3.zero, Quaternion.identity, 10.0f, 10.0f, HandleCapFunction);
       // Handles.EndGUI();
    }
}
