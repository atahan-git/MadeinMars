using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BeltCompressionUtility;

public static class FactoryBuilder {

    public static GenericCallback ObjectsUpdated;
    public static GenericCallback DronesUpdated;
    public static ShipCardAddedCallback shipCardAdded;
    
    public static Drone CreateDrone(Position location) {
        var drone = new Drone(location);
        FactoryMaster.s.AddDrone(drone);
        DronesUpdated?.Invoke();
        return drone;
    }

    public static void CreateShipPart(BuildingData myData, Position location) {
        var shipCard = DataHolder.s.BuildingDataToShipCard(myData);
        
        DataSaver.s.mySave.availableShipCards.Add(new DataSaver.ShipCardData(shipCard));
        
        shipCardAdded?.Invoke(shipCard, location);
    }
    
    public static Construction StartConstruction(BuildingData buildingData, int direction, Position location, List<InventoryItemSlot> afterConstructionInventory = null) {
        var construction = new Construction(buildingData, direction, location,  GetRequirements(buildingData,false), afterConstructionInventory, true);
        FactoryMaster.s.AddConstruction(construction);
        
        foreach (var loc in construction.locations) {
            Grid.s.GetTile(loc).simObject = construction;
        }
        
        ObjectsUpdated?.Invoke();
        return construction;
    }
    
    static List<InventoryItemSlot> GetRequirements(BuildingData myDat, bool isFilled) {
        CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(BuildingData.ItemType.Building);
        var reqs = new List<InventoryItemSlot>();
        if (ps != null) {
            for (int i = 0; i < ps.Length; i++) {
                if (DataHolder.s.GetConnections(ps[i], false)[0].itemUniqueName == myDat.uniqueName) {
                    for (int m = 0; m < DataHolder.s.GetConnections(ps[i], true).Count; m++) {
                        var count = DataHolder.s.GetConnections(ps[i], true)[m].count;
                        reqs.Add(new InventoryItemSlot(DataHolder.s.GetItem(DataHolder.s.GetConnections(ps[i], true)[m].itemUniqueName),
                            isFilled? count : 0, count, 
                            InventoryItemSlot.SlotType.storage)
                        );
                    }
                    return reqs;
                }
            }
        }

        return reqs;
    }

    public static void CompleteConstruction(Construction construction) {
        FactoryMaster.s.RemoveConstruction(construction);
        
        foreach (var loc in construction.locations) {
            Grid.s.GetTile(loc).simObject = null;
        }
        
        switch (construction.myData.myType) {
            case BuildingData.ItemType.Belt:
                CreateBelt(construction.locations[0], construction.direction, construction.afterConstructionInventory);
                break;
            case BuildingData.ItemType.Connector:
                CreateConnector(construction.locations[0], construction.direction);
                break;
            case BuildingData.ItemType.ShipCard:
                CreateShipPart(construction.myData, construction.center);
                break;
            default:
                CreateBuilding(construction.myData, construction.center, construction.afterConstructionInventory);
                break;
        }
        

        ObjectsUpdated?.Invoke();
    }


    public static void StartDeconstruction(Position location) {
        var myTile = Grid.s.GetTile(location);

        if (myTile.simObject is Construction construction) {
            
            if (construction.isConstruction) {
                construction.isConstruction = false;

                if (construction.constructionInventory.GetTotalAmountOfItems() + construction.afterConstructionInventory.GetTotalAmountOfItems() <= 0) {
                    CompleteDeconstruction(construction.center);
                } else {
                    FactoryDrones.CancelConstructionMidTask(construction);
                }
            }
            
        } else {
            int direction = 0;
            BuildingData buildingData = null;
            List<InventoryItemSlot> afterConstructionInventory = null;
            bool canBeDestroyed = true;
            if (myTile.simObject is Belt belt) {
                buildingData = FactoryMaster.s.beltBuildingData;
                direction = belt.direction;
                afterConstructionInventory = RemoveBelt(location);
                for (int i = afterConstructionInventory.Count-1; i >=0; i--) {
                    if (afterConstructionInventory[i].myItem.isEmpty()) {
                        afterConstructionInventory.RemoveAt(i);
                    }
                }
            } else if (myTile.simObject is Connector connector) {
                buildingData = FactoryMaster.s.connectorBuildingData;
                direction = connector.direction;
                RemoveConnector(location);
            } else if (myTile.simObject is Building building) {
                buildingData = building.buildingData;
                canBeDestroyed = building.isDestructable;
                location = building.center;
                afterConstructionInventory = RemoveBuilding(location);
            }

            if (canBeDestroyed) {
                if (buildingData != null) {
                    var newConstruction = new Construction(buildingData, direction, location, GetRequirements(buildingData, true), afterConstructionInventory, false);
                    FactoryMaster.s.AddConstruction(newConstruction);

                    foreach (var loc in newConstruction.locations) {
                        Grid.s.GetTile(loc).simObject = newConstruction;
                    }
                }
            }
        }

        ObjectsUpdated?.Invoke();
    }

