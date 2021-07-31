using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ShipCards/DroneCard")]
public class DroneShipCard : ShipCard, IShipCard {
    public Sprite droneSprite;
    public int count;
    public void ApplyEffect(ShipCardApplicableObjects data) {
        data.master.droneCount += count;
    }

    public override Sprite GetImage() {
        return droneSprite;
    }

    public override string GetName() {
        return $"Drone Card";
    }

    public override string GetDescription() {
        return $"Adds {count} drones to your control after you land";
    }
}