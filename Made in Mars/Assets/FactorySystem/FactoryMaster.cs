using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FactoryMaster {
    public static FactoryMaster s;
    public bool isSimStarted = false;
    public static float SimUpdatePerSecond = 4;
    public const float BeltLength = 1f;
    public static float BeltLengthToMovePerSecond = BeltLength * SimUpdatePerSecond;

    public const int SlotPerSegment = 4;
    public const int ConnectorTransporterCount = 6;


    public BuildingData beltBuildingData;
    public BuildingData connectorBuildingData;

    [SerializeField] List<ShipPart> shipParts = new List<ShipPart>();
    [SerializeField] List<Drone> drones = new List<Drone>();
    [SerializeField] List<Construction> constructions = new List<Construction>();
    [SerializeField] List<Belt> belts = new List<Belt>();
    [SerializeField] List<Connector> connectors = new List<Connector>();
    [SerializeField] List<Building> buildings = new List<Building>();

    public void RegisterLoad() {
        GameMaster.CallWhenLoaded(LoadFromSave);
        GameMaster.CallWhenFactorySimulationStart(StartFactorySystem);
        GameMaster.CallWhenFactorySimulationStop(StopFactorySystem);
        GameMaster.CallWhenClearPlanet(ClearPlanetEvent);
        DataSaver.s.saveEvent += SaveFactory;
    }

    public void UnregisterLoad() {
        GameMaster.RemoveFromCall(LoadFromSave);
        GameMaster.RemoveFromCall(StartFactorySystem);
        GameMaster.RemoveFromCall(StopFactorySystem);
        GameMaster.RemoveFromCall(ClearPlanetEvent);
        DataSaver.s.saveEvent -= SaveFactory;
    }

    void ClearPlanetEvent() {
        shipParts.Clear();
        drones.Clear();
        constructions.Clear();
        belts.Clear();
        connectors.Clear();
        buildings.Clear();
    }

    public void LoadFromSave(bool isSuccess) {
        if (isSuccess) {
            foreach (var saveData in DataSaver.s.mySave.beltData) {
                FactoryBuilder.CreateBeltFromSave(saveData);
            }

            foreach (var saveData in DataSaver.s.mySave.buildingData) {
                FactoryBuilder.CreateBuildingFromSave(saveData);
            }

            foreach (var saveData in DataSaver.s.mySave.connectorData) {
                FactoryBuilder.CreateConnectorFromSave(saveData);
            }

            foreach (var saveData in DataSaver.s.mySave.constructionData) {
                FactoryBuilder.CreateConstructionFromSave(saveData);
            }

            foreach (var saveData in DataSaver.s.mySave.droneData) {
                var drone = FactoryBuilder.CreateDroneFromSave(saveData);
            }
            
            FactoryBuilder.ObjectsUpdated?.Invoke();
            FactoryBuilder.DronesUpdated?.Invoke();
        } 
    }


    public void SaveFactory() {
        DataSaver.s.mySave.beltData = new List<DataSaver.BeltSaveData>();
        foreach (var belt in belts) {
            DataSaver.s.mySave.beltData.Add(new DataSaver.BeltSaveData(
                belt.startPos, belt.endPos, belt.direction, FactoryBuilder.DecompressBeltForSaving(belt)
            ));
        }

        DataSaver.s.mySave.buildingData = new List<DataSaver.BuildingSaveData>();
        foreach (var building in buildings) {
            DataSaver.s.mySave.buildingData.Add(new DataSaver.BuildingSaveData(
                building.buildingData.uniqueName, building.center, building.inv.inventoryItemSlots,
                building.craftController.lastCheckId, building.craftController.GetCraftingProcessProgress()
            ));
        }


        DataSaver.s.mySave.connectorData = new List<DataSaver.ConnectorSaveData>();
        foreach (var connector in connectors) {
            DataSaver.s.mySave.connectorData.Add(new DataSaver.ConnectorSaveData(
                connector.startPos, connector.endPos, connector.direction
            ));
        }

        DataSaver.s.mySave.constructionData = new List<DataSaver.ConstructionSaveData>();
        foreach (var construction in constructions) {
            DataSaver.s.mySave.constructionData.Add(new DataSaver.ConstructionSaveData(
                construction.myData.uniqueName, construction.center, construction.direction,
                construction.isConstruction, construction.IsAssignedToDrone(),
                construction.constructionInventory.inventoryItemSlots, construction.afterConstructionInventory
            ));
        }

        DataSaver.s.mySave.droneData = new List<DataSaver.DroneSaveData>();
        foreach (var drone in drones) {
            Position curTaskPos = Position.InvalidPosition();
            List<InventoryItemSlot> curTaskMaterials = new List<InventoryItemSlot>();

            if (drone.currentTask != null) {
                switch (drone.currentTask.myType) {
                    case DroneTask.DroneTaskType.build:
                    case DroneTask.DroneTaskType.destroy:
                        curTaskPos = drone.currentTask.construction.center;
                        curTaskMaterials = drone.currentTask.materials;
                        break;
                    case DroneTask.DroneTaskType.transportShipPart:
                        curTaskPos = drone.currentTask.shipPart.curPosition;
                        break;

                    case DroneTask.DroneTaskType.transportItem:
                        throw new NotImplementedException();
                        break;
                }
            }

            DataSaver.s.mySave.droneData.Add(new DataSaver.DroneSaveData(
                drone.curPosition, drone.targetPosition,
                drone.isTravelling, drone.isBusy, drone.isLaser,
                curTaskPos, curTaskMaterials,
                drone.myInventory.inventoryItemSlots,
                Drone.ConvertDroneStateAndInt(drone.myState)
            ));
        }
    }

    public void StartFactorySystem() {
        isSimStarted = true;
        FactoryBuilder.ObjectsUpdated?.Invoke();
        FactorySimulator.s.StartSimulation();
    }
    
    public void StopFactorySystem() {
        isSimStarted = false;
        FactorySimulator.s.StopSimulation();
    }


    public void AddBelt(Belt toAdd) {
        belts.Add(toAdd);
    }

    public void RemoveBelt(Belt toRemove) {
        belts.Remove(toRemove);
    }

    public List<Belt> GetBelts() {
        return belts;
    }

    public void AddConnector(Connector toAdd) {
        connectors.Add(toAdd);
    }

    public void RemoveConnector(Connector toRemove) {
        connectors.Remove(toRemove);
    }

    public List<Connector> GetConnectors() {
        return connectors;
    }

    public void AddBuilding(Building toAdd) {
        buildings.Add(toAdd);
    }

    public void RemoveBuilding(Building toRemove) {
        buildings.Remove(toRemove);
    }

    public List<Building> GetBuildings() {
        return buildings;
    }

    public void AddConstruction(Construction toAdd) {
        constructions.Add(toAdd);
    }

    public void RemoveConstruction(Construction toRemove) {
        constructions.Remove(toRemove);
    }

    public List<Construction> GetConstructions() {
        return constructions;
    }


    public void AddDrone(Drone toAdd) {
        drones.Add(toAdd);
    }

    public void RemoveDrone(Drone toRemove) {
        drones.Remove(toRemove);
    }

    public List<Drone> GetDrones() {
        return drones;
    }
    
    public void AddShipPart(ShipPart toAdd) {
        shipParts.Add(toAdd);
    }

    public void RemoveShipPart(ShipPart toRemove) {
        shipParts.Remove(toRemove);
    }

    public List<ShipPart> GetShipParts() {
        return shipParts;
    }

    public delegate void ItemCreated(Item item, int count);
}

