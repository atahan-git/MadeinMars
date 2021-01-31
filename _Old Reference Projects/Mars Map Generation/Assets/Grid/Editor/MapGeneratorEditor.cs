using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI () {
        MapGenerator myScript = (MapGenerator)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map")) {
            myScript.GenerateMap();
        }

        if (GUILayout.Button("Generate Resources")) {
            myScript.GenerateResources();
        }
    }
}

