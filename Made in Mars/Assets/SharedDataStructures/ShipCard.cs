using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipCard : ScriptableObject {

	public string uniqueName = "new Ship Card";

	public bool isSpawnableOnPlanet = true;
	public bool canOnlyHaveOne = false;

	public abstract Sprite GetImage();
	public abstract string GetName();
	public abstract string GetDescription();


}