public class SimGridObject {
    
}

public interface IAssignableToDrone {
    bool IsAssignedToDrone();
    void AssignToDrone();
    void UnAssignFromDrone();
    Position GetPositionForDrone();
}

public class ShipPart : IAssignableToDrone {
    public Position curPosition;
    

    public ShipPart(Position location) {
        curPosition = location;
    }

    private bool isAssignedToDrone = false;
    public bool IsAssignedToDrone() { return isAssignedToDrone; }
    public void AssignToDrone() { isAssignedToDrone = true; }
    public void UnAssignFromDrone() { isAssignedToDrone = false; }
    public Position GetPositionForDrone() { return curPosition; }
}

/// <summary>
/// Drones construct and deconstruct objects
/// </summary>
[Serializable]
public class Drone  {
    Position _curPosition;
    public Position curPosition {
        get { return _curPosition; }
        set { _curPosition = value; dronePositionUpdatedCallback?.Invoke(); }
    }

    Position _targetPosition = Position.InvalidPosition();
    public Position targetPosition {
        get { return _targetPosition; }
        set { _targetPosition = value; droneTargetPositionUpdatedCallback?.Invoke(); }
    }
    
    public bool isTravelling = false;
    public bool isBusy = false;
    
    bool _isLaser = false;
    public bool isLaser {
        get { return _isLaser; }
        set { _isLaser = value; droneLaserStateUpdatedCallback?.Invoke(); }
    }