    /// <summary>
    /// Remove construction after deconstruction is done
    /// </summary>
    /// <param name="location"></param>
    public static void CompleteDeconstruction(Position location) {
        if(!(Grid.s.GetTile(location).simObject is Construction))
            return;
        var construction = Grid.s.GetTile(location).simObject as Construction;
        
        FactoryMaster.s.RemoveConstruction(construction);
        
        foreach (var loc in construction.locations) {
            Grid.s.GetTile(loc).simObject = null;
        }
        
        
        ObjectsUpdated?.Invoke();
    }
    
    static bool DoConnectorsLineOnALine(Connector backConnector, Connector frontConnector, Position location, int direction) {
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
    

    public static Connector CreateConnector(Position location, int direction) {
        if (direction != 0) {
            var forwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, 1));
            var backwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, -1));

            if (forwardTile.isEmpty && backwardTile.isEmpty) {
                var connector = new Connector(location, location, direction);
                FactoryMaster.s.AddConnector(connector);
                Grid.s.GetTile(location).simObject = connector;
                UpdateConnectorConnections(connector);
                ObjectsUpdated?.Invoke();
                return connector;


                // We need to do very special merge if both forward and back tiles are belts
                // AND they are in the same direction as the belt we are trying to place
            } else if (( backwardTile.simObject is Connector firstConnector && forwardTile.simObject is Connector secondConnector)
                       && DoConnectorsLineOnALine(firstConnector, secondConnector, location, direction)) {


                var connector = new Connector(firstConnector.startPos, secondConnector.endPos, direction);

                FactoryMaster.s.RemoveConnector(firstConnector);
                FactoryMaster.s.RemoveConnector(secondConnector);
                FactoryMaster.s.AddConnector(connector);

                for (int i = 0; i < connector.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(connector.startPos, connector.endPos, i)).simObject = connector;
                }

                UpdateConnectorConnections(connector);
                ObjectsUpdated?.Invoke();
                return connector;
            } else {

                var tiles = new TileData[] {forwardTile, backwardTile};
                Connector finalConnector = null;

                foreach (var tile in tiles) {
                    if (tile.simObject is Connector existingConnector) {
                        if (existingConnector.direction == direction || existingConnector.direction == 0 || direction == 0) {
                            if (Position.Distance(location, existingConnector.startPos) == 1) {
                                finalConnector = new Connector(location, existingConnector.endPos);
                                FactoryMaster.s.RemoveConnector(existingConnector);
                                FactoryMaster.s.AddConnector(finalConnector);
                                for (int i = 0; i < finalConnector.length; i++) {
                                    Grid.s.GetTile(Position.MoveTowards(finalConnector.startPos, finalConnector.endPos, i)).simObject = finalConnector;
                                }
                            } else if (Position.Distance(location, existingConnector.endPos) == 1) {
                                finalConnector = new Connector(existingConnector.startPos, location);
                                FactoryMaster.s.RemoveConnector(existingConnector);
                                FactoryMaster.s.AddConnector(finalConnector);
                                for (int i = 0; i < finalConnector.length; i++) {
                                    Grid.s.GetTile(Position.MoveTowards(finalConnector.startPos, finalConnector.endPos, i)).simObject = finalConnector;
                                }
                            } else {
                                // This can happen when there is a connector nearby, but they dont align
                            }
                        }
                    }
                }

                if (finalConnector == null) {
                    finalConnector = new Connector(location, location, direction);
                    FactoryMaster.s.AddConnector(finalConnector);
                    Grid.s.GetTile(location).simObject = finalConnector;
                }

                UpdateConnectorConnections(finalConnector);
                ObjectsUpdated?.Invoke();
                return finalConnector;
            }
        } else {
            // If direction is 0, then we try to connect it to a nearby connector if we can. We do this by aligning the direction then doing the placement normally.
            for (int i = 1; i <= 4; i++) {
                var tile = Grid.s.GetTile(Position.MoveCardinalDirection(location, i, 1));
                if (tile.simObject is Connector connector) {
                    if (connector.direction != 0) {
                        ObjectsUpdated?.Invoke();
                        return CreateConnector(location, connector.direction);
                    } else {
                        ObjectsUpdated?.Invoke();
                        return CreateConnector(location, Position.ParallelDirection(location, tile.location));
                    }
                }
            }
            
            // If we reached here it means there are no nearby connectors, hence we just put down our connector
            var newConnector = new Connector(location, location, direction);
            FactoryMaster.s.AddConnector(newConnector);
            Grid.s.GetTile(location).simObject = newConnector;
            UpdateConnectorConnections(newConnector);
            ObjectsUpdated?.Invoke();
            return newConnector;
        }
    }

    static void UpdateConnectorConnections(Connector connector) {
        connector.inputs.Clear();
        connector.inputCounter = 0;
        connector.outputs.Clear();
        connector.outputCounter = 0;

        for (int i = 0; i < connector.length; i++) {
            for (int dir = 1; dir <= 4; dir++) {
                var curPos = Position.MoveTowards(connector.startPos, connector.endPos, i);
                var nextTile = Grid.s.GetTile(Position.MoveCardinalDirection(curPos, dir, 1));

                if (!nextTile.isEmpty) {
                    if (nextTile.simObject is Belt nextTileBelt) {
                        if (Position.MoveCardinalDirection(nextTileBelt.endPos, nextTileBelt.CardinalDirection(), 1) == curPos) {
                            connector.inputs.Add(new Connector.Connection(nextTileBelt, curPos, dir));
                        } else if (Position.MoveCardinalDirection(nextTileBelt.startPos, nextTileBelt.CardinalDirection(), -1) == curPos) {
                            connector.outputs.Add(new Connector.Connection(nextTileBelt, curPos, dir));
                        }
                    }

                    if (nextTile.simObject is Building nextTileBuilding) {
                        bool connectionExists = false;
                        for (int k = 0; k < connector.inputs.Count; k++) {
                            if (!connector.inputs[k].isBeltConnection) {
                                if (connector.inputs[k].building == nextTileBuilding) {
                                    connectionExists = true;
                                    break;
                                }
                            }
                        }

                        if (!connectionExists) {
                            connector.inputs.Add(new Connector.Connection(nextTileBuilding, curPos, dir));
                            connector.outputs.Add(new Connector.Connection(nextTileBuilding, curPos, dir));
                        }
                    }
                }
            }
        }
        
        connector.ConnectorInputsUpdatedCallback?.Invoke();
    }

    public static void RemoveConnector(Position location) {
        if(!(Grid.s.GetTile(location).simObject is Connector connector))
            return;
        // When removing connector, we either remove the entire one length connector, the first one, the last one, or split from the middle.
        if (connector.length == 1) {
            FactoryMaster.s.RemoveConnector(connector);
            Grid.s.GetTile(location).simObject = null;
            
        }else if (location == connector.startPos) {
            connector.startPos = Position.MoveTowards(connector.startPos, connector.endPos, 1);
            
            UpdateConnectorConnections(connector);

            Grid.s.GetTile(location).simObject = null;

        }else if (location == connector.endPos) {
            connector.endPos = Position.MoveTowards(connector.endPos, connector.startPos, 1);
            
            UpdateConnectorConnections(connector);

            Grid.s.GetTile(location).simObject = null;
            
        } else {
            var firstConnector = new Connector(connector.startPos, Position.MoveTowards(location, connector.startPos, 1));
            var secondConnector = new Connector(Position.MoveTowards(location, connector.endPos, 1), connector.endPos);
            
            for (int i = 0; i < firstConnector.length; i++) {
                Grid.s.GetTile(Position.MoveTowards(firstConnector.startPos, firstConnector.endPos, i)).simObject = firstConnector;
            }

            for (int i = 0; i < secondConnector.length; i++) {
                Grid.s.GetTile(Position.MoveTowards(secondConnector.startPos, secondConnector.endPos, i)).simObject = secondConnector;
            }
            
            
            FactoryMaster.s.RemoveConnector(connector);
            Grid.s.GetTile(location).simObject = null;
            
            FactoryMaster.s.AddConnector(firstConnector);
            FactoryMaster.s.AddConnector(secondConnector);

            UpdateConnectorConnections(firstConnector);
            UpdateConnectorConnections(secondConnector);
        }
        ObjectsUpdated?.Invoke();
    }
    
    
    
    static Belt ExtendBeltFromStart(Belt belttoextend) {
        if (belttoextend.items[0].item.isEmpty()) {
            belttoextend.items[0] = new Belt.BeltSegment() {count = belttoextend.items[0].count + FactoryMaster.SlotPerSegment, item = belttoextend.items[0].item};
            belttoextend.startPos = belttoextend.startPos - Position.GetCardinalDirection(belttoextend.direction);
        } else {
            belttoextend.items.Insert(0, new Belt.BeltSegment() {count = FactoryMaster.SlotPerSegment, item = Item.GetEmpty()});
            belttoextend.startPos = belttoextend.startPos - Position.GetCardinalDirection(belttoextend.direction);
        }

        return belttoextend;
    }

    static Belt ExtendBeltFromEnd(Belt belttoextend) {
        var lastIndex = belttoextend.items.Count - 1;
        if (belttoextend.items[lastIndex].item.isEmpty()) {
            belttoextend.items[lastIndex] = new Belt.BeltSegment() {count = belttoextend.items[lastIndex].count + FactoryMaster.SlotPerSegment, item = belttoextend.items[lastIndex].item};
            belttoextend.endPos = belttoextend.endPos + Position.GetCardinalDirection(belttoextend.direction);
        } else {
            belttoextend.items.Add(new Belt.BeltSegment() {count = FactoryMaster.SlotPerSegment, item = Item.GetEmpty()});
            belttoextend.endPos = belttoextend.endPos + Position.GetCardinalDirection(belttoextend.direction);
        }

        return belttoextend;
    }

    static bool DoBeltsLineOnALine(Belt backBelt, Belt frontBelt, Position location, int direction) {
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

    public static Belt CreateBelt(Position location, int direction, List<InventoryItemSlot> afterConstructionSlots) {
        var belt = CreateBelt(location, direction);
        var offset = Position.Distance(belt.startPos, location) * FactoryMaster.SlotPerSegment;

        var items = DecompressBelt(belt.items);
        var n = 0;
        for (int i = offset; i < 4; i++) {
            if (afterConstructionSlots != null) {
                if (afterConstructionSlots.Count <= n) {
                    break;
                }
                
                items[i] = afterConstructionSlots[n].myItem;
                afterConstructionSlots[n].count -= 1;
                if (afterConstructionSlots[n].count <= 0) {
                    n++;
                }
            } else {
                break;
            }
        }

        belt.items = CompressItemsToBelt(items);

        ObjectsUpdated?.Invoke();
        return belt;
    }

    /// <summary>
    /// Creates a sim belt at the specified center. Automatically handles belt connections
    /// Assumes there is no overlap at the center
    /// </summary>
    /// <param name="location"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Belt CreateBelt(Position location, int direction) {
        if (direction == 0) {
            Debug.LogError("Trying to create a belt without direction. This is not allowed! " + location);
            return null;
        }

        var forwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, 1));
        var backwardTile = Grid.s.GetTile(Position.MoveCardinalDirection(location, direction, -1));

        if (forwardTile.isEmpty && backwardTile.isEmpty) {
            var belt = new Belt(location, location, direction);
            Grid.s.GetTile(location).simObject = belt;
            FactoryMaster.s.AddBelt(belt);
            UpdateBeltNearConnectors(belt);
            ObjectsUpdated?.Invoke();
            return belt;


            // We need to do very special merge if both forward and back tiles are belts
            // AND they are in the same direction as the belt we are trying to place
        } else if ((backwardTile.simObject is Belt firstBelt && forwardTile.simObject is Belt secondBelt)
                   && (DoBeltsLineOnALine(firstBelt, secondBelt, location, direction))) {

            var belt = new Belt(firstBelt.startPos, secondBelt.endPos, direction);
            belt.items.Clear();

            for (int i = 0; i < firstBelt.items.Count; i++) {
                belt.items.Add(firstBelt.items[i]);
            }

            if (belt.items[belt.items.Count - 1].item.isEmpty()) {
                belt.items[belt.items.Count - 1].AddCount(FactoryMaster.SlotPerSegment);
            } else {
                belt.items.Add(new Belt.BeltSegment() {count = FactoryMaster.SlotPerSegment, item = Item.GetEmpty()});
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
                Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).simObject = belt;
            }

            FactoryMaster.s.RemoveBelt(firstBelt);
            FactoryMaster.s.RemoveBelt(secondBelt);
            FactoryMaster.s.AddBelt(belt);
            UpdateBeltNearConnectors(belt);
            ObjectsUpdated?.Invoke();
            return belt;

        } else {

            var tiles = new TileData[] {forwardTile, backwardTile};
            Belt FinalBelt = null;

            foreach (var tile in tiles) {
                if (tile.simObject is Belt existingBelt) {
                    if (existingBelt.direction == direction || existingBelt.direction == 0) {
                        if (Position.MoveCardinalDirection(location, direction, 1) == existingBelt.startPos) {
                            FinalBelt = ExtendBeltFromStart(existingBelt);
                            FinalBelt.direction = Position.CardinalDirection(FinalBelt.startPos, FinalBelt.endPos);
                        } else if (Position.MoveCardinalDirection(location, direction, -1) == existingBelt.endPos) {
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
                FactoryMaster.s.AddBelt(FinalBelt);
            }

            Grid.s.GetTile(location).simObject = FinalBelt;

            UpdateBeltNearConnectors(FinalBelt);
            ObjectsUpdated?.Invoke();
            return FinalBelt;
        }
    }

    static void UpdateBeltNearConnectors(Belt belt) {
        var tileFront = Grid.s.GetTile(Position.MoveCardinalDirection(belt.startPos, belt.direction, -1));
        if (tileFront.simObject is Connector frontConnector) {
            UpdateConnectorConnections(frontConnector);
        }

        var tileBack = Grid.s.GetTile(Position.MoveCardinalDirection(belt.endPos, belt.direction, 1));
        if (tileBack.simObject is Connector backConnector) {
            UpdateConnectorConnections(backConnector);
        }
    }


    public static List<InventoryItemSlot> RemoveBelt(Position location) {
        if (!(Grid.s.GetTile(location).simObject is Belt belt))
            return null;
        // When removing belt, we either remove the first slot, the last slot, or split from the middle.
        // If at the end of these operations we end up with a belt that is of length 1, then we delete it.
        if (location == belt.startPos) {
            if (belt.length - 1 <= 0) {
                FactoryMaster.s.RemoveBelt(belt);

                for (int i = 0; i < belt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).simObject = null;
                }

                UpdateBeltNearConnectors(belt);

                ObjectsUpdated?.Invoke();
                return CompressItemsToInventorySlots(DecompressBelt(belt.items),0,1);
            }
            
            
            belt.startPos = Position.MoveTowards(belt.startPos, belt.endPos, 1);
            var removedItems = new List<InventoryItemSlot>();
            var firstIndex = 0;
            var slotsRemaining = FactoryMaster.SlotPerSegment;
            while (belt.items[firstIndex].count <= slotsRemaining) {
                slotsRemaining -= belt.items[firstIndex].count;
                removedItems.Add(new InventoryItemSlot(belt.items[firstIndex].item, belt.items[firstIndex].count, belt.items[firstIndex].count, InventoryItemSlot.SlotType.storage));
                belt.items.RemoveAt(firstIndex);
                if(slotsRemaining == 0)
                    break;
            }

            if (slotsRemaining > 0) {
                removedItems.Add(new InventoryItemSlot(belt.items[firstIndex].item, slotsRemaining, slotsRemaining, InventoryItemSlot.SlotType.storage));
                belt.items[firstIndex] = new Belt.BeltSegment() {count = belt.items[firstIndex].count - slotsRemaining, item = belt.items[firstIndex].item};
            }

            Grid.s.GetTile(location).simObject = null;
            UpdateBeltNearConnectors(belt);

            ObjectsUpdated?.Invoke();
            return removedItems;

        } else if (location == belt.endPos) {
            if (belt.length - 1 <= 0) {
                FactoryMaster.s.RemoveBelt(belt);

                for (int i = 0; i < belt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).simObject = null;
                }

                UpdateBeltNearConnectors(belt);

                ObjectsUpdated?.Invoke();
                return CompressItemsToInventorySlots(DecompressBelt(belt.items),0,1);
            }
            
            belt.endPos = Position.MoveTowards(belt.endPos, belt.startPos, 1);
            var removedItems = new List<InventoryItemSlot>();
            var lastIndex = belt.items.Count - 1;
            var slotsRemaining = FactoryMaster.SlotPerSegment;
            while (belt.items[lastIndex].count <= slotsRemaining) {
                slotsRemaining -= belt.items[lastIndex].count;
                removedItems.Add(new InventoryItemSlot(belt.items[lastIndex].item, belt.items[lastIndex].count, belt.items[lastIndex].count, InventoryItemSlot.SlotType.storage));
                belt.items.RemoveAt(lastIndex);
                lastIndex -= 1;
            }

            if (slotsRemaining > 0) {
                removedItems.Add(new InventoryItemSlot(belt.items[lastIndex].item, slotsRemaining, slotsRemaining, InventoryItemSlot.SlotType.storage));
                belt.items[lastIndex] = new Belt.BeltSegment() {count = belt.items[lastIndex].count - slotsRemaining, item = belt.items[lastIndex].item};
            }

            Grid.s.GetTile(location).simObject = null;
            UpdateBeltNearConnectors(belt);
            
            ObjectsUpdated?.Invoke();
            return removedItems;

        } else {
            var firstBelt = new Belt(belt.startPos, Position.MoveTowards(location, belt.startPos, 1));
            var secondBelt = new Belt(Position.MoveTowards(location, belt.endPos, 1), belt.endPos);
            firstBelt.items.Clear();
            secondBelt.items.Clear();

            // Decompress belt contents
            var decompressedItems = DecompressBelt(belt.items);


            if (firstBelt.length > 0) {
                //Put items in the first belt
                firstBelt.items = CompressItemsToBelt(decompressedItems, 0, firstBelt.length);
                
                for (int i = 0; i < firstBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(firstBelt.startPos, firstBelt.endPos, i)).simObject = firstBelt;
                }
                
                /*if (Grid.s.GetTile(Position.MoveCardinalDirection(firstBelt.startPos, firstBelt.direction, -1)).areThereConnector) {
                    UpdateConnectorConnections(Grid.s.GetTile(Position.MoveCardinalDirection(firstBelt.startPos, firstBelt.direction, -1)).myConnector);
                }*/

                FactoryMaster.s.AddBelt(firstBelt);
            } else {
                for (int i = 0; i < firstBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(firstBelt.startPos, firstBelt.endPos, i)).simObject = null;
                }
            }

            if (secondBelt.length > 0) {
                //Put items in the second belt
                secondBelt.items = CompressItemsToBelt(decompressedItems, firstBelt.length + 1, belt.length);
                
                
                for (int i = 0; i < secondBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(secondBelt.startPos, secondBelt.endPos, i)).simObject = secondBelt;
                }

                /*if (Grid.s.GetTile(Position.MoveCardinalDirection(secondBelt.endPos, secondBelt.direction, 1)).areThereConnector) {
                    UpdateConnectorConnections(Grid.s.GetTile(Position.MoveCardinalDirection(secondBelt.endPos, secondBelt.direction, 1)).myConnector);
                }*/

                FactoryMaster.s.AddBelt(secondBelt);
            } else {
                for (int i = 0; i < secondBelt.length; i++) {
                    Grid.s.GetTile(Position.MoveTowards(secondBelt.startPos, secondBelt.endPos, i)).simObject = null;
                }
            }


            FactoryMaster.s.RemoveBelt(belt);
            Grid.s.GetTile(location).simObject = null;
            UpdateBeltNearConnectors(belt);
            UpdateBeltNearConnectors(firstBelt);
            UpdateBeltNearConnectors(secondBelt);

            ObjectsUpdated?.Invoke();
            return CompressItemsToInventorySlots(decompressedItems, firstBelt.length, firstBelt.length+1);
        }
    }


    public static Building CreateBuilding(BuildingData buildingData, Position center, List<InventoryItemSlot> previousInventory) {
        var building = new Building(center, buildingData, previousInventory);

        foreach (Position position in building.myPositions) {
            var myTile = Grid.s.GetTile(position);
            if (myTile != null)
                myTile.simObject = building;
        }

        FactoryMaster.s.AddBuilding(building);

        UpdateBuildingNearConnectors(building);
        ObjectsUpdated?.Invoke();
        return building;
    }


    public static List<InventoryItemSlot> RemoveBuilding(Position location) {
        if(!(Grid.s.GetTile(location).simObject is Building building))
            return null;

        foreach (Position myPosition in building.myPositions) {
            var myTile = Grid.s.GetTile(myPosition);
            if (myTile != null)
                myTile.simObject = null;
        }

        FactoryMaster.s.RemoveBuilding(building);
        UpdateBuildingNearConnectors(building);
        ObjectsUpdated?.Invoke();
        FactoryDrones.storageBuildings.Remove(building);
        return building.inv.inventoryItemSlots;
    }

    static void UpdateBuildingNearConnectors(Building building) {
        foreach (Position myPosition in building.myPositions) {
            for (int i = 1; i <= 4; i++) {
                var tile = Grid.s.GetTile(Position.MoveCardinalDirection(myPosition, i, 1));
                if (tile.simObject is Connector connector) {
                    UpdateConnectorConnections(connector);
                }
            }
        }
    }
    
    public static Construction CreateConstructionFromSave(DataSaver.ConstructionSaveData saveData) {
        var bData = DataHolder.s.GetBuilding(saveData.myUniqueName);
        var construction = new Construction(bData, saveData.direction, saveData.center,
            saveData.myInvConverted(),
            DataSaver.InventoryData.ConvertToRegularData(saveData.afterConstructionInventory),
            saveData.isConstruction);
        if (saveData.isAssignedDrone) {
            construction.AssignToDrone();
        } else {
            construction.UnAssignFromDrone();
        }


        for (int i = 0; i < construction.locations.Count; i++) {
            Grid.s.GetTile(construction.locations[i]).simObject = construction;
        }

        FactoryMaster.s.AddConstruction(construction);
        
        return construction;
    }
    
    public static Belt CreateBeltFromSave(DataSaver.BeltSaveData saveData) {
        var belt = new Belt(saveData.start, saveData.end, saveData.direction);
        belt.items = CompressSaveToBeltSegments(saveData.myInvConverted());
        
        for (int i = 0; i < belt.length; i++) {
            Grid.s.GetTile(Position.MoveTowards(belt.startPos, belt.endPos, i)).simObject = belt;
        }
        
        FactoryMaster.s.AddBelt(belt);
        
        return belt;
    }
    
    public static Building CreateBuildingFromSave(DataSaver.BuildingSaveData saveData) {
        var bData = DataHolder.s.GetBuilding(saveData.myUniqueName);
        var building = new Building(saveData.center, bData, saveData.myInvConverted(), saveData.lastCheckid, saveData.curCraftingProgress);
        
        for (int i = 0; i < building.myPositions.Count; i++) {
            Grid.s.GetTile(building.myPositions[i]).simObject = building;
        }

        FactoryMaster.s.AddBuilding(building);
        
        return building;
    }

    
    public static Connector CreateConnectorFromSave(DataSaver.ConnectorSaveData saveData) {
        var connector = new Connector(saveData.start, saveData.end, saveData.direction);
        
        for (int i = 0; i < connector.length; i++) {
            Grid.s.GetTile(Position.MoveTowards(connector.startPos, connector.endPos, i)).simObject = connector;
        }
        
        FactoryMaster.s.AddConnector(connector);
        UpdateConnectorConnections(connector);
        
        return connector;
    }
    
    
    public static Drone CreateDroneFromSave(DataSaver.DroneSaveData saveData) {
        var drone = new Drone(saveData.curPosition) {
            targetPosition = saveData.targetPosition,
            isTravelling = saveData.isTravelling, isBusy = saveData.isBusy, isLaser = saveData.isLaser,
            myState = Drone.ConvertDroneStateAndInt(saveData.droneState),
        };
        if (saveData.currentTaskPosition.isValid()) {
            drone.currentTask = new DroneTask(saveData);
        }
        
        drone.myInventory.SetInventory(DataSaver.InventoryData.ConvertToRegularData(saveData.myInv));
        
        FactoryMaster.s.AddDrone(drone);
        
        return drone;
    }
    
    public static List<InventoryItemSlot> DecompressBeltForSaving(Belt belt) {
        return BeltCompressionUtility.DecompressBeltForSaving(belt);
    }
    
    public static List<Belt.BeltSegment> CompressSaveToBeltSegments(List<InventoryItemSlot> slots) {
        return BeltCompressionUtility.CompressSaveToBeltSegments(slots);
    }
}


