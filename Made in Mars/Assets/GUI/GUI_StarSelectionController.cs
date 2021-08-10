using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_StarSelectionController : MonoBehaviour {

    public static GUI_StarSelectionController s;

    private void Awake() {
        s = this;
    }

    private void OnDestroy() {
        s = null;
    }

    public void LandToPlanet(Planet planet) {
        Debug.Log($"Landing on planet: {planet.schematic.uniqueName} - {planet.oreDensities} - {planet.oreNames}");
        var saveFile = DataSaver.s.GetSave();
        saveFile.currentPlanet = new DataSaver.LocalPlanetData(
            new DataSaver.PlanetData(planet.planetGenerationInt, planet.schematic.uniqueName, planet.oreDensities, planet.oreNames, planet.myPresentCardNames)
            );
        SceneChangeMaster.s.LoadPlanetLevel();
    }
}