    public DroneTask currentTask;

    public Inventory myInventory;
    public Inventory constructionInv {
        get { return currentTask.construction.constructionInventory; }
    }

    public GenericCallback dronePositionUpdatedCallback;
    public GenericCallback droneTargetPositionUpdatedCallback;
    public GenericCallback droneLaserStateUpdatedCallback;

    public IDroneState myState = new DroneIdleState();

    public Building targetStorage;
    public float idleCounter = 0;

    public List<string> debugDroneState = new List<string>();

    public Drone(Position _curPosition) {
        myInventory= InventoryFactory.CreateDroneInventory();
        curPosition = _curPosition;
    }


    private static readonly Map< int,Type> droneStateSavingMap = new Map<int ,Type >() {
        {0, typeof(DroneIdleState)},
        {1, typeof(DroneBeginConstruction)},
        {2, typeof(DroneSearchItemForConstruction)},
        {3, typeof(DroneTravelToItemStorage)},
        {4, typeof(DroneTakeItemFromStorage)},
        {5, typeof(DroneTravelToConstruction)},
        {6, typeof(DroneConstruct)},
        {7, typeof(DroneBeginDeconstruction)},
        {8, typeof(DroneTravelToDeconstruction)},
        {9, typeof(DroneDeconstruct)},
        {10, typeof(DroneSearchStorageToEmptyInventory)},
        {11, typeof(DroneTravelToEmptyDroneInventoryToStorage)},
        {12, typeof(DroneEmptyInventoryToStorage)},
        {13, typeof(DroneUnableToFindConstructionMaterial)},
        {14, typeof(DroneUnableToFindEmptyStorage)},
    };
    public static int ConvertDroneStateAndInt(IDroneState state) {
        return droneStateSavingMap.Get(state.GetType());
    }
	
    public static IDroneState ConvertDroneStateAndInt(int type) {
        return (IDroneState) Activator.CreateInstance(droneStateSavingMap.Get(type));
    }
    
    public interface IDroneState {
        IDroneState ExecuteAndReturnNextState(Drone drone);
        string GetInfoDisplayText(Drone drone);
    }

}




/// <summary>
/// Used for construction and deconstruction of items.
/// </summary>
[Serializable]
public class Construction : SimGridObject, IAssignableToDrone {
    public List<Position> locations = new List<Position>();
    public Position center;
    public int direction;
    public BuildingData myData;
    public Inventory constructionInventory;
    // Construction object is used both for construction and destruction. This bool determines which
    public bool isConstruction = true;

    private bool isAssignedToDrone = false;
    public bool IsAssignedToDrone() { return isAssignedToDrone; }
    public void AssignToDrone() { isAssignedToDrone = true; }
    public void UnAssignFromDrone() { isAssignedToDrone = false; }
    public Position GetPositionForDrone() { return center; }

    public List<InventoryItemSlot> afterConstructionInventory;
    
    public Construction(BuildingData buildingData, int _direction, Position _center,
        List<InventoryItemSlot> materials, List<InventoryItemSlot> _afterConstructionInventory, 
        bool _isConstruction) {
        center = _center;
        direction = _direction;
        myData = buildingData;
        if (myData.myType != BuildingData.ItemType.Belt && myData.myType != BuildingData.ItemType.Connector) {
            locations = myData.shape.CoveredPositions(center);
        } else {
            locations.Clear();
            locations.Add(center);
        }
        isConstruction = _isConstruction;
        constructionInventory = InventoryFactory.CreateConstructionInventory(materials);
        afterConstructionInventory = _afterConstructionInventory;
    }
}

/// <summary>
/// One of the core building blocks of the belt system.
/// A belt has a start point and a end point, and it is a straight line.
/// It holds item in the way (# = empty slot, O = ore slot)
/// (2, null), (3, ore), (1, null), (1, ore) >> ##OOO#O
/// </summary>
[System.Serializable]
public class Belt : SimGridObject {
    public Position startPos;
    public Position endPos;
    public int direction;
    
    public int length {
        get { return Position.Distance(startPos, endPos) + 1; }
    }
        
    public List<BeltSegment> items = new List<BeltSegment>();

