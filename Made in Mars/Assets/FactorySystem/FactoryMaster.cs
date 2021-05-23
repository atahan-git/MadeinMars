using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMaster : MonoBehaviour {
	public static FactoryMaster s;
    public bool isSimStarted = false;
	public static float SimUpdatePerSecond = 4;
	public const float BeltLength = 1f;
	public static float BeltLengthToMovePerSecond = BeltLength * SimUpdatePerSecond;

	public const int SlotPerSegment = 4;
	public const int ConnectorTransporterCount = 6;
    
    public BuildingData beltBuildingData;
    public BuildingData connectorBuildingData;

    [SerializeField]
    List<Drone> drones = new List<Drone>();
    [SerializeField]
    List<Construction> constructions = new List<Construction>();
    [SerializeField]
    List<Belt> belts = new List<Belt>();
    [SerializeField]
    List<Connector> connectors = new List<Connector>();
    [SerializeField]
    List<Building> buildings = new List<Building>();

    public int population;
    public int housed;
    public int workers;
    public int jobs;

    private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
        
    }

    private void Start() {
        GameLoader.CallWhenLoaded(LoadFromSave);
        DataSaver.s.saveEvent += SaveFactory;
    }

    private void OnDestroy() {
        GameLoader.RemoveFromCall(LoadFromSave);
        DataSaver.s.saveEvent -= SaveFactory;
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
                FactoryVisuals.s.CreateDroneVisuals(drone);
            }
        } /*else {
            // For now, lets create some drones manually
            for (int i = 0; i < 3; i++) {
                
            }
        }*/
        
        //FactoryBuilder.ObjectsUpdated?.Invoke();
    }

    public void CreateDrone(Position location) {
        var drone = new Drone(location);
            
        AddDrone(drone);
        FactoryVisuals.s.CreateDroneVisuals(drone);
    }


    public void SaveFactory() {
        foreach (var belt in belts) {
            DataSaver.s.BeltsToBeSaved.Add(new DataSaver.BeltSaveData(
                belt.startPos, belt.endPos, belt.direction, FactoryBuilder.DecompressBeltForSaving(belt)
                ));
        }
        
        foreach (var building in buildings) {
            DataSaver.s.BuildingsToBeSaved.Add(new DataSaver.BuildingSaveData(
                building.buildingData.uniqueName, building.center, building.invController.inventory,
                building.craftController.lastCheckId, building.craftController.GetCraftingProcessProgress()
            ));
        }
        
        
        foreach (var connector in connectors) {
            DataSaver.s.ConnectorsToBeSaved.Add(new DataSaver.ConnectorSaveData(
                connector.startPos, connector.endPos, connector.direction
            ));
        }
        
        foreach (var construction in constructions) {
            DataSaver.s.ConstructionsToBeSaved.Add(new DataSaver.ConstructionSaveData(
                construction.myData.uniqueName, construction.center, construction.direction,
                construction.isConstruction, construction.isAssignedToDrone,
                construction.constructionInventory.inventory, construction.afterConstructionInventory
            ));
        }
        
        foreach (var drone in drones) {
            Position curTaskPos = Position.InvalidPosition();
            List<InventoryItemSlot> curTaskMaterials = new List<InventoryItemSlot>();

            if (drone.currentTask != null) {
                if (drone.currentTask.location.isValid()) {
                    curTaskPos = drone.currentTask.location;
                    curTaskMaterials = drone.currentTask.materials;
                }
            }

            Position targetStorageLocation = Position.InvalidPosition();
            if (drone.targetStorage != null) {
                if(drone.targetStorage.myLocation.isValid())
                    targetStorageLocation = drone.targetStorage.myLocation;
            }
            
            Position constructionInventoryLocation = Position.InvalidPosition();
            if (drone.constructionInv != null) {
                if(drone.constructionInv.myLocation.isValid())
                    constructionInventoryLocation = drone.constructionInv.myLocation;
            }
            
            DataSaver.s.DronesToBeSaved.Add(new DataSaver.DroneSaveData(
                drone.curPosition, drone.targetPosition,
                drone.isTravelling, drone.isBusy, drone.isLaser,
                curTaskPos, curTaskMaterials,
                drone.myInventory.inventory,
                Drone.DroneStateToInt(drone.myState),
                targetStorageLocation,
                constructionInventoryLocation
            ));
        }
    }

	public void StartFactorySystem () {
        isSimStarted = true;
        FactoryBuilder.ObjectsUpdated?.Invoke();
        FactorySimulator.s.StartSimulation();
    }


    public void AddBelt(Belt toAdd) { belts.Add(toAdd); }
    public void RemoveBelt(Belt toRemove) { belts.Remove(toRemove); }
    public List<Belt> GetBelts() { return belts; }
    
    public void AddConnector(Connector toAdd) { connectors.Add(toAdd); }
    public void RemoveConnector(Connector toRemove) { connectors.Remove(toRemove); }
    public List<Connector> GetConnectors() { return connectors; }
    
    public void AddBuilding(Building toAdd) { buildings.Add(toAdd); }
    public void RemoveBuilding(Building toRemove) { buildings.Remove(toRemove); }
    public List<Building> GetBuildings() { return buildings; }
    
    public void AddConstruction(Construction toAdd) { constructions.Add(toAdd); }
    public void RemoveConstruction(Construction toRemove) { constructions.Remove(toRemove); }
    public List<Construction> GetConstructions() { return constructions; }
    
    
    public void AddDrone(Drone toAdd) { drones.Add(toAdd); }
    public void RemoveDrone(Drone toRemove) { drones.Remove(toRemove); }
    public List<Drone> GetDrones() { return drones; }
}


