using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ShipCards/ResourceCard")]
public class ResourceShipCard : ShipCard, IShipCard {

    public Item item;
    public int count;
    public void ApplyEffect(ShipCardApplicableObjects data) {
        data.master.shipStarterInventory.Add(new InventoryItemSlot(item, count, count, InventoryItemSlot.SlotType.storage));
    }

    public override Sprite GetImage() {
        return item.GetSprite();
    }

    public override string GetName() {
        return $"{item.name} Card";
    }

    public override string GetDescription() {
        return $"Adds {count} {item.name} to your ship inventory when you land.";
    }
}