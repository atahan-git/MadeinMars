using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShipCards/UpgradeCard")]
public class ShipUpgradeCard : ShipCard, IShipCard {

    public void ApplyEffect(ShipCardApplicableObjects data) {
        
    }

    public override Sprite GetImage() {
        return null;
    }

    public override string GetName() {
        return $"{name} Card";
    }

    public override string GetDescription() {
        return $"Applies {name} upgrade.";
    }
}