/// <summary>
/// Drones construct and deconstruct objects
/// </summary>
[Serializable]
public class Drone {
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

    public BuildingInventoryController myInventory = new BuildingInventoryController();

    public GenericCallback dronePositionUpdatedCallback;
    public GenericCallback droneTargetPositionUpdatedCallback;
    public GenericCallback droneLaserStateUpdatedCallback;

    public enum DroneState {
        idle,
        BeginBuildingTask,
        SearchingItem,
        TravellingToItem,
        TakingItem,
        TravellingToBuild,
        Building,
        SearchingToEmptyInventory,
        TravellingToEmptyInventory,
        EmptyingInventory,
        SearchingToDestroy,
        BeingDestructionTask,
        TravellingToDestroy,
        Destroying,
        UnableToFindConstructionItem,
        UnableToFindEmptyStorage
    }

    public DroneState myState = DroneState.idle;

    public BuildingInventoryController targetStorage;
    public BuildingInventoryController constructionInv;
    public float idleCounter = 0;


    public Drone(Position _curPosition) {
        myInventory.SetUpDrone();
        curPosition = _curPosition;
    }
    
    public static int DroneStateToInt(DroneState type) {
        switch (type) {
            case DroneState.idle: return 0;
            case DroneState.BeginBuildingTask: return 1;
            case DroneState.SearchingItem: return 2;
            case DroneState.TravellingToItem: return 3;
            case DroneState.TakingItem: return 4;
            case DroneState.TravellingToBuild: return 5;
            case DroneState.Building: return 6;
            case DroneState.SearchingToEmptyInventory: return 7;
            case DroneState.TravellingToEmptyInventory: return 8;
            case DroneState.EmptyingInventory: return 9;
            case DroneState.SearchingToDestroy: return 10;
            case DroneState.BeingDestructionTask: return 11;
            case DroneState.TravellingToDestroy: return 12;
            case DroneState.Destroying: return 13;
            case DroneState.UnableToFindConstructionItem: return 14;
            case DroneState.UnableToFindEmptyStorage: return 15;
        }

        return -1;
    }
	
