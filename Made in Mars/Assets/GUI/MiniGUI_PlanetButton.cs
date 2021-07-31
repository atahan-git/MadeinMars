using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniGUI_PlanetButton : MonoBehaviour {
	public Planet myPlanet;

	public GameObject infoDisplay;

	public Text planetName;
	public Image planetImage;
	public Text planetDesc;

	public void SetUp(Planet planet) {
		myPlanet = planet;
		planetName.text = $"Planet #{planet.planetGenerationInt}";
		planetImage.sprite = planet.schematic.previewImage;

		var oreText = "Has ores:\n";

		for (int i = 0; i < planet.oreDensities.Length; i++) {
			oreText += $"{planet.oreDensities[i]+1} x {DataHolder.s.GetItem(planet.oreNames[i]).name} \n";
		}

		planetDesc.text = oreText;
	}
	
	public void ToggleInfo() {
		
		infoDisplay.SetActive(!infoDisplay.activeSelf);
	}
	
	public void OpenInfoDisplay() {
		
		infoDisplay.SetActive(true);
	}


	public void CloseInfoDisplay() {
		infoDisplay.SetActive(false);
		
	}

	public void SelectPlanet() {
		GUI_StarSelectionController.s.LandToPlanet(myPlanet);
	}

}
