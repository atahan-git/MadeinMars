using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class GUI_StarsCanvasMaker : MonoBehaviour {
    public GameObject StarsParent;

    public GameObject[] starOptions = new GameObject[3];


    private void Start() {
        for (int i = 0; i < starOptions.Length; i++) {
            starOptions[i].GetComponent<MiniGUI_PlanetButton>().SetUp(MakeRandomPlanet());
        }
    }


    public Planet MakeRandomPlanet() {
        int seed = Random.Range(0, 1000);
        var myPlanet = new Planet(seed);
        
        Random.InitState(seed);
        
        var oreCount = Random.Range(0, DataHolder.s.allOreDensities.Length);

        var ores = new List<OreDensityLevelsHolder>(DataHolder.s.allOreDensities);

        myPlanet.oreNames = new string[oreCount];
        myPlanet.oreDensities = new int[oreCount];
        for(int i=0; i<oreCount; i++){
            int index = Random.Range(0, ores.Count);

            var ore = ores[i];
            var myDensity = Random.Range(0, ore.oreLevels.Length);

            myPlanet.oreNames[i] = ore.GetUniqueName();
            myPlanet.oreDensities[i] = myDensity;
            
            ores.RemoveAt(index);
        }

        var planetType = Random.Range(0, DataHolder.s.allPlanetSchematics.Length);

        myPlanet.schematic = DataHolder.s.allPlanetSchematics[planetType];

        return myPlanet;
    }
}
