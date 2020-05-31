using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

	Grid grid;

	void OnEnable(){
		grid = (Grid)target;
	}

	public override void OnInspectorGUI(){
		//base.OnInspectorGUI();
		DrawDefaultInspector ();


		if (GUILayout.Button ("Update Tile Size")) {
			grid.UpdateTileSize ();
		}

		/*if (GUILayout.Button ("Reset Grid")) {
			grid.Awake ();
			grid.DrawTiles ();
		}*/
	}

	/*void OnSceneGUI(){
		//Debug.Log (Event.current.mousePosition);
		Event e = Event.current;
		if (e.type == EventType.mouseDown && e.alt && !e.control/*Event.current.type == EventType.mouseDown && Event.current.keyCode == (KeyCode.LeftAlt)) {

			if (Camera.current != null) {
				Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector2(Event.current.mousePosition.x , SceneView.lastActiveSceneView.camera.pixelHeight - Event.current.mousePosition.y)/*Input.mousePosition);

				RaycastHit hit = new RaycastHit();
				Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000);

				if (Physics.Raycast(ray, out hit)) {
					//Debug.Log ("hit");
					grid.ClickTile (hit.collider.gameObject);
				} else {
					//Debug.Log("miss");
				}
			} else {
				//Debug.Log("camera is null");
			}


		}
	}*/
}
