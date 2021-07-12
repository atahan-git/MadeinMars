using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI () {
        MapGenerator myScript = (MapGenerator)target;
        if (GUILayout.Button("Generate Map")) {
            myScript.Clear();
            myScript.GenerateMap(myScript.editorMapSchematic);
        }

        if (GUILayout.Button("Clear")) {
            myScript.Clear();
        }
        
        if (GUILayout.Button("Show Height")) {
            myScript.ShowHeightDebug();
        }
        
        if (GUILayout.Button("Generate Resources")) {
            myScript.GenerateResources(myScript.debugOreSpawnSettings);
        }

        DrawDefaultInspector();

    }
}