    /// <summary>
    /// Removes and returns the last item in the belt, whether actual item or empty
    /// automatically replaces it with empty and does reshaping as needed
    /// </summary>
    /// <returns>Returns an item (can be empty item)</returns>
    public bool TryRemoveLastItemFromBelt(out Item item) {
        var lastIndex = items.Count - 1;
        item = items[lastIndex].item;
        
        // If the last item is not empty
        if (!item.isEmpty()) {
            int count = items[lastIndex].count;
            count -= 1; // reduce its count by 1

            // If the entire section gets to zero
            if (count == 0) {
                // If the belt has only one section, it is an edge case
                // In this case we just replace the current slot with an empty slot
                if (lastIndex == 0) {

                    items[lastIndex] = new BeltSegment() {count = 1, item = Item.GetEmpty()};
                } else {
                    // Remove the last section
                    items.RemoveAt(lastIndex);
                    lastIndex -= 1;

                    // If the previous section was also empty, add to it
                    if (items[lastIndex].item.isEmpty()) {
                        items[lastIndex] = new BeltSegment() {count = items[lastIndex].count + 1, item = items[lastIndex].item};
                    } else {
                        // If the previous section was not empty, then add an empty section at the end
                        items.Add(new BeltSegment() {count = 1, item = Item.GetEmpty()});
                    }
                }
            } else {
                //if the entire section does not get to zero, reduce its count and add an empty section at the end
                items[lastIndex] = new BeltSegment() {count = count, item = item};
                items.Add(new BeltSegment() {count = 1, item = Item.GetEmpty()});

            }

            return true;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// Returns the last item in the belt without removing it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CheckTakeLastItemFromBelt(out Item item) {
        var lastIndex = items.Count - 1;
        item = items[lastIndex].item;
        return !item.isEmpty();
    }

    /// <summary>
    /// Tries to insert an item to the beginning of the belt
    /// Automatically merges slots if the next slot matches with the item
    /// </summary>
    /// <returns>Returns true only if there is empty slot at the start of the belt</returns>
    public bool TryInsertItemToBelt(Item itemToInsert) {
        if (itemToInsert.isEmpty()) {
            return true;
        }
        
        if (items[0].item.isEmpty()) {
            if (items[0].count == 1) {
                if (items.Count > 1 && items[1].item == itemToInsert) {
                    items[1] = new BeltSegment() {item = itemToInsert, count = items[1].count + 1};
                    items.RemoveAt(0);
                } else {
                    items[0] = new BeltSegment() {item = itemToInsert, count = 1};
                }
            } else {
                items[0] = new BeltSegment() {item = items[0].item, count = items[0].count - 1};
                items.Insert(0, new BeltSegment(){item = itemToInsert, count = 1});
            }

            return true;
        } else {
            return false;
        }
    }
    
    
    /// <summary>
    /// Checks if we can insert the item to the belt.
    /// Does NOT actually inserts it!
    /// </summary>
    /// <returns>Returns true only if there is empty slot at the start of the belt</returns>
    public bool CheckInsertItemToBelt(Item itemToInsert) {
        if (itemToInsert.isEmpty()) {
            return true;
        }
        
        if (items[0].item.isEmpty()) {
            return true;
        } else {
            return false;
        }
    }

    [Serializable]
    public class BeltSegment {
        public int count;
        public Item item;

        public void AddCount(int amount) {
            count += amount;
        }
    }
    
    public Belt(Position startPos, Position endPos, int direction) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.direction = direction;
        
        items.Add(new BeltSegment(){count = length*FactoryMaster.SlotPerSegment, item = Item.GetEmpty()});
    }

    public Belt(Position startPos, Position endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.direction = Position.CardinalDirection(startPos,endPos);
        
        items.Add(new BeltSegment(){count = length*FactoryMaster.SlotPerSegment, item = Item.GetEmpty()});
    }


    public int CardinalDirection() {
        return this.direction;
    }
}





/// <summary>
/// The second core building block of the belt system.
/// A connector will connect belts with other belts or belts with buildings.
/// A connector will have inputs and outputs, and it will try to balance input/output equally
/// A connector will also always be a straight line
/// </summary>
[Serializable]
public class Connector : SimGridObject {
    public Position startPos;
    public Position endPos;
    public int direction;

    public ItemTransportJob[] itemTransportJobs = new ItemTransportJob[] { };

    public GenericCallback ConnectorInputsUpdatedCallback;
    public Connector(Position startPos, Position endPos, int direction) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.direction = direction;
        
