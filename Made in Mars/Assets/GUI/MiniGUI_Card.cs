using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

public class MiniGUI_Card : MonoBehaviour {

	public Image cardIcon;
	public Text cardName;
	public Text cardDescription;

	public ShipCard myCard;


	private void Start() {
		var buildingWorldObject = GetComponentInParent<BuildingWorldObject>();
		if (buildingWorldObject != null) {
			SetUp(DataHolder.s.BuildingDataToShipCard(buildingWorldObject.myData));
		}
	}

	public void SetUp(ShipCard card) {
		myCard = card;

		cardIcon.sprite = myCard.GetImage();
		cardName.text = myCard.GetName();
		cardDescription.text = myCard.GetDescription();
	}

	public void ManualSetUp() {
		SetUp(GetComponentInParent<ShipCardWorldObject>().myCard);
	}
}