public static class BeltCompressionUtility {
    public static List<InventoryItemSlot> DecompressBeltForSaving(Belt belt) {
        var decompressedItems = new List<InventoryItemSlot>();

        for (int i = 0; i < belt.items.Count; i++) {
            decompressedItems.Add(new InventoryItemSlot(belt.items[i].item, belt.items[i].count, belt.items[i].count, InventoryItemSlot.SlotType.storage));
        }

        return decompressedItems;
    }
    
    public static List<Belt.BeltSegment> CompressSaveToBeltSegments(List<InventoryItemSlot> slots) {
        var beltSlots = new List<Belt.BeltSegment>();

        for (int i = 0; i < slots.Count; i++) {
            beltSlots.Add(new Belt.BeltSegment() {item = slots[i].myItem, count = slots[i].count});
        }

        return beltSlots;
    }

    public static List<Item> DecompressBelt(List<Belt.BeltSegment> segments) {
        var decompressedItems = new List<Item>();

        for (int i = 0; i < segments.Count; i++) {
            for (int n = 0; n < segments[i].count; n++) {
                decompressedItems.Add(segments[i].item);
            }
        }

        return decompressedItems;
    }
    
    
    public static List<Belt.BeltSegment> CompressItemsToBelt(List<Item> items) {
        return _CompressItemsToBelt(items, 0, items.Count);
    }
    
