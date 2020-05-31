using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

[CustomEditor(typeof(TileBaseScript))]
public class TileEditor : Editor {

	TileBaseScript tile;


	void OnEnable(){
		tile = (TileBaseScript)target;
	}

	public override void OnInspectorGUI(){
		//base.OnInspectorGUI();
		DrawDefaultInspector ();


		if (GUILayout.Button ("Update Location")) {
			tile.UpdateLocation ();
		}
	}
}
