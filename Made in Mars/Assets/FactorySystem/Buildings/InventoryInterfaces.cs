using System.Collections.Generic;

public interface IInventorySimulation {
	void SetUp(List<InventoryItemSlot> existingItems);
	void SetInventory(List<InventoryItemSlot> _inventory);
	InventoryItemSlot AddSlot (Item item, int maxCount, InventoryItemSlot.SlotType slotType, bool reDrawInventory = true);
	bool TryAndAddItem(Item item, int amount, bool isOutput);
	bool TryAndAddItem(Item item, int amount, bool isOutput, bool isForced);
	bool CheckIfCanAddItem(Item item, int amount, bool isOutput);
	bool TryAndTakeItem(Item item, int amount, bool isOutput);
	bool CheckIfCanTakeItem(Item item, int amount, bool isOutput);
	bool TryAndTakeNextItem(out Item item);
	bool TryAndTakeNextItem(out Item item, bool isOutput);
	bool CheckIfCanTakeNextItem(out Item item);
	bool CheckIfCanTakeNextItem(out Item item, bool isOutput);
	int GetAmountOfItems(Item item);
	int GetTotalAmountOfItems(bool isOutput);
	int GetTotalAmountOfItems();
}

public interface IInventoryDisplayable {
	event GenericCallback drawInventoryEvent; 
	event GenericCallback inventoryContentsChangedEvent;
}

public interface IInventoryWithSlots {
    
}

public interface IInventoryWithGrids {
    
}