    public static List<Belt.BeltSegment> CompressItemsToBelt(List<Item> items, int skipBeltCount, int endBeltCount) {
        var startIndex = skipBeltCount * FactoryMaster.SlotPerSegment;
        var endIndex = endBeltCount * FactoryMaster.SlotPerSegment;

        return _CompressItemsToBelt(items, startIndex, endIndex);
    }
    
    public static List<InventoryItemSlot> CompressItemsToInventorySlots(List<Item> items, int skipBeltCount, int endBeltCount) {
        var startIndex = skipBeltCount * FactoryMaster.SlotPerSegment;
        var endIndex = endBeltCount * FactoryMaster.SlotPerSegment;

        
        var segments = new List<InventoryItemSlot>();
        //Put items in the first belt
        segments.Add(new InventoryItemSlot(items[startIndex], 1, 1, InventoryItemSlot.SlotType.storage));
        for (int i = startIndex; i < endIndex; i++) {
            if (segments[segments.Count - 1].myItem == items[i]) {
                segments[segments.Count - 1].count += 1;
                segments[segments.Count - 1].maxCount += 1;
            } else {
                segments.Add(new InventoryItemSlot(items[i], 1, 1, InventoryItemSlot.SlotType.storage));
            }
        }

        return segments;
    }
    
    static List<Belt.BeltSegment> _CompressItemsToBelt(List<Item> items, int startIndex, int endIndex) {

        var segments = new List<Belt.BeltSegment>();
        //Put items in the first belt
        segments.Add(new Belt.BeltSegment() {count = 1, item = items[startIndex]});
        for (int i = startIndex+1; i < endIndex; i++) {
            if (segments[segments.Count - 1].item == items[i]) {
                segments[segments.Count - 1].AddCount(1);
            } else {
                segments.Add(new Belt.BeltSegment() {item = items[i], count = 1});
            }
        }

        return segments;
    }
}