    public static DroneState IntToDroneState(int type) {
        switch (type) {
            case 0: return DroneState.idle;
            case 1: return DroneState.BeginBuildingTask;
            case 2: return DroneState.SearchingItem;
            case 3: return DroneState.TravellingToItem;
            case 4: return DroneState.TakingItem;
            case 5: return DroneState.TravellingToBuild;
            case 6: return DroneState.Building;
            case 7: return DroneState.SearchingToEmptyInventory;
            case 8: return DroneState.TravellingToEmptyInventory;
            case 9: return DroneState.EmptyingInventory;
            case 10: return DroneState.SearchingToDestroy;
            case 11: return DroneState.BeingDestructionTask;
            case 12: return DroneState.TravellingToDestroy;
            case 13: return DroneState.Destroying;
            case 14: return DroneState.UnableToFindConstructionItem;
            case 15: return DroneState.UnableToFindEmptyStorage;
        }

        return DroneState.idle;
    }
}



/// <summary>
/// Used for construction and deconstruction of items.
/// </summary>
[Serializable]
public class Construction {
    public List<Position> locations = new List<Position>();
    public Position center;
    public int direction;
    public BuildingData myData;
    public BuildingInventoryController constructionInventory;
    // Construction object is used both for construction and destruction. This bool determines which
    public bool isConstruction = true;

    public bool isAssignedToDrone = false;

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
        constructionInventory = new BuildingInventoryController();
        constructionInventory.SetUpConstruction(_center, materials);
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
public class Belt {
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
public class Connector {
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
                return building.TryInsertItemToBuilding(item);
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
                return building.CheckInsertItemToBuilding(item);
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
                return building.TryTakeItemFromBuilding(out item);
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
                return building.CheckTakeItem(out item);
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
public class Building {
    public bool isDestructable = true;
    public BuildingData buildingData;

    public List<Position> myPositions = new List<Position>();
    public Position center = Position.InvalidPosition();

    public BuildingInventoryController invController = new BuildingInventoryController();
    public BuildingCraftingController craftController = new BuildingCraftingController();

    public Building(Position _center, BuildingData _buildingData, List<InventoryItemSlot> starterInventory) {
        if (_buildingData == null) {
            Debug.Log("Trying to create a building with null data!");
        } else {
            center = _center;
            buildingData = _buildingData;
            myPositions = buildingData.shape.CoveredPositions(center);
            craftController.SetUp(buildingData, invController);
            invController.SetUp(_center, craftController, buildingData, starterInventory);
        }
    }
    
    /// <summary>
    ///  This is used when creating building from save as it also resumes the crafting controller
    /// </summary>
    public Building(Position _center, BuildingData _buildingData, List<InventoryItemSlot> starterInventory, 
        int lastCheckId, float[] CraftingProcessProgress) {
        if (_buildingData == null) {
            Debug.Log("Trying to create a building with null data!");
        } else {
            center = _center;
            buildingData = _buildingData;
            myPositions = buildingData.shape.CoveredPositions(center);
            craftController.SetUp(buildingData, invController, lastCheckId, CraftingProcessProgress);
            invController.SetUp(_center, craftController, buildingData, starterInventory);
        }
    }

    /// <summary>
    /// Try to insert an item to the buildings input slots
    /// Only works if the item type matches and there is enough space
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryInsertItemToBuilding(Item item) {
        return invController.TryAddItem(item, 1, false);
    }
    
    
    /// <summary>
    /// Checks if we can insert the item to the building.
    /// Does NOT actually insert it!
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CheckInsertItemToBuilding(Item item) {
        return invController.CheckAddItem(item, 1, false);
    }

    /// <summary>
    /// Tries to take an item from a building output slot.
    /// Always gives priority to the smaller indexed output slot.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryTakeItemFromBuilding(out Item item) {
        return invController.TryTakeNextItem(out item, true);
    }

    /// <summary>
    /// Gets the next item that would come out of the building without removing it
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CheckTakeItem(out Item item) {
        return invController.CheckTakeNextItem(out item, true);
    }


    public float UpdateCraftingProcess(float energySupply) {
        return craftController.UpdateCraftingProcess(energySupply);
    }
}
