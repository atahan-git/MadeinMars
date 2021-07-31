using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShipCards/BuildingCard")]
public class BuildingShipCard : ShipCard, IShipCard {

    public BuildingData buildingData;
    public void ApplyEffect(ShipCardApplicableObjects data) {
        data.master.playerBuildableBuildingsSetByShipCards.Add(buildingData);
    }

    public override Sprite GetImage() {
        return buildingData.GetSprite();
    }

    public override string GetName() {
        return $"{buildingData.name} Card";
    }

    public override string GetDescription() {
        return $"Adds {buildingData.name} to your buildable buildings list.";
    }
}