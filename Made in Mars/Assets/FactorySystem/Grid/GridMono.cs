using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A wrapper class for the Grid to interact with the world.
/// </summary>
public class GridMono : MonoBehaviour {
	public Grid myGrid;
	private void Awake () {
		myGrid = new Grid();
		if (Grid.s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		Grid.s = myGrid;
	}

	private void Start() {
		myGrid.WorldSetup(GetComponent<MapGenerator>());
	}

	private void OnDestroy() {
		Grid.s.OnDestroy();
	}
}