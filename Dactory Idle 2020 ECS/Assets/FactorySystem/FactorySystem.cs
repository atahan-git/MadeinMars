using System;
using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FactorySystem : MonoBehaviour {
    public static FactorySystem s;

    public const int SlotPerSegment = 4;
    public const int ConnectorTransporterCount = 6;

    public List<Belt> belts = new List<Belt>();
    public List<Connector> connectors = new List<Connector>();
    public List<Building> buildings = new List<Building>();
    
    
    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
    }
    
    /// <summary>
    /// Find the last empty spot in the belt and reduce it by 1.
    /// The Factorio belt optimization
    /// </summary>
    /// <param name="belt"></param>
    public static void UpdateBelt(Belt belt) {
        bool thereIsEmptySlot = false;


        if (belt.items.Count == 0) {
            Debug.LogError("Belt doesn't have belt segments! " + belt.startPos + " - " + belt.endPos);
            return;
        }

        // Edge case where the belt is perfectly uniform, hence we cannot move anything
        if (belt.items.Count == 1) {
            return;
        }
        
        
        var beltsLastSlot = belt.items.Count-1;
        
        for (int i = beltsLastSlot; i >= 0; i--) { 
            if (belt.items[i].item.isEmpty()) {
                var count = belt.items[i].count;
                count -= 1;
                bool slotDestroyed;
                if (count > 0) {
                    belt.items[i] = new Belt.BeltSegment() {count = count, item = belt.items[i].item};
                    slotDestroyed = false;
                } else {
                    belt.items.RemoveAt(i);
                    slotDestroyed = true;
                }
                    
                //If we destroyed a slot, me might need to merge the new touching slots
                if (slotDestroyed) {
                    // We cannot merge if the destroyed slot was at the very end or at the very start
                    if (i != beltsLastSlot) {
                        if (i != 0) {
                            // Check if the current slot (right after the empty slot) and the previous slot have the same item
                            if (belt.items[i].item == belt.items[i - 1].item) {
                                belt.items[i - 1] = new Belt.BeltSegment() {
                                    count = belt.items[i].count + belt.items[i - 1].count,
                                    item = belt.items[i].item
                                };
                                
                                belt.items.RemoveAt(i);
                            }
                        }
                    }
                }

                thereIsEmptySlot = true;
                break;
            }
        }

        if (thereIsEmptySlot) {
            if (belt.items[0].item.isEmpty()) {
                var count = belt.items[0].count;
                count += 1;
                belt.items[0] = new Belt.BeltSegment() {count = count, item = belt.items[0].item};
            } else {
                belt.items.Insert(0, new Belt.BeltSegment(){count = 1, item = Item.GetEmpty()});
            }
        }
    }


    /// <summary>
    /// Updates the building - handled by building crafting controller
    /// </summary>
    /// <param name="building"></param>
    public static float UpdateBuilding(Building building, float efficiency) {
        return building.UpdateCraftingProcess(efficiency);
    }

    /// <summary>
    /// In each sim step each connect will find the next input slot item, and put it into the next free output slot.
    /// </summary>
    /// <param name="connector"></param>
    public static void UpdateConnector(Connector connector) {

        for (int job = 0; job < connector.itemTransportJobs.Length; job++) {
            var curJob = connector.itemTransportJobs[job];
            // If we are not moving an item, then take it.
            if (!curJob.isMovingItem) {
                // Move over every possible input source once
                for (int i = 0; i < connector.inputs.Count; i++) {
                    var n = (i + connector.inputCounter) % connector.inputs.Count;
                    // Try to take an item from a belt or a building
                    if (connector.inputs[n].CheckTakeItem(out Item curItem)) {
                        connector.inputCounter = n + 1;

                        if (ConnectorTryToPlaceItem(connector, curItem, out Connector.Connection target, connector.inputs[n])) {
                            curJob.isMovingItem = true;
                            curJob.isTakingFromInput = true;
                            curJob.startConnection = connector.inputs[n];
                            curJob.endConnection = target;
                            curJob.movingItem = curItem;
                            curJob.itemCurPos = Position.MoveCardinalDirection(curJob.startConnection.position, curJob.startConnection.direction, 1);
                            curJob.itemDir = Position.CardinalDirection(curJob.itemCurPos, curJob.startConnection.position);
                            connector.inputs[n].TryTakeItem(out curItem);
                            break;
                        }
                    }
                }
            } else {
                // The first move from belt/building ONTO connector
                if (curJob.isTakingFromInput) {
                    curJob.itemCurPos = curJob.startConnection.position;
                    curJob.itemDir = Position.CardinalDirection(curJob.startConnection.position, curJob.endConnection.position);
                    curJob.isTakingFromInput = false;


                    // Subsequent moves that are on top of the connector
                } else if (curJob.itemCurPos != curJob.endConnection.position) {
                    curJob.itemCurPos = Position.MoveTowards(curJob.itemCurPos, curJob.endConnection.position, 1);

                    if (curJob.itemCurPos == curJob.endConnection.position) {
                        curJob.itemDir = curJob.endConnection.direction;
                    }

                    // movement complete, try inserting to the slot. If cannot insert just stay here.
                } else {
                    if (curJob.endConnection.TryInsertItem(curJob.movingItem)) {
                        curJob.isMovingItem = false;
                    }
                }
            }
        }
    }

    static bool ConnectorTryToPlaceItem(Connector connector, Item curItem, out Connector.Connection target, Connector.Connection source) {
        // Move over every possible output source once to try to find an empty slot
        for (int i = 0; i < connector.outputs.Count; i++) {
            var n = (i + connector.outputCounter) % connector.outputs.Count;
                // Try to insert to a belt or a building
                if (connector.outputs[n] != source && connector.outputs[n].CheckInsertItem(curItem)) {
                    connector.outputCounter = n + 1;
                    target = connector.outputs[n];
                    return true;
                }
        }

        target = null;
        return false;
    }
    
    
    
    
    
    
    
    
    
    //-------------------------------------------------------------------------------------------Creation/deletion
    
    public Building CreateBuilding(BuildingData buildingData, List<Position> myPositions, bool isInventory, List<InventoryItemSlot> inventory) {
        var building = CreateBuilding(buildingData, myPositions);
        
        if (isInventory) {
            building.invController.SetInventory(inventory);
        }
        
        return building;
    }
    public Building CreateBuilding(BuildingData buildingData, List<Position> myPositions) {
        var building = new Building(myPositions[0], buildingData);
        building.myPositions = myPositions;
        
        foreach (Position position in building.myPositions) {
            var myTile = Grid.s.GetTile(position);
            if (myTile != null)
                myTile.myBuilding = building;
        }
        
        buildings.Add(building);
        
        UpdateBuildingNearConnectors(building);
        return building;
    }


    public void RemoveBuilding(Building building) {
        if (building == null)
            return;
        
        foreach (Position myPosition in building.myPositions) {
            var myTile = Grid.s.GetTile(myPosition);
            if (myTile != null)
                myTile.myBuilding = null;
        }
        
        buildings.Remove(building);
        UpdateBuildingNearConnectors(building);
    }


    Belt ExtendBeltFromStart(Belt belttoextend) {
        if (belttoextend.items[0].item.isEmpty()) {
            belttoextend.items[0] = new Belt.BeltSegment() {count = belttoextend.items[0].count + SlotPerSegment, item = belttoextend.items[0].item};
            belttoextend.startPos = belttoextend.startPos - Position.GetCardinalDirection(belttoextend.direction);
        } else {
            belttoextend.items.Insert(0, new Belt.BeltSegment() {count = SlotPerSegment, item = Item.GetEmpty()});
            belttoextend.startPos =  belttoextend.startPos - Position.GetCardinalDirection(belttoextend.direction);
        }

        return belttoextend;
    }
    Belt ExtendBeltFromEnd(Belt belttoextend) {
        var lastIndex = belttoextend.items.Count - 1;
        if (belttoextend.items[lastIndex].item.isEmpty()) {
            belttoextend.items[lastIndex] = new Belt.BeltSegment() {count = belttoextend.items[lastIndex].count + SlotPerSegment, item = belttoextend.items[lastIndex].item};
            belttoextend.endPos = belttoextend.endPos + Position.GetCardinalDirection(belttoextend.direction);
        } else {
            belttoextend.items.Add( new Belt.BeltSegment() {count = SlotPerSegment, item = Item.GetEmpty()});
            belttoextend.endPos = belttoextend.endPos + Position.GetCardinalDirection(belttoextend.direction);
        }

        return belttoextend;
    }

    bool DoBeltsLineOnALine(Belt backBelt, Belt frontBelt, Position location, int direction) {
        if (backBelt.direction != frontBelt.direction) {
            return false;
        } else if (backBelt.direction != direction) {
            return false;
        } else if (Position.MoveTowards(backBelt.endPos, frontBelt.startPos, 2) != frontBelt.startPos) {
            return false;
        } else if (Position.MoveTowards(backBelt.endPos, frontBelt.startPos, 1) != location) {
            return false;
        } else {
            return true;
        }
    }

    /// <summary>
    /// Creates a sim belt at the specified location. Automatically handles belt connections
    /// Assumes there is no overlap at the location
    /// </summary>
    /// <param name="location"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Belt CreateBelt(Position location, int direction) {
        if (direction == 0) {
            Debug.LogError("Trying to create a belt without direction. This is not allowed! " + location);
            return null;
        }

        var forwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, 1));
        var backwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, -1));

        if (forwardTile.isEmpty && backwardTile.isEmpty) {
            var belt = new Belt(location, location, direction);
            Grid.s.GetTile(location).myBelt = belt;
            belts.Add(belt);
            UpdateBeltNearConnectors(belt);
            return belt;


            // We need to do very special merge if both forward and back tiles are belts
            // AND they are in the same direction as the belt we are trying to place
        }else if ((forwardTile.areThereBelt && backwardTile.areThereBelt)
                  && (DoBeltsLineOnALine(backwardTile.myBelt, forwardTile.myBelt, location, direction))) {

            var firstBelt = backwardTile.myBelt;
            var secondBelt = forwardTile.myBelt;

            var belt = new Belt(firstBelt.startPos, secondBelt.endPos, direction);
            belt.items = new List<Belt.BeltSegment>();

            for (int i = 0; i < firstBelt.items.Count; i++) {
                belt.items.Add(firstBelt.items[i]);
            }

            if (belt.items[belt.items.Count - 1].item.isEmpty()) {
                belt.items[belt.items.Count - 1].AddCount(SlotPerSegment);
            } else {
                belt.items.Add(new Belt.BeltSegment() {count = SlotPerSegment, item = Item.GetEmpty()});
            }

            if (belt.items[belt.items.Count - 1].item == secondBelt.items[0].item) {
                belt.items[belt.items.Count - 1].AddCount(secondBelt.items[0].count);
            } else {
                belt.items.Add(secondBelt.items[0]);
            }

            for (int i = 1; i < secondBelt.items.Count; i++) {
                belt.items.Add(secondBelt.items[i]);
            }

            for (int i = 0; i < belt.length; i++) {
                Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).myBelt = belt;
            }

            belts.Remove(firstBelt);
            belts.Remove(secondBelt);
            belts.Add(belt);
            UpdateBeltNearConnectors(belt);
            return belt;

        } else {

            var tiles = new TileData[] {forwardTile, backwardTile};
            Belt FinalBelt = null;

            foreach (var tile in tiles) {
                if (tile.areThereBelt) {
                    var existingBelt = tile.myBelt;
                    if (existingBelt.direction == direction || existingBelt.direction == 0) {
                        if (Position.MoveCardinalDirection(location, direction, 1) == existingBelt.startPos) {
                            FinalBelt = ExtendBeltFromStart(existingBelt);
                            FinalBelt.direction = Position.CardinalDirection(FinalBelt.startPos, FinalBelt.endPos);
                        } else if(Position.MoveCardinalDirection(location, direction, -1) == existingBelt.endPos) {
                            FinalBelt = ExtendBeltFromEnd(existingBelt);
                            FinalBelt.direction = Position.CardinalDirection(FinalBelt.startPos, FinalBelt.endPos);
                        } else {
                            // This means even though they are pointing in the same direction, the new belt is not at the end/start point!
                            // Eg they are running in parallel
                        }
                    } 
                }
            }

            if (FinalBelt == null) {
                FinalBelt = new Belt(location, location, direction);
                belts.Add(FinalBelt);
            }
            
            Grid.s.GetTile(location).myBelt = FinalBelt;

            UpdateBeltNearConnectors(FinalBelt);
            return FinalBelt;
        } 
    }

    void UpdateBeltNearConnectors(Belt belt) {
        var tileFront = Grid.s.GetTile(Position.MoveCardinalDirection(belt.startPos, belt.direction, -1));
        if (tileFront.areThereConnector) {
            UpdateConnectorConnections(tileFront.myConnector);
        }
        
        var tileBack = Grid.s.GetTile(Position.MoveCardinalDirection(belt.endPos, belt.direction, 1));
        if (tileBack.areThereConnector) {
            UpdateConnectorConnections(tileBack.myConnector);
        }
    }

    void UpdateBuildingNearConnectors(Building building) {
        foreach (Position myPosition in building.myPositions) {
            for (int i = 1; i <= 4; i++) {
                var tile = Grid.s.GetTile(Position.MoveCardinalDirection(myPosition, i, 1));
                if (tile.areThereConnector) {
                    UpdateConnectorConnections(tile.myConnector);
                }
            }
        }
    }

    public void RemoveBelt(Position location) {
        if(!Grid.s.GetTile(location).areThereBelt)
            return;
        var belt = Grid.s.GetTile(location).myBelt;
        // When removing belt, we either remove the first slot, the last slot, or split from the middle.
        // If at the end of these operations we end up with a belt that is of length 1, then we delete it.
        if (location == belt.startPos) {
            if (belt.length - 1 <= 1) {
                belts.Remove(belt);

                for (int i = 0; i < belt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).myBelt = null;
                }

                UpdateBeltNearConnectors(belt);
                
                return;
            }
            
            belt.startPos = Position.MoveTowards(belt.startPos, belt.endPos, 1);
            var firstIndex = 0;
            var slotsRemaining = SlotPerSegment;
            while (belt.items[firstIndex].count <= slotsRemaining) {
                slotsRemaining -= belt.items[firstIndex].count;
                belt.items.RemoveAt(firstIndex);
            }
            if(slotsRemaining > 0){
                belt.items[firstIndex] = new Belt.BeltSegment(){count = belt.items[firstIndex].count - slotsRemaining , item =  belt.items[firstIndex].item};
            }
            
            Grid.s.GetTile(location).myBelt = null;
            UpdateBeltNearConnectors(belt);

        }else if (location == belt.endPos) {
            if (belt.length - 1 <= 1) {
                belts.Remove(belt);

                for (int i = 0; i < belt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).myBelt = null;
                }
                
                UpdateBeltNearConnectors(belt);
                
                return;
            }
            
            belt.endPos = Position.MoveTowards(belt.endPos, belt.startPos, 1);
            var lastIndex = belt.items.Count-1;
            var slotsRemaining = SlotPerSegment;
            while (belt.items[lastIndex].count <= slotsRemaining) {
                slotsRemaining -= belt.items[lastIndex].count;
                belt.items.RemoveAt(lastIndex);
                lastIndex -= 1;
            }
            if(slotsRemaining > 0){
                belt.items[lastIndex] = new Belt.BeltSegment(){count = belt.items[lastIndex].count - slotsRemaining , item =  belt.items[lastIndex].item};
            }
            
            Grid.s.GetTile(location).myBelt = null;
            UpdateBeltNearConnectors(belt);
            
        } else {
            var firstBelt = new Belt(belt.startPos, Position.MoveTowards(location, belt.startPos, 1));
            var secondBelt = new Belt(Position.MoveTowards(location, belt.endPos, 1), belt.endPos);
            firstBelt.items = new List<Belt.BeltSegment>();
            secondBelt.items = new List<Belt.BeltSegment>();

            // Decompress belt contents
            var decompressedItems = new Item[belt.length*SlotPerSegment];

            int count = 0;
            for (int i = 0; i < belt.items.Count; i++) {
                for (int n = 0; n < belt.items[i].count; n++) {
                    decompressedItems[count] = belt.items[i].item;
                    count += 1;
                }
            }
            
            var firstBeltEndPoint = firstBelt.length * SlotPerSegment;
            var secondBeltStartPoint = firstBelt.length * SlotPerSegment + SlotPerSegment;
            
            //Put items in the first belt
            firstBelt.items.Add(new Belt.BeltSegment(){count = 1, item =  decompressedItems[0]});
            for (int i = 1; i < firstBeltEndPoint; i++) {
                if (firstBelt.items[firstBelt.items.Count - 1].item == decompressedItems[i]) {
                    firstBelt.items[firstBelt.items.Count-1].AddCount(1);
                } else {
                    firstBelt.items.Add(new Belt.BeltSegment(){item = decompressedItems[i], count = 1});
                }
            }
            
            //Put items in the second belt
            secondBelt.items.Add(new Belt.BeltSegment(){count = 1, item =  decompressedItems[secondBeltStartPoint]});
            for (int i = secondBeltStartPoint+1; i < decompressedItems.Length; i++) {
                if (secondBelt.items[secondBelt.items.Count - 1].item == decompressedItems[i]) {
                    secondBelt.items[secondBelt.items.Count-1].AddCount(1);
                } else {
                    secondBelt.items.Add(new Belt.BeltSegment(){item = decompressedItems[i], count = 1});
                }
            }

            if (firstBelt.length > 1) {
                for (int i = 0; i < firstBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(firstBelt.startPos, firstBelt.endPos, i)).myBelt = firstBelt;
                }

                belts.Add(firstBelt);
            } else {
                for (int i = 0; i < firstBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(firstBelt.startPos, firstBelt.endPos, i)).myBelt = null;
                }
            }

            if (secondBelt.length > 1) {
                for (int i = 0; i < secondBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(secondBelt.startPos, secondBelt.endPos, i)).myBelt = secondBelt;
                }
                
                if (Grid.s.GetTile(Position.MoveCardinalDirection(secondBelt.endPos, secondBelt.direction, 1)).areThereConnector) {
                    UpdateConnectorConnections(Grid.s.GetTile(Position.MoveCardinalDirection(secondBelt.endPos, secondBelt.direction, 1)).myConnector);
                }
                
                belts.Add(secondBelt);
            }else {
                for (int i = 0; i < secondBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(secondBelt.startPos, secondBelt.endPos, i)).myBelt = null;
                }
            }
            belts.Remove(belt);
            Grid.s.GetTile(location).myBelt = null;
            UpdateBeltNearConnectors(belt);
        }
    }
    
    bool DoConnectorsLineOnALine(Connector backConnector, Connector frontConnector, Position location, int direction) {
        if (backConnector.direction == 0 && frontConnector.direction == 0 ) {
            if (direction == 0) {
                return true;
            }else if (direction == Position.ParallelDirection(backConnector.endPos, frontConnector.startPos)) {
                return true;
            } else {
                return false;
            }
        } else {
            var parallelDirection = Position.ParallelDirection(backConnector.endPos, frontConnector.startPos);
            if ((direction == 0 || direction == parallelDirection)
                && (backConnector.direction == 0 || backConnector.direction == parallelDirection)
                && (frontConnector.direction == 0 || frontConnector.direction == parallelDirection)) {
                return true;
            } else {
                return false;
            }
        }
    }

    public Connector CreateConnector(Position location, int direction) {
        if (direction != 0) {
            var forwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, 1));
            var backwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, -1));

            if (forwardTile.isEmpty && backwardTile.isEmpty) {
                var connector = new Connector(location, location, direction);
                connectors.Add(connector);
                Grid.s.GetTile(location).myConnector = connector;
                UpdateConnectorConnections(connector);
                return connector;


                // We need to do very special merge if both forward and back tiles are belts
                // AND they are in the same direction as the belt we are trying to place
            } else if ((forwardTile.areThereConnector && backwardTile.areThereConnector)
                       && DoConnectorsLineOnALine(backwardTile.myConnector, forwardTile.myConnector, location, direction)) {

                var firstConnector = backwardTile.myConnector;
                var secondConnector = forwardTile.myConnector;

                var connector = new Connector(firstConnector.startPos, secondConnector.endPos, direction);

                connectors.Remove(firstConnector);
                connectors.Remove(secondConnector);
                connectors.Add(connector);

                for (int i = 0; i < connector.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(connector.startPos, connector.endPos, i)).myConnector = connector;
                }

                UpdateConnectorConnections(connector);
                return connector;
            } else {

                var tiles = new TileData[] {forwardTile, backwardTile};
                Connector finalConnector = null;

                foreach (var tile in tiles) {
                    if (tile.areThereConnector) {
                        var existingConnector = tile.myConnector;
                        if (existingConnector.direction == direction || existingConnector.direction == 0 || direction == 0) {
                            if (Position.Distance(location, existingConnector.startPos) == 1) {
                                finalConnector = new Connector(location, existingConnector.endPos);
                                connectors.Remove(existingConnector);
                                connectors.Add(finalConnector);
                                for (int i = 0; i < finalConnector.length; i++) {
                                    Grid.s.GetTile(Position.MoveTowards(finalConnector.startPos, finalConnector.endPos, i)).myConnector = finalConnector;
                                }
                            } else if (Position.Distance(location, existingConnector.endPos) == 1) {
                                finalConnector = new Connector(existingConnector.startPos, location);
                                connectors.Remove(existingConnector);
                                connectors.Add(finalConnector);
                                for (int i = 0; i < finalConnector.length; i++) {
                                    Grid.s.GetTile(Position.MoveTowards(finalConnector.startPos, finalConnector.endPos, i)).myConnector = finalConnector;
                                }
                            } else {
                                // This can happen when there is a connector nearby, but they dont align
                            }
                        }
                    }
                }

                if (finalConnector == null) {
                    finalConnector = new Connector(location, location, direction);
                    connectors.Add(finalConnector);
                    Grid.s.GetTile(location).myConnector = finalConnector;
                }

                UpdateConnectorConnections(finalConnector);
                return finalConnector;
            }
        } else {
            // If direction is 0, then we try to connect it to a nearby connector if we can. We do this by aligning the direction then doing the placement normally.
            for (int i = 1; i <= 4; i++) {
                var tile = Grid.s.GetTile(Position.MoveCardinalDirection(location, i, 1));
                if (tile.areThereConnector) {
                    if (tile.myConnector.direction != 0) {
                        return CreateConnector(location, tile.myConnector.direction);
                    } else {
                        return CreateConnector(location, Position.ParallelDirection(location, tile.position));
                    }
                }
            }
            
            // If we reached here it means there are no nearby connectors, hence we just put down our connector
            var connector = new Connector(location, location, direction);
            connectors.Add(connector);
            Grid.s.GetTile(location).myConnector = connector;
            UpdateConnectorConnections(connector);
            return connector;
        }
    }

    void UpdateConnectorConnections(Connector connector) {
        connector.inputs = new List<Connector.Connection>();
        connector.inputCounter = 0;
        connector.outputs = new List<Connector.Connection>();
        connector.outputCounter = 0;

        for (int i = 0; i < connector.length; i++) {
            for (int dir = 1; dir <= 4; dir++) {
                var curPos = Position.MoveTowards(connector.startPos, connector.endPos, i);
                var nextTile = Grid.s.GetTile(Position.MoveCardinalDirection(curPos, dir, 1));

                if (!nextTile.isEmpty) {
                    if (nextTile.areThereBelt) {
                        if (Position.MoveCardinalDirection(nextTile.myBelt.endPos, nextTile.myBelt.CardinalDirection(), 1) == curPos) {
                            connector.inputs.Add(new Connector.Connection(nextTile.myBelt, curPos, dir));
                        } else if (Position.MoveCardinalDirection(nextTile.myBelt.startPos, nextTile.myBelt.CardinalDirection(), -1) == curPos) {
                            connector.outputs.Add(new Connector.Connection(nextTile.myBelt, curPos, dir));
                        }
                    }

                    if (nextTile.areThereBuilding) {
                        bool connectionExists = false;
                        for (int k = 0; k < connector.inputs.Count; k++) {
                            if (!connector.inputs[k].isBeltConnection) {
                                if (connector.inputs[k].building == nextTile.myBuilding) {
                                    connectionExists = true;
                                    break;
                                }
                            }
                        }

                        if (!connectionExists) {
                            connector.inputs.Add(new Connector.Connection(nextTile.myBuilding, curPos, dir));
                            connector.outputs.Add(new Connector.Connection(nextTile.myBuilding, curPos, dir));
                        }
                    }
                }
            }
        }
        
        connector.ConnectorInputsUpdatedCallback?.Invoke();
    }

    public void RemoveConnector(Position location) {
        if(!Grid.s.GetTile(location).areThereConnector)
            return;
        var connector = Grid.s.GetTile(location).myConnector;
        // When removing connector, we either remove the entire one length connector, the first one, the last one, or split from the middle.
        if (connector.length == 1) {
            connectors.Remove(connector);
            Grid.s.GetTile(location).myConnector = null;
            
        }else if (location == connector.startPos) {
            connector.startPos = Position.MoveTowards(connector.startPos, connector.endPos, 1);
            
            UpdateConnectorConnections(connector);

            Grid.s.GetTile(location).myConnector = null;

        }else if (location == connector.endPos) {
            connector.endPos = Position.MoveTowards(connector.endPos, connector.startPos, 1);
            
            UpdateConnectorConnections(connector);

            Grid.s.GetTile(location).myConnector = null;
            
        } else {
            var firstConnector = new Connector(connector.startPos, Position.MoveTowards(location, connector.startPos, 1));
            var secondConnector = new Connector(Position.MoveTowards(location, connector.endPos, 1), connector.endPos);
            
            for (int i = 0; i < firstConnector.length; i++) {
                Grid.s.GetTile(Position.MoveTowards(firstConnector.startPos, firstConnector.endPos, i)).myConnector = firstConnector;
            }

            for (int i = 0; i < secondConnector.length; i++) {
                Grid.s.GetTile(Position.MoveTowards(secondConnector.startPos, secondConnector.endPos, i)).myConnector = secondConnector;
            }
            
            
            connectors.Remove(connector);
            Grid.s.GetTile(location).myConnector = null;
            
            connectors.Add(firstConnector);
            connectors.Add(secondConnector);

            UpdateConnectorConnections(firstConnector);
            UpdateConnectorConnections(secondConnector);
        }
    }
}