        CreateItemTransportJobs(FactoryMaster.ConnectorTransporterCount);
    }

    public Connector(Position startPos, Position endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.direction = Position.ParallelDirection(startPos,endPos);

        CreateItemTransportJobs(FactoryMaster.ConnectorTransporterCount);
    }

    void CreateItemTransportJobs(int count) {
        itemTransportJobs = new ItemTransportJob[count];
        for (int i = 0; i < count; i++) {
            itemTransportJobs[i] = new ItemTransportJob();
        }
    }


    public int CardinalDirection() {
        return this.direction;
    }

    public int length {
        get { return Position.Distance(startPos, endPos) + 1; }
    }

    public class ItemTransportJob {
        public bool isMovingItem = false;
        public bool isTakingFromInput = false;
        public Connection startConnection;
        public Connection endConnection;
        public Item movingItem;
        public Position itemCurPos;
        public int itemDir;
    }


    [Serializable]
    public class Connection {
        public bool isBeltConnection;
        public Belt belt;
        public Building building;
        public Position position;
        public int direction;


        public Connection(Belt _belt, Position _position, int _direction) {
            isBeltConnection = true;
            belt = _belt;
            position = _position;
            direction = _direction;
        }
        
        public Connection(Building _building, Position _position, int _direction) {
            isBeltConnection = false;
            building = _building;
            position = _position;
            direction = _direction;
        }
        

        /// <summary>
        /// Try to insert the item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryInsertItem(Item item) {
            if (isBeltConnection) {
                return belt.TryInsertItemToBelt(item);
            }else {
                return building.TryAndInsertItemToBuilding(item);
            }
        }
        /// <summary>
        /// Try to get the next item without removing it
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckInsertItem(Item item) {
            if (isBeltConnection) {
                return belt.CheckInsertItemToBelt(item);
            } else {
                return building.CheckIfCanInsertItemToBuilding(item);
            }
        }

        /// <summary>
        /// Try and take the next item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryTakeItem(out Item item) {
            if (isBeltConnection) {
                return belt.TryRemoveLastItemFromBelt(out item);
            }else {
                return building.TryAndTakeItemFromBuilding(out item);
            }
        }

        /// <summary>
        /// Try to get the next item without removing it
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckTakeItem(out Item item) {
            if (isBeltConnection) {
                return belt.CheckTakeLastItemFromBelt(out item);
            }else {
                return building.CheckIfCanTakeItem(out item);
            }
        }
        
        
        public static bool operator ==(Connection a, Connection b) {
            if (a.isBeltConnection) {
                if (b.isBeltConnection) {
                    return a.belt == b.belt;
                } else {
                    return false;
                }
                
            } else {
                if (!b.isBeltConnection) {
                    return a.building == b.building;
                } else {
                    return false;
                }
            }
        }
	
        public static bool operator !=(Connection a, Connection b) {
            if (a.isBeltConnection) {
                if (b.isBeltConnection) {
                    return a.belt != b.belt;
                } else {
                    return true;
                }
                
            } else {
                if (!b.isBeltConnection) {
                    return a.building != b.building;
                } else {
                    return true;
                }
            }
        }
       
    }
    
    public int inputCounter;
    public List<Connection> inputs = new List<Connection>();
        
    public int outputCounter;
    public List<Connection> outputs = new List<Connection>();
}


[Serializable]
public class Building : SimGridObject {
    public bool isDestructable = true;
    public BuildingData buildingData;

    public List<Position> myPositions = new List<Position>();
    public Position center = Position.InvalidPosition();

    public Inventory inv;
    public BuildingCraftingController craftController = new BuildingCraftingController();

    // This is used when creating building from save as it also resumes the crafting controller
    public Building(Position _center, BuildingData _buildingData, List<InventoryItemSlot> existingInventory,
        int lastCheckId = -1, float[] craftingProcessProgress = null) {
        if (_buildingData == null) {
            Debug.Log("Trying to create a building with null data!");
        } else {
            center = _center;
            buildingData = _buildingData;
            myPositions = buildingData.shape.CoveredPositions(center);
            craftController.SetUp(buildingData, lastCheckId, craftingProcessProgress);
            inv = InventoryFactory.CreateBuildingInventory(existingInventory, this);
        }
    }

    public bool TryAndInsertItemToBuilding(Item item) {
        return inv.TryAndAddItem(item, 1, false);
    }


    public bool CheckIfCanInsertItemToBuilding(Item item) {
        return inv.CheckIfCanAddItem(item, 1, false);
    }

    public bool TryAndTakeItemFromBuilding(out Item item) {
        return inv.TryAndTakeNextItem(out item, true);
    }

    public bool CheckIfCanTakeItem(out Item item) {
        return inv.CheckIfCanTakeNextItem(out item, true);
    }


    public float UpdateCraftingProcess(float energySupply, Inventory inventory) {
        return craftController.UpdateCraftingProcess(energySupply, inventory);
    }
}

