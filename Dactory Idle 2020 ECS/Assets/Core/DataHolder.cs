using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds all the building data, item data, recipe data etc. needed
/// Mods and stuff will somehow add data here.
/// </summary>
public class DataHolder : MonoBehaviour {
    public static DataHolder s;
    [SerializeField]
    private BuildingData[] myBuildings;

    //Layers
    public static int worldLayer = 1;
    public static int beltLayer = 0;
    public static int itemLayer = -1;
    public static int buildingLayer = -2;

    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
    }

    public BuildingData GetBuilding (string uniqueName) {
        for (int i = 0; i < myBuildings.Length; i++) {
            if (myBuildings[i] != null) {
                if (myBuildings[i].uniqueName == uniqueName)
                    return myBuildings[i];
            }
        }
        throw new NullReferenceException("The building you are requesting " + uniqueName + " does not exist currently!");
    }
}