//------------------------------------------------------------------------------------------------------------ Helper Classes

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
        
        items.Add(new BeltSegment(){count = length*FactorySystem.SlotPerSegment, item = Item.GetEmpty()});
    }

    public Belt(Position startPos, Position endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.direction = Position.CardinalDirection(startPos,endPos);
        
        items.Add(new BeltSegment(){count = length*FactorySystem.SlotPerSegment, item = Item.GetEmpty()});
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
        
        CreateItemTransportJobs(FactorySystem.ConnectorTransporterCount);
    }

    public Connector(Position startPos, Position endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.direction = Position.ParallelDirection(startPos,endPos);

        CreateItemTransportJobs(FactorySystem.ConnectorTransporterCount);
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
    public BuildingData buildingData;

    public List<Position> myPositions = new List<Position>(); 

    public BuildingInventoryController invController = new BuildingInventoryController();
    public BuildingCraftingController craftController = new BuildingCraftingController();

    public Building(Position position, BuildingData _buildingData) {
        if (_buildingData == null) {
            Debug.Log("Trying to create a building with null data!");
        } else {
            buildingData = _buildingData;
            craftController.SetUp(buildingData, invController);
            invController.SetUp(position, craftController, buildingData);
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


    public float UpdateCraftingProcess(float efficiency) {
        return craftController.UpdateCraftingProcess(efficiency);
    }